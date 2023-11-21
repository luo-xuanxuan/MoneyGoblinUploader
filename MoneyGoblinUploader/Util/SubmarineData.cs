using FFXIVClientStructs.FFXIV.Client.Game;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;

namespace MoneyGoblinUploader.Util
{
    public class SubmarineDataPacket
    {
        [JsonProperty]
        public SubmarineData Submarine;
        public unsafe SubmarineDataPacket(SubmarineData sub)
        {
            this.Submarine = sub;
        }

    }

    public class SubmarineData
    {
        [JsonProperty]
        public string SubmarineName;
        [JsonProperty]
        public int SubmarineId;
        [JsonProperty]
        public int Rank;
        [JsonProperty]
        public int Experience;
        [JsonProperty]
        public int HullId;
        [JsonProperty]
        public int SternId;
        [JsonProperty]
        public int BowId;
        [JsonProperty]
        public int BridgeId;
        [JsonProperty]
        public ShipCondition Condition;
        [JsonIgnore]
        public int Surveillance;
        [JsonIgnore]
        public int Retrieval;
        [JsonIgnore]
        public int Favor;
        [JsonIgnore]
        public int Range;
        [JsonIgnore]
        public int Speed;
        [JsonProperty]
        public uint ReturnTime;
        [JsonProperty]
        public List<int> Voyage;

        public SubmarineData()
        {
            this.SubmarineName = "";
            this.SubmarineId = 0;
            this.Rank = 0;
            this.Experience = 0;
            this.HullId = 0;
            this.SternId = 0;
            this.BowId = 0;
            this.BridgeId = 0;
            this.ReturnTime = 0;
            this.Surveillance = 0;
            this.Retrieval = 0;
            this.Favor = 0;
            this.Range = 0;
            this.Speed = 0;
            this.Condition = new ShipCondition();
            this.Voyage = new List<int>();
        }

        public unsafe SubmarineData(SubmersibleData submersibleData, InventoryContainer* inventory, int id)
        {

            this.SubmarineName = Encoding.UTF8.GetString(submersibleData.Name, 20).TrimEnd('\0');
            this.SubmarineId = id;
            this.Rank = submersibleData.RankId;
            this.Experience = (int)submersibleData.Experience;
            this.HullId = submersibleData.HullId;
            this.SternId = submersibleData.SternId;
            this.BowId = submersibleData.BowId;
            this.BridgeId = submersibleData.BridgeId;
            this.Condition = new ShipCondition(inventory,id);
            this.ReturnTime = submersibleData.ReturnTime;
            this.Surveillance = submersibleData.SurveillanceBase + submersibleData.SurveillanceBonus;
            this.Retrieval = submersibleData.RetrievalBase + submersibleData.RetrievalBonus;
            this.Favor = submersibleData.FavorBase + submersibleData.FavorBonus;
            this.Range = submersibleData.RangeBase + submersibleData.RangeBonus;
            this.Speed = submersibleData.SpeedBase + submersibleData.SpeedBonus;
            this.Voyage = new List<int>();
            for (int i = 0; i < 5; i++)
            {
                if (submersibleData.Route[i] != 0)
                {
                    this.Voyage.Add(submersibleData.Route[i]);
                    continue;
                }
                break;
            }
        }

        public static bool operator ==(SubmarineData sub1, SubmarineData sub2)
        {
            if (sub1.SubmarineId != sub2.SubmarineId) return false;
            if (sub1.SubmarineName != sub2.SubmarineName) return false;
            if (sub1.Rank != sub2.Rank) return false;
            if (sub1.Experience != sub2.Experience) return false;
            if (sub1.HullId != sub2.HullId) return false;
            if (sub1.SternId != sub2.SternId) return false;
            if (sub1.BowId != sub2.BowId) return false;
            if (sub1.BridgeId != sub2.BridgeId) return false;
            if (sub1.Condition.HullCondition != sub2.Condition.HullCondition) return false;
            if (sub1.Condition.SternCondition != sub2.Condition.SternCondition) return false;
            if (sub1.Condition.BowCondition != sub2.Condition.BowCondition) return false;
            if (sub1.Condition.BridgeCondition != sub2.Condition.BridgeCondition) return false;
            if (sub1.ReturnTime != sub2.ReturnTime) return false;
            if (sub1.Voyage.Count != sub2.Voyage.Count) return false;
            for (int i = 0; i < sub1.Voyage.Count; i++)
                if (sub1.Voyage[i] != sub2.Voyage[i]) return false;
            return true;
        }

        public static bool operator !=(SubmarineData sub1, SubmarineData sub2)
        {
            if (sub1.SubmarineId != sub2.SubmarineId) return true;
            if (sub1.SubmarineName != sub2.SubmarineName) return true;
            if (sub1.Rank != sub2.Rank) return true;
            if (sub1.Experience != sub2.Experience) return true;
            if (sub1.HullId != sub2.HullId) return true;
            if (sub1.SternId != sub2.SternId) return true;
            if (sub1.BowId != sub2.BowId) return true;
            if (sub1.BridgeId != sub2.BridgeId) return true;
            if (sub1.Condition.HullCondition != sub2.Condition.HullCondition) return true;
            if (sub1.Condition.SternCondition != sub2.Condition.SternCondition) return true;
            if (sub1.Condition.BowCondition != sub2.Condition.BowCondition) return true;
            if (sub1.Condition.BridgeCondition != sub2.Condition.BridgeCondition) return true;
            if (sub1.ReturnTime != sub2.ReturnTime) return true;
            if (sub1.Voyage.Count != sub2.Voyage.Count) return true;
            for (int i = 0; i < sub1.Voyage.Count; i++)
                if (sub1.Voyage[i] != sub2.Voyage[i]) return true;
            return false;
        }

        public string getBuild()
        {
            var idToLetterMap = new Dictionary<int, string>()
            {
                { 1, "S" },
                { 2, "U" },
                { 3, "W" },
                { 4, "C" },
                { 5, "Y" },
                { 21, "S+" },
                { 22, "U+" },
                { 23, "W+" },
                { 24, "C+" },
                { 25, "Y+" }
            };

            var build = new StringBuilder();

            if (HullId <= 20 || SternId <= 20 || BowId <= 20 || BridgeId <= 20)
            {
                foreach (var partId in new[] { HullId, SternId, BowId, BridgeId })
                {
                    if (idToLetterMap.TryGetValue(partId, out var letter))
                    {
                        build.Append(letter[0]);
                        if (letter.EndsWith("+"))
                        {
                            build.Append("+");
                        }
                    }
                }
            }
            else
            {
                foreach (var partId in new[] { HullId, SternId, BowId, BridgeId })
                {
                    if (idToLetterMap.TryGetValue(partId, out var letter))
                    {
                        build.Append(letter[0]);
                    }
                }

                build.Append("++");
            }

            return build.ToString();
        }
    }
}
