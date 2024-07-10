using FFXIVClientStructs.Interop;
using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace MoneyGoblinUploader.Util
{
    [StructLayout(LayoutKind.Explicit, Size = 0xB8C0)]
    public unsafe partial struct HousingWorkshopTerritory
    {
        //[FixedSizeArray<AirshipData>(4)]
        [FieldOffset(0x68)] public fixed byte AirshipDataList[0x1C0 * 4];

        [FieldOffset(0x7D8)] public byte ActiveAirshipId; // 0-3, 255 if none
        [FieldOffset(0x7D9)] public byte AirshipCount;

        [FieldOffset(0x2950)] public fixed byte AishipUnlockedSectorFlags[0x4];
        [FieldOffset(0x2954)] public fixed byte AishipExploredSectorFlags[0x4];

        //[FixedSizeArray<SubmersibleData>(4)]
        [FieldOffset(0x2960)] public fixed byte SubmersibleDataList[0x2320 * 4];

        //[FixedSizeArray<Pointer<SubmersibleData>>(5)]
        [FieldOffset(0xB5E0)] public fixed byte SubmersibleDataPointerList[0x8 * 5]; // 0-3 is the same as SubmersibleDataList, 4 is the one you are currently using
        [FieldOffset(0xB6F9)] public fixed byte SubmersibleUnlockedSectorFlags[0xB];
        [FieldOffset(0xB708)] public fixed byte SubmersibleExploredSectorFlags[0xB];
        //public Span<SubmersibleData> SubmersibleList => new(Unsafe.AsPointer(ref SubmersibleDataList[0]), 4);
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x1C0)]
    public unsafe partial struct AirshipData
    {
        [FieldOffset(0x4)] public uint RegisterTime;
        [FieldOffset(0xC)] public byte RankId;
        [FieldOffset(0x10)] public uint ReturnTime;
        [FieldOffset(0x14)] public uint CurrentExp;
        [FieldOffset(0x18)] public uint NextLevelExp;
        [FieldOffset(0x1C)] public uint MaxCapacity;

        [FieldOffset(0x20)] public ushort HullId;
        [FieldOffset(0x22)] public ushort RiggingId;
        [FieldOffset(0x24)] public ushort ForecastleId;
        [FieldOffset(0x26)] public ushort AftcastleId;

        [FieldOffset(0x2E)] public ushort Surveillance;
        [FieldOffset(0x30)] public ushort Retrieval;
        [FieldOffset(0x38)] public ushort Speed;
        [FieldOffset(0x3A)] public ushort Range;
        [FieldOffset(0x3C)] public ushort Favor;

        [FieldOffset(0x3A)] public fixed byte Route[5];

        [FieldOffset(0x37)] public fixed byte Name[20];

        [FieldOffset(0x50)] public ushort LogRating;
        [FieldOffset(0x52)] public ushort VoyageSpeed; //? Might update to current speed, weird value

        //[FixedSizeArray<AirshipSectorData>(5)]
        //[FieldOffset(0x54)] public fixed byte VoyageSectorData[0x38 * 5];
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x38)]
    public unsafe partial struct AirshipSectorData
    {
        [FieldOffset(0x0)] public uint Experience;
        [FieldOffset(0x4)] public uint FavorResult;
        [FieldOffset(0x8)] public byte SectorId;
        [FieldOffset(0x9)] public byte DiscoveredSectorId;
        [FieldOffset(0xA)] public byte ExperienceRating;
        [FieldOffset(0xB)] public byte UnlockedAirship;
        [FieldOffset(0xC)] public fixed uint ItemId[2];
        [FieldOffset(0x14)] public fixed ushort Quantity[2];
        [FieldOffset(0x18)] public fixed uint SurveillanceResult[2];
        [FieldOffset(0x20)] public fixed uint RetrievalResult[2];
        [FieldOffset(0x28)] public fixed uint QualityResult[2];

        [FieldOffset(0x33)] public byte IsDoubleDip;
        [FieldOffset(0x34)] public fixed byte IsHq[2];
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x2320)]
    public unsafe partial struct SubmersibleData
    {
        [FieldOffset(0x0)] public SubmersibleData* Self;
        [FieldOffset(0xC)] public ushort Status; // 0:Unregistered 1:Idle 2:Exploring
        [FieldOffset(0xE)] public byte RankId;
        [FieldOffset(0x10)] public uint RegisterTime;
        [FieldOffset(0x14)] public uint ReturnTime;
        [FieldOffset(0x18)] public uint Experience;
        [FieldOffset(0x1C)] public uint ExperienceToNextRank;

        [FieldOffset(0x20)] public byte MaxCapacity;
        [FieldOffset(0x22)] public fixed byte Name[20];

        [FieldOffset(0x3A)] public ushort HullId;
        [FieldOffset(0x3C)] public ushort SternId;
        [FieldOffset(0x3E)] public ushort BowId;
        [FieldOffset(0x40)] public ushort BridgeId;

        [FieldOffset(0x42)] public fixed byte Route[5];

        [FieldOffset(0x4A)] public ushort SurveillanceBase;
        [FieldOffset(0x4C)] public ushort RetrievalBase;
        [FieldOffset(0x4E)] public ushort SpeedBase;
        [FieldOffset(0x50)] public ushort RangeBase;
        [FieldOffset(0x52)] public ushort FavorBase;

        [FieldOffset(0x54)] public ushort SurveillanceBonus;
        [FieldOffset(0x56)] public ushort RetrievalBonus;
        [FieldOffset(0x58)] public ushort SpeedBonus;
        [FieldOffset(0x5A)] public ushort RangeBonus;
        [FieldOffset(0x5C)] public ushort FavorBonus;

        //[FixedSizeArray<SubmersibleSectorData>(5)]
        //[FieldOffset(0x64)] public fixed byte VoyageSectorData[0x38 * 5];

        //public Span<SubmersibleSectorData> VoyageSectors => new(Unsafe.AsPointer(ref VoyageSectorData[0]), 5);
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x38)]
    public unsafe partial struct SubmersibleSectorData
    {
        [FieldOffset(0x0)] public byte SectorId;
        [FieldOffset(0x1)] public byte ExperienceRating;
        [FieldOffset(0x2)] public byte DiscoveredSectorId;
        [FieldOffset(0x3)] public byte IsFirstTimeExplored;
        [FieldOffset(0x4)] public byte UnlockedSubmarine;
        [FieldOffset(0x5)] public byte IsDoubleDip;
        //2bytes
        [FieldOffset(0x8)] public uint FavorResult;
        [FieldOffset(0xC)] public uint Experience;
        [FieldOffset(0x10)] public fixed uint ItemId[2];
        [FieldOffset(0x18)] public fixed ushort Quantity[2];
        [FieldOffset(0x1C)] public fixed byte isHQ[2];
        [FieldOffset(0x1E)] public fixed byte isNotTier3[2];
        [FieldOffset(0x20)] public fixed uint SurveillanceResult[2];
        [FieldOffset(0x28)] public fixed uint RetrievalResult[2];
        [FieldOffset(0x30)] public fixed uint QualityResult[2];
    }
}
