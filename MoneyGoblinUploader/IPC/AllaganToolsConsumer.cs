using Dalamud.IoC;
using Dalamud.Plugin.Ipc;
using FFXIVClientStructs.FFXIV.Client.UI.Info;
using MoneyGoblin.Utils;
using MoneyGoblinUploader.Utils;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using InventoryItem = FFXIVClientStructs.FFXIV.Client.Game.InventoryItem;

namespace MoneyGoblin.IPC;

public class AllaganToolsConsumer
{
    private bool Available;
    private long TimeSinceLastCheck;

    private readonly Plugin Plugin;

    public AllaganToolsConsumer(Plugin plugin)
    {
        Plugin = plugin;
        Subscribe();
    }

    public bool IsAvailable
    {
        get
        {
            if (TimeSinceLastCheck + 5000 > Environment.TickCount64)
            {
                return Available;
            }

            try
            {
                TimeSinceLastCheck = Environment.TickCount64;

                IsInitialized.InvokeFunc();
                Available = true;
            }
            catch
            {
                Available = false;
            }

            return Available;
        }
    }

    private ICallGateSubscriber<bool> IsInitialized = null!;
    private ICallGateSubscriber<uint, ulong, int, uint> ItemCount = null!;
    private ICallGateSubscriber<(uint, InventoryItem.ItemFlags, ulong, uint), bool> ItemAdded = null!;
    private ICallGateSubscriber<(uint, InventoryItem.ItemFlags, ulong, uint), bool> ItemRemoved = null!;

    private void Subscribe()
    {
        try
        {
            IsInitialized = Plugin.PluginInterface.GetIpcSubscriber<bool>("AllaganTools.IsInitialized");
            ItemCount = Plugin.PluginInterface.GetIpcSubscriber<uint, ulong, int, uint>("AllaganTools.ItemCount");
            ItemAdded = Plugin.PluginInterface.GetIpcSubscriber<(uint, InventoryItem.ItemFlags, ulong, uint), bool>("AllaganTools.ItemAdded");
            ItemRemoved = Plugin.PluginInterface.GetIpcSubscriber<(uint, InventoryItem.ItemFlags, ulong, uint), bool>("AllaganTools.ItemRemoved");
            ItemAdded.Subscribe(InventoryChanged);
            ItemRemoved.Subscribe(InventoryChanged);
        }
        catch (Exception e)
        {
            Plugin.Log.Debug($"Failed to subscribe to AllaganTools\nReason: {e}");
        }
    }

    public void Unsubscribe()
    {
        ItemAdded.Unsubscribe(InventoryChanged);
        ItemRemoved.Unsubscribe(InventoryChanged);
    }

    public uint GetCount(uint itemId, ulong characterId)
    {
        try
        {
            // -1 checks all inventories
            return ItemCount.InvokeFunc(itemId, characterId, -1);
        }
        catch
        {
            //Plugin.ChatGui.PrintError(Utils.ErrorMessage("AllaganTools plugin is not responding"));
            return uint.MaxValue;
        }
    }

    public unsafe void InventoryChanged((uint Id, InventoryItem.ItemFlags Flags, ulong CharacterId, uint Quantity) ItemChanged)
    {
        //Returns If Not Tanks or Repairs
        if (ItemChanged.Id != 10155 && ItemChanged.Id != 10373)
            return;

        int inventory_type = -1;
        var fc = (InfoProxyFreeCompany*)InfoModule.Instance()->GetInfoProxyById(InfoProxyId.FreeCompany);
        var fcid = fc->Id;

        if (ItemChanged.CharacterId == Plugin.ClientState.LocalContentId)
            inventory_type = 0;

        if (ItemChanged.CharacterId == fcid)
            inventory_type = 1;

        if (inventory_type == -1)
            return; //idk retainer prolly? Not gonna cover this case rn

        var playerName = Plugin.ClientState.LocalPlayer.Name.ToString();
        var world = Plugin.ClientState.LocalPlayer.CurrentWorld.Value.Name.ToString();
        //var world = Plugin.ClientState.LocalPlayer.CurrentWorld.GameData.Name.ToString();

        ResourcePacket p = new ResourcePacket(inventory_type, GetCount(10155, ItemChanged.CharacterId), GetCount(10373, ItemChanged.CharacterId), playerName, world, fcid.ToString());

        Upload.PostJson(p.getJSON(), Plugin.Configuration.TargetAddress, "resources");

    }

}
