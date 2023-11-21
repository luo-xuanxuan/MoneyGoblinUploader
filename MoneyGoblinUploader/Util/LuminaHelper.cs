using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyGoblinUploader.Util
{
    public class LuminaHelper
    {

        Lumina.GameData gameData;
        public LuminaHelper()
        {
            gameData = new Lumina.GameData("C:/Program Files (x86)/SquareEnix/FINAL FANTASY XIV - A Realm Reborn/game/sqpack");
            gameData.Options.PanicOnSheetChecksumMismatch = false;
        }

        public string getItemName(int id)
        {
            var itemSheet = gameData.GetExcelSheet<Item>();
            var itemRow = itemSheet.GetRow((uint)id);
            return itemRow.Name;
        }

        public (ushort Surveillance, ushort Retrieval, ushort Speed, ushort Range, ushort Favor) getStatsByRank(int rank)
        {
            var rankSheet = gameData.GetExcelSheet<SubmarineRank>();
            var rankRow = rankSheet.GetRow((uint) rank);
            return (Surveillance: rankRow.SurveillanceBonus,
                    Retrieval: rankRow.RetrievalBonus,
                    Speed: rankRow.SpeedBonus,
                    Range: rankRow.RangeBonus,
                    Favor: rankRow.FavorBonus);
        }
    }
}
