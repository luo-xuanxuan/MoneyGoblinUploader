using FFXIVClientStructs.FFXIV.Client.Game;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;


namespace MoneyGoblin.Utils
{
    public class Packet
    {
        [JsonProperty]
        public List<PacketRow> entries;

        public unsafe Packet(string fcid, string player, string world, HousingWorkshopSubmersibleSubData * sub, int sub_id) {
            this.entries = new List<PacketRow>();
            long unixTimeSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            foreach (HousingWorkshopSubmarineGathered s in sub->GatheredData)
            {
                if (s.Point == 0)
                    break;
                this.entries.Add(new PacketRow(unixTimeSeconds, fcid, sub_id, player, world, s.Point, s.ItemIdPrimary, s.ItemCountPrimary));
                if(s.DoubleDip)
                    this.entries.Add(new PacketRow(unixTimeSeconds, fcid, sub_id, player, world, s.Point, s.ItemIdAdditional, s.ItemCountAdditional));
            }
        
        }

        public string getJSON()
        {
            string json = "[";
            json += entries[0].getJSON();
            for (int i = 1; i < entries.Count; i++)
            {
                json += "," + entries[i].getJSON();
            }
            json += "]";
            return json;
        }
    }

    public class PacketRow
    {
        [JsonProperty]
        public long time;
        [JsonProperty]
        public string fcid;
        [JsonProperty]
        public int sub_id;
        [JsonProperty]
        public string player;
        [JsonProperty]
        public string world;
        [JsonProperty]
        public byte sector_id;
        [JsonProperty]
        public uint item_id;
        [JsonProperty]
        public ushort quantity;

        public PacketRow(long time, string fcid, int sub_id, string player, string world, byte sector_id, uint item_id, ushort quantity)
        {
            this.time = time;
            this.fcid = fcid;
            this.player = player;
            this.world = world;
            this.sub_id = sub_id;
            this.sector_id = sector_id;
            this.item_id = item_id;
            this.quantity = quantity;
        }

        public string getJSON()
        {
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            return JsonConvert.SerializeObject(this, settings);
        }
    }

    public class ResourcePacket
    {
        [JsonProperty]
        public int inventory; //0 for player, 1 for fc
        [JsonProperty]
        public uint tanks;
        [JsonProperty]
        public uint repairs;
        [JsonProperty]
        public uint gil;
        [JsonProperty]
        public string player;
        [JsonProperty]
        public string world;
        [JsonProperty]
        public string fcid;

        public ResourcePacket(int inventory, uint tanks, uint repairs,uint gil, string player, string world, string fcid)
        {
            this.inventory = inventory;
            this.tanks = tanks;
            this.repairs = repairs;
            this.gil = gil;
            this.player = player;
            this.world = world;
            this.fcid = fcid;
        }
        public string getJSON()
        {
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            return JsonConvert.SerializeObject(this, settings);
        }
    }

    public class ReturnTimePacket
    {
        [JsonProperty]
        public uint return_time;
        [JsonProperty]
        public string fcid;
        [JsonProperty]
        public string name;
        [JsonProperty]
        public int sub_id;

        public ReturnTimePacket(uint return_time, string fcid, string name, int sub_id)
        {
            this.return_time = return_time;
            this.fcid = fcid;
            this.name = name;
            this.sub_id = sub_id;
        }
        
        public string getJSON()
        {
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            return JsonConvert.SerializeObject(this, settings);
        }
    }
}
