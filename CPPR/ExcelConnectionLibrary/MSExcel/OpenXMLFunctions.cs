using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Data;
using System.Text.RegularExpressions;

namespace FileParsingLibrary.MSExcel;

public  class OpenXMLFunctions
{

    public static List<string> GetSheetNames(string strPath)
    {
        var names = new List<string>();

        using (var spreadSheetDocument = SpreadsheetDocument.Open(strPath, false))
        {
            int sheetIndex = 0;
            foreach (var worksheetpart in spreadSheetDocument.WorkbookPart.WorksheetParts)
            {
                string sheetName = spreadSheetDocument.WorkbookPart.Workbook.Descendants<Sheet>().ElementAt(sheetIndex).Name;
                names.Add(sheetName);

                sheetIndex++;
            }
        }

        return names;
    }

    public static string[] strCellsToIgnoreArr = null;
    public static SheetData sheetData = null;
    public static WorkbookPart workbookPart = null;
    public static DataTable ReadAsDataTable(SpreadsheetDocument spreadSheetDocument, string strSheetName, int intStartingColumnRow = 1, int intStartingDataRow = 2, int intStartingDataCell = 0, bool blSheetsWild = false, bool blNullColumns = false)
    {
        DataTable dataTable = new DataTable();

        workbookPart = spreadSheetDocument.WorkbookPart;
        DocumentFormat.OpenXml.Spreadsheet.Workbook workbook = workbookPart.Workbook;

        Sheet wsSheet;

        if (!blSheetsWild) //DEFAULT!!!
        {
            wsSheet = workbook.Descendants<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Where(sht => sht.Name == strSheetName).FirstOrDefault();
            if (wsSheet == null)
                wsSheet = workbook.Descendants<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Where(sht => sht.Name.ToString().ToLower().Equals(strSheetName.ToLower())).FirstOrDefault();
        }
        else
            wsSheet = workbook.Descendants<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Where(sht => sht.Name.ToString().ToLower().StartsWith(strSheetName.ToLower())).FirstOrDefault();

        //string relationshipId = sheets.First().Id.Value;
        string relationshipId = wsSheet.Id.Value;
        WorksheetPart worksheetPart = (WorksheetPart)spreadSheetDocument.WorkbookPart.GetPartById(relationshipId);
        Worksheet workSheet = worksheetPart.Worksheet;
        sheetData = workSheet.GetFirstChild<SheetData>();
        IEnumerable<Row> rows = sheetData.Descendants<Row>();

        int intNullCnt = 1;
        //foreach (Cell cell in rows.ElementAt(0))
        foreach (Cell cell in rows.ElementAt(intStartingColumnRow - 1))
        {
            var value = GetCellValue(spreadSheetDocument, cell);
            //if (String.IsNullOrEmpty(value))
            if (value == null && blNullColumns)
            {
                value = "Column" + intNullCnt;
                intNullCnt++;
            }

            if (value == null)
                continue;

            if (dataTable.Columns.Contains(value.Trim()))
                dataTable.Columns.Add(value.Trim() + "1");
            else
                dataTable.Columns.Add(value.Trim());
        }

        // skip the part that retrieves the worksheet sheetData
        int intcnt = 0;
        int cellCnt = 0;
        bool blAllBlank = true;
        Int16 intBlankCnt = 0;
        Int16 intBlankLimit = 10;
        foreach (Row row in rows)
        {

            if (intcnt < intStartingDataRow - 1)
            {
                intcnt++;
                continue;
            }

            DataRow dataRow = dataTable.NewRow();
            IEnumerable<Cell> cells = GetRowCells(row, intStartingDataCell);
            cellCnt = 0;
            blAllBlank = true;
            //foreach (Cell cell in cells)
            foreach (Cell cell in cells)
            {
                if (cellCnt >= dataTable.Columns.Count)
                    continue;

                // skip part that reads the text according to the cell-type
                var val = GetCellValue(spreadSheetDocument, cell);

                if (!string.IsNullOrEmpty(val))
                    blAllBlank = false;

                dataRow[cellCnt] = val;
                cellCnt++;
            }

            if (!blAllBlank)
            {
                dataTable.Rows.Add(dataRow);
                intBlankCnt = 0;
            }
            else
                intBlankCnt++;


            if (intBlankCnt == intBlankLimit)
            {
                break;
            }

        }
        return dataTable;
    }

    private static IEnumerable<Cell> GetRowCells(Row row, int intStartingDataCell = 0)
    {
        int currentCount = intStartingDataCell;

        foreach (DocumentFormat.OpenXml.Spreadsheet.Cell cell in
            row.Descendants<DocumentFormat.OpenXml.Spreadsheet.Cell>())
        {

            string columnName = GetColumnName(cell.CellReference);

            //MERGED CHECKER
            if (strCellsToIgnoreArr != null)
            {
                //MERGED SO IGNORE THESE
                if (strCellsToIgnoreArr.Contains(columnName))
                {
                    currentCount++;
                    continue;
                }

            }

            int currentColumnIndex = ConvertColumnNameToNumber(columnName);

            for (; currentCount < currentColumnIndex; currentCount++)
            {
                yield return new DocumentFormat.OpenXml.Spreadsheet.Cell();
            }

            yield return cell;
            currentCount++;
        }
    }


    private static string GetColumnName(string cellReference)
    {
        // Match the column name portion of the cell name.
        var regex = new System.Text.RegularExpressions.Regex("[A-Za-z]+");
        var match = regex.Match(cellReference);

        return match.Value;
    }

    private static int ConvertColumnNameToNumber(string columnName)
    {
        var alpha = new Regex("^[A-Z]+$");
        if (!alpha.IsMatch(columnName)) throw new ArgumentException();

        char[] colLetters = columnName.ToCharArray();
        Array.Reverse(colLetters);

        int convertedValue = 0;
        for (int i = 0; i < colLetters.Length; i++)
        {
            char letter = colLetters[i];
            int current = i == 0 ? letter - 65 : letter - 64; // ASCII 'A' = 65
            convertedValue += current * (int)Math.Pow(26, i);
        }

        return convertedValue;
    }

    private static string GetCellValue(SpreadsheetDocument document, Cell cell)
    {
        SharedStringTablePart stringTablePart = document.WorkbookPart.SharedStringTablePart;
        string value = (cell.CellValue != null ? cell.CellValue.InnerXml : null);

        if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
        {
            return stringTablePart.SharedStringTable.ChildElements[Int32.Parse(value)].InnerText;
        }
        else
        {
            return value;
        }
    }
}
