using FFXIVClientStructs.FFXIV.Client.Game;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MoneyGoblinUploader.Util
{
    public class VoyageDataPacket
    {
        [JsonProperty]
        public byte sector_id;
        [JsonProperty]
        public byte exp_rating;
        [JsonProperty]
        public byte discovered_id;
        [JsonProperty]
        public ushort surveillance;
        [JsonProperty]
        public ushort retrieval;
        [JsonProperty]
        public ushort favor;
        [JsonProperty]
        public byte is_double_dip;
        [JsonProperty]
        public byte surveillance_result;
        [JsonProperty]
        public byte retrieval_result;
        [JsonProperty]
        public byte favor_result;
        [JsonProperty]
        public byte quality_result;
        [JsonProperty]
        public uint item_id;
        [JsonProperty]
        public ushort quantity;
        [JsonProperty]
        public byte is_hq;

        public string getJSON()
        {
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            return JsonConvert.SerializeObject(this, settings);
        }

    }


    [StructLayout(LayoutKind.Explicit, Size = 0x1E)]
    public unsafe struct VoyageBinPacket
    {
        [FieldOffset(0x0)] public byte sector_id;
        [FieldOffset(0x1)] public byte exp_rating;
        [FieldOffset(0x2)] public byte discovered_id;
        [FieldOffset(0x3)] public ushort surveillance;
        [FieldOffset(0x5)] public ushort retrieval;
        [FieldOffset(0x7)] public ushort favor;
        [FieldOffset(0x9)] public byte favor_result;
        [FieldOffset(0xA)] public fixed uint item_id[2];
        [FieldOffset(0x12)] public fixed ushort quantity[2];
        [FieldOffset(0x16)] public fixed byte is_hq[2];
        [FieldOffset(0x18)] public fixed byte surveillance_result[2];
        [FieldOffset(0x1A)] public fixed byte retrieval_result[2];
        [FieldOffset(0x1C)] public fixed byte quality_result[2];

        public VoyageBinPacket(SubmersibleSectorData * data, ushort surveillance, ushort retrieval, ushort favor)
        {
            this.sector_id = data->SectorId;
            this.exp_rating = data->ExperienceRating;
            this.discovered_id = data->DiscoveredSectorId;
            this.surveillance = surveillance;
            this.retrieval = retrieval;
            this.favor = favor;
            this.favor_result = (byte)data->FavorResult;
            this.item_id[0] = data->ItemId[0];
            this.item_id[1] = data->ItemId[1];
            this.quantity[0] = data->Quantity[0];
            this.quantity[1] = data->Quantity[1];
            this.is_hq[0] = data->isHQ[0];
            this.is_hq[1] = data->isHQ[1];
            this.surveillance_result[0] = (byte)data->SurveillanceResult[0];
            this.surveillance_result[1] = (byte)data->SurveillanceResult[1];
            this.retrieval_result[0] = (byte)data->RetrievalResult[0];
            this.retrieval_result[1] = (byte)data->RetrievalResult[1];
            this.quality_result[0] = (byte)data->QualityResult[0];
            this.quality_result[1] = (byte)data->QualityResult[1];
        }

    }


    public class VoyageLogData
    {
        [JsonProperty]
        public int Surveillance;
        [JsonProperty]
        public int Retrieval;
        [JsonProperty]
        public int Favor;
        [JsonProperty]
        public List<VoyageSectorData> Log;

        public unsafe VoyageLogData(HousingWorkshopSubmersibleSubData submersibleData)
        {
            LuminaHelper lumina = new LuminaHelper();

            this.Log = new List<VoyageSectorData>();

            int experienceGained = 0;

            for (int i = 0; i < 5; i++)
            {

                if (submersibleData.GatheredData[i].Point != 0)
                {
                    VoyageSectorData sector = new VoyageSectorData(submersibleData.GatheredData[i]);
                    experienceGained += sector.Experience;
                    this.Log.Add(sector);
                    continue;
                }
                break;
            }

            if((int)submersibleData.CurrentExp - experienceGained < 0)
            {
                var rankStats = lumina.getStatsByRank(submersibleData.RankId - 1);
                this.Surveillance = rankStats.Surveillance + submersibleData.SurveillanceBase;
                this.Retrieval = rankStats.Retrieval + submersibleData.RetrievalBase;
                this.Favor = rankStats.Favor + submersibleData.FavorBase;
            } else
            {
                var rankStats = lumina.getStatsByRank(submersibleData.RankId);
                this.Surveillance = rankStats.Surveillance + submersibleData.SurveillanceBase;
                this.Retrieval = rankStats.Retrieval + submersibleData.RetrievalBase;
                this.Favor = rankStats.Favor + submersibleData.FavorBase;
            }

        }

        public List<string> getRouteLetters()
        {
            List<string> route = new List<string>();
            foreach (VoyageSectorData sector in this.Log)
            {
                route.Add(sector.getLetterID());
            }
            return route;
        }
        public string getJSON()
        {
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            return JsonConvert.SerializeObject(this, settings);
        }
    }

    public class VoyageSectorData
    {
        [JsonProperty]
        public int SectorId;
        [JsonProperty]
        public int ExperienceRating;
        [JsonProperty]
        public int DiscoveredSectorId;
        [JsonProperty]
        public bool IsFirstTimeExplored;
        [JsonProperty]
        public int UnlockedSubmarine;
        [JsonProperty]
        public bool IsDoubleDip;
        [JsonProperty]
        public int FavorResult;
        [JsonProperty]
        public int Experience;
        [JsonProperty]
        public List<DipData> DipData;

        public unsafe VoyageSectorData(HousingWorkshopSubmarineGathered sector)
        {
            this.DipData = new List<DipData>();
            this.SectorId = sector.Point;
            this.ExperienceRating = ((int)sector.PointRating);
            this.DiscoveredSectorId = sector.UnlockedPoint;
            this.IsFirstTimeExplored = sector.FirstExploration;
            this.UnlockedSubmarine = 0;
            this.IsDoubleDip = sector.DoubleDip;
            this.FavorResult = (int)sector.FavorLine;
            this.Experience = (int)sector.ExpGained;
            DipData dip = new DipData((int)sector.ItemIdPrimary, sector.ItemCountPrimary, sector.ItemHQPrimary, sector.UnknownPrimary == 1, (int)sector.SurveyLinePrimary, (int)sector.YieldLinePrimary, (int)sector.DiscoveredLinePrimary);
            this.DipData.Add(dip);
            if((int)sector.ItemIdAdditional != 0)
            {
                DipData dip2 = new DipData((int)sector.ItemIdAdditional, sector.ItemCountAdditional, sector.ItemHQAdditional, sector.UnknownAdditional == 1, (int)sector.SurveyLineAdditional, (int)sector.YieldLineAdditional, (int)sector.DiscoveredLineAdditional);
                this.DipData.Add(dip2);
            }
        }

        public string getLetterID()
        {
            string letter = "";
            switch(this.SectorId)
            {
                case 1:
                case 32:
                case 53:
                case 74:
                    letter = "A";
                    break;
                case 2:
                case 33:
                case 54:
                case 75:
                    letter = "B";
                    break;
                case 3:
                case 34:
                case 55:
                case 76:
                    letter = "C";
                    break;
                case 4:
                case 35:
                case 56:
                case 77:
                    letter = "D";
                    break;
                case 5:
                case 36:
                case 57:
                case 78:
                    letter = "E";
                    break;
                case 6:
                case 37:
                case 58:
                case 79:
                    letter = "F";
                    break;
                case 7:
                case 38:
                case 59:
                case 80:
                    letter = "G";
                    break;
                case 8:
                case 39:
                case 60:
                case 81:
                    letter = "H";
                    break;
                case 9:
                case 40:
                case 61:
                case 82:
                    letter = "I";
                    break;
                case 10:
                case 41:
                case 62:
                case 83:
                    letter = "J";
                    break;
                case 11:
                case 42:
                case 63:
                case 84:
                    letter = "K";
                    break;
                case 12:
                case 43:
                case 64:
                case 85:
                    letter = "L";
                    break;
                case 13:
                case 44:
                case 65:
                case 86:
                    letter = "M";
                    break;
                case 14:
                case 45:
                case 66:
                case 87:
                    letter = "N";
                    break;
                case 15:
                case 46:
                case 67:
                case 88:
                    letter = "O";
                    break;
                case 16:
                case 47:
                case 68:
                case 89:
                    letter = "P";
                    break;
                case 17:
                case 48:
                case 69:
                case 90:
                    letter = "Q";
                    break;
                case 18:
                case 49:
                case 70:
                case 91:
                    letter = "R";
                    break;
                case 19:
                case 50:
                case 71:
                case 92:
                    letter = "S";
                    break;
                case 20:
                case 51:
                case 72:
                case 93:
                    letter = "T";
                    break;
                case 21:
                    letter = "U";
                    break;
                case 22:
                    letter = "V";
                    break;
                case 23:
                    letter = "W";
                    break;
                case 24:
                    letter = "X";
                    break;
                case 25:
                    letter = "Y";
                    break;
                case 26:
                    letter = "Z";
                    break;
                case 27:
                    letter = "AA";
                    break;
                case 28:
                    letter = "AB";
                    break;
                case 29:
                    letter = "AC";
                    break;
                case 30:
                    letter = "AD";
                    break;
            }
            return letter;
        }
    }

    public class DipData
    {
        [JsonProperty]
        public int ItemId;
        [JsonProperty]
        public int Quantity;
        [JsonProperty]
        public bool IsHq;
        [JsonProperty]
        public bool IsNotTier3;
        [JsonProperty]
        public int SurveillanceResult;
        [JsonProperty]
        public int RetrievalResult;
        [JsonProperty]
        public int QualityResult;

        public unsafe DipData(int id, int quantity, bool ishq, bool nott3, int surv, int ret, int quality)
        {
            this.ItemId = id;
            this.Quantity = quantity;
            this.IsHq = ishq;
            this.IsNotTier3 = nott3;
            this.SurveillanceResult = surv;
            this.RetrievalResult = ret;
            this.QualityResult = quality;
        }
    }
}
