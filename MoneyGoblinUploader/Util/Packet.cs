using FFXIVClientStructs.FFXIV.Client.Game.Housing;
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

        public unsafe Packet(string fcid, string player, string world, HousingWorkshopSubmersibleSubData * sub) {
            this.entries = new List<PacketRow>();
            long unixTimeSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            foreach (HousingWorkshopSubmarineGathered s in sub->GatheredDataSpan)
            {
                if (s.Point == 0)
                    break;
                this.entries.Add(new PacketRow(unixTimeSeconds, fcid, player, world, Encoding.UTF8.GetString(sub->Name, 20).TrimEnd('\0'), s.Point, s.ItemIdPrimary, s.ItemCountPrimary));
                if(s.DoubleDip)
                    this.entries.Add(new PacketRow(unixTimeSeconds, fcid, player, world, Encoding.UTF8.GetString(sub->Name, 20).TrimEnd('\0'), s.Point, s.ItemIdAdditional, s.ItemCountAdditional));
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
        public string player;
        [JsonProperty]
        public string world;
        [JsonProperty]
        public string sub;
        [JsonProperty]
        public byte sector_id;
        [JsonProperty]
        public uint item_id;
        [JsonProperty]
        public ushort quantity;

        public PacketRow(long time, string fcid, string player, string world, string sub, byte sector_id, uint item_id, ushort quantity)
        {
            this.time = time;
            this.fcid = fcid;
            this.player = player;
            this.world = world;
            this.sub = sub;
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
}
