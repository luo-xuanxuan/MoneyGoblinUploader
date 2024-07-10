using System;
using System.Text;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Info;
using MoneyGoblinUploader.Utils;

namespace MoneyGoblin.Utils;

public class HookManager
{
    private readonly Plugin Plugin;

    private const string PacketReceiverSig = "E8 ?? ?? ?? ?? E9 ?? ?? ?? ?? 44 0F B6 43 ?? 4C 8D 4B 17";
    private const string PacketReceiverSigCN = "E8 ?? ?? ?? ?? E9 ?? ?? ?? ?? 44 0F B6 46 ??";
    private delegate void PacketDelegate(uint param1, ushort param2, sbyte param3, Int64 param4, char param5);
    private readonly Hook<PacketDelegate> PacketHandlerHook;

    public HookManager(Plugin plugin)
    {
        Plugin = plugin;

        // Try to resolve the CN sig if normal one fails ...
        // Doing this because CN people use an outdated version that still uploads data
        // so trying to get them at least somewhat up to date
        nint packetReceiverPtr;
        try
        {
            packetReceiverPtr = Plugin.SigScanner.ScanText(PacketReceiverSig);
        }
        catch (Exception e)
        {
            Plugin.Log.Error("Exception in sig scan, maybe CN client?");
            packetReceiverPtr = Plugin.SigScanner.ScanText(PacketReceiverSigCN);
        }

        PacketHandlerHook = Plugin.Hook.HookFromAddress<PacketDelegate>(packetReceiverPtr, PacketReceiver);
        PacketHandlerHook.Enable();
    }

    public void Dispose()
    {
        PacketHandlerHook.Dispose();
    }

    private unsafe void PacketReceiver(uint param1, ushort param2, sbyte param3, Int64 param4, char param5)
    {
        PacketHandlerHook.Original(param1, param2, param3, param4, param5);

        // We only care about voyage Result
        if (param1 != 721343)
            return;

        try
        {
            var instance = HousingManager.Instance();

            if (instance == null || instance->WorkshopTerritory == null)
                return;

            var current = instance->WorkshopTerritory->Submersible.DataPointers[4];
            if (current.Value == null)
                return;

            var sub = current.Value;

            var playerName = Plugin.ClientState.LocalPlayer.Name.ToString();
            var world = Plugin.ClientState.LocalPlayer.CurrentWorld.GameData.Name.ToString();
            var fc = (InfoProxyFreeCompany*)InfoModule.Instance()->GetInfoProxyById(InfoProxyId.FreeCompany);
            ReadOnlySpan<byte> nameSpan = fc->Name.Slice(0, 16);
            var fcName = Encoding.UTF8.GetString(nameSpan).TrimEnd('\0');
            string fcid = fc->Id.ToString();

            int sub_id = -1;

            for(int i = 0; i < 4; i++)
            {
                if(instance->WorkshopTerritory->Submersible.DataPointers[i].Value->RegisterTime == sub->RegisterTime)
                {
                    sub_id = i;
                    break;
                }
            }

            var p = new Packet(fcid, playerName, world, sub, sub_id);

            Upload.PostJson(p.getJSON(), Plugin.Configuration.TargetAddress);

        }
        catch (Exception e)
        {
            Plugin.Log.Error(e.Message);
            Plugin.Log.Error(e.StackTrace ?? "Unknown");
        }
    }
}
