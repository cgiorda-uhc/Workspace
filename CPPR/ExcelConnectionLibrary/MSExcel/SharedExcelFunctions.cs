using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace FileParsingLibrary.MSExcel
{
    public static class SharedExcelFunctions
    {
        public static string GetColumnName(int index)
        {
            const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            var value = "";

            if (index >= letters.Length)
                value += letters[index / letters.Length - 1];

            value += letters[index % letters.Length];

            return value;
        }

        public static void AddClosedXMLBorders(ref IXLCell cell)
        {
            cell.Style.Border.RightBorder = XLBorderStyleValues.Thick;
            cell.Style.Border.RightBorderColor = XLColor.Black;

            cell.Style.Border.LeftBorder = XLBorderStyleValues.Thick;
            cell.Style.Border.LeftBorderColor = XLColor.Black;

            cell.Style.Border.TopBorder = XLBorderStyleValues.Thick;
            cell.Style.Border.TopBorderColor = XLColor.Black;

            cell.Style.Border.BottomBorder = XLBorderStyleValues.Thick;
            cell.Style.Border.BottomBorderColor = XLColor.Black;
        }

    }
}
