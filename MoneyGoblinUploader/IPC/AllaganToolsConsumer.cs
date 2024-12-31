using Dalamud.IoC;
using Dalamud.Plugin.Ipc;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.UI.Info;
using ImGuiNET;
using MoneyGoblin.Utils;
using MoneyGoblinUploader.Utils;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using static FFXIVClientStructs.ThisAssembly;
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

    private ICallGateSubscriber<ulong, HashSet<ulong[]>>? _getCharacterItemsSubscriber;
    private ICallGateSubscriber<ulong, uint, HashSet<ulong[]>>? _getCharacterItemsByTypeSubscriber;


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

            _getCharacterItemsSubscriber = Plugin.PluginInterface.GetIpcSubscriber<ulong, HashSet<ulong[]>>("AllaganTools.GetCharacterItems");
            _getCharacterItemsByTypeSubscriber = Plugin.PluginInterface.GetIpcSubscriber<ulong, uint, HashSet<ulong[]>>("AllaganTools.GetCharacterItemsByType");
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

        //Returns If Not Tanks or Repairs or Gil/Salvage
        if (ItemChanged.Id != 10155 && ItemChanged.Id != 10373 && !isGilItem(ItemChanged.Id))
            return;

        SendMessage(ItemChanged.CharacterId);

    }

    private static bool isGilItem(uint id)
    {
        switch (id)
        {
            case 1:
            case 22500:
            case 22501:
            case 22502:
            case 22503:
            case 22504:
            case 22505:
            case 22506: 
            case 22507:
                return true;
            default:
                return false;
        }
    }

    private uint getGil(ulong inventoryid)
    {
        uint gil = 0;
        gil += GetCount(1, inventoryid);
        gil += GetCount(22500, inventoryid) * 8000;
        gil += GetCount(22501, inventoryid) * 9000;
        gil += GetCount(22502, inventoryid) * 10000;
        gil += GetCount(22503, inventoryid) * 13000;
        gil += GetCount(22504, inventoryid) * 27000;
        gil += GetCount(22505, inventoryid) * 28500;
        gil += GetCount(22506, inventoryid) * 30000;
        gil += GetCount(22507, inventoryid) * 34500;
        return gil;
    }

    private static Dictionary<ulong, uint> monitoredValues = new Dictionary<ulong, uint>();

    public void GilMonitor(ulong id)
    {

        // Get the current value for the given ID
        uint currentValue = GetCount(1, id);

        // Check if the ID is already being monitored
        if (monitoredValues.TryGetValue(id, out uint oldValue))
        {
            // If the value has changed, update the dictionary and send a message
            if (oldValue != currentValue)
            {
                monitoredValues[id] = currentValue;
                SendMessage(id);
            }
        }
        else
        {
            // Add new ID to the dictionary and send a message
            monitoredValues[id] = currentValue;
            SendMessage(id);
        }
    }

    private unsafe void SendMessage(ulong id)
    {
        int inventory_type = -1;
        var fc = (InfoProxyFreeCompany*)InfoModule.Instance()->GetInfoProxyById(InfoProxyId.FreeCompany);
        var fcid = fc->Id;

        if (id == Plugin.ClientState.LocalContentId)
            inventory_type = 0;

        if (id == fcid)
            inventory_type = 1;

        if (inventory_type == -1)
            return; //idk retainer prolly? Not gonna cover this case rn

        var playerName = Plugin.ClientState.LocalPlayer.Name.ToString();
        var world = Plugin.ClientState.LocalPlayer.CurrentWorld.Value.Name.ToString();
        //var world = Plugin.ClientState.LocalPlayer.CurrentWorld.GameData.Name.ToString();

        ResourcePacket p = new ResourcePacket(inventory_type, GetCount(10155, id), GetCount(10373, id), getGil(id), playerName, world, id.ToString());

        Upload.PostJson(p.getJSON(), Plugin.Configuration.TargetAddress, "resources");
    }

}
