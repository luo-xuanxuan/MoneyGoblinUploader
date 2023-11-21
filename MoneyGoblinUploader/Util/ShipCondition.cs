using FFXIVClientStructs.FFXIV.Client.Game;
using Newtonsoft.Json;

namespace MoneyGoblinUploader.Util
{
    public class DamageDataPacket
    {
        [JsonProperty]
        DamageData Damage;
        [JsonProperty]
        UploaderVersionPacket UploaderVersion;
        public DamageDataPacket(DamageData Damage)
        {
            this.Damage = Damage;
            this.UploaderVersion = new UploaderVersionPacket();
        }

        public string getJSON()
        {
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            return JsonConvert.SerializeObject(this, settings);
        }
    }

    public class DamageData
    {
        [JsonProperty]
        public int HullId;
        [JsonProperty]
        public int SternId;
        [JsonProperty]
        public int BowId;
        [JsonProperty]
        public int BridgeId;

        [JsonProperty]
        ShipCondition InitialCondition;
        [JsonProperty]
        ShipCondition FinalCondition;

        [JsonProperty]
        public int[]? Route;

        public unsafe DamageData(SubmersibleData submersibleData, ShipCondition condition, ShipCondition condition2)
        {
            this.HullId = submersibleData.HullId;
            this.SternId = submersibleData.SternId;
            this.BowId = submersibleData.BowId;
            this.BridgeId = submersibleData.BridgeId;
            this.InitialCondition = condition;
            this.FinalCondition = condition2;
            this.Route = new int[5];
            for (int i = 0; i < 5; i++)
                this.Route[i] = (int)submersibleData.Route[i];
        }

        public string getJSON()
        {
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            return JsonConvert.SerializeObject(this, settings);
        }
    }
    public class ShipCondition
    {
        [JsonProperty]
        public int HullCondition;
        [JsonProperty]
        public int SternCondition;
        [JsonProperty]
        public int BowCondition;
        [JsonProperty]
        public int BridgeCondition;

        public ShipCondition()
        {
            HullCondition = 0;
            SternCondition = 0;
            BowCondition = 0;
            BridgeCondition = 0;
        }

        public unsafe ShipCondition(InventoryContainer* inventory, int id)
        {
            this.HullCondition = (inventory->GetInventorySlot(0 + (id * 5)))->Condition;
            this.SternCondition = (inventory->GetInventorySlot(1 + (id * 5)))->Condition;
            this.BowCondition = (inventory->GetInventorySlot(2 + (id * 5)))->Condition;
            this.BridgeCondition = (inventory->GetInventorySlot(3 + (id * 5)))->Condition;
        }

        public static bool operator ==(ShipCondition ship1, ShipCondition ship2)
        {
            if (ship1.HullCondition != ship2.HullCondition) return false;
            if (ship1.SternCondition != ship2.SternCondition) return false;
            if (ship1.BowCondition != ship2.BowCondition) return false;
            if (ship1.BridgeCondition != ship2.BridgeCondition) return false;
            return true;
        }

        public static bool operator !=(ShipCondition ship1, ShipCondition ship2)
        {
            if (ship1.HullCondition != ship2.HullCondition) return true;
            if (ship1.SternCondition != ship2.SternCondition) return true;
            if (ship1.BowCondition != ship2.BowCondition) return true;
            if (ship1.BridgeCondition != ship2.BridgeCondition) return true;
            return false;
        }

        public string getJSON()
        {
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            return JsonConvert.SerializeObject(this, settings);
        }
    }
}
