using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace UCS_Data_Cleaner
{
    public class OpenXML
    {


        public static DataTable ReadAsDataTable(SpreadsheetDocument spreadSheetDocument, string strSheetName, int intStartingColumnRow = 1, int intStartingDataRow = 2)
        {
            DataTable dataTable = new DataTable();
           
            WorkbookPart workbookPart = spreadSheetDocument.WorkbookPart;
           // IEnumerable<Sheet> sheets = spreadSheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>();


            //WorkbookPart bkPart = spreadSheetDocument.WorkbookPart;
            DocumentFormat.OpenXml.Spreadsheet.Workbook workbook = workbookPart.Workbook;
            Sheet wsSheet = workbook.Descendants<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Where(sht => sht.Name == strSheetName).FirstOrDefault();


            //string relationshipId = sheets.First().Id.Value;
            string relationshipId = wsSheet.Id.Value;
            WorksheetPart worksheetPart = (WorksheetPart)spreadSheetDocument.WorkbookPart.GetPartById(relationshipId);
            Worksheet workSheet = worksheetPart.Worksheet;
            SheetData sheetData = workSheet.GetFirstChild<SheetData>();
            IEnumerable<Row> rows = sheetData.Descendants<Row>();

            //foreach (Cell cell in rows.ElementAt(0))
            foreach (Cell cell in rows.ElementAt(intStartingColumnRow - 1))
            {
                dataTable.Columns.Add(GetCellValue(spreadSheetDocument, cell));
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
                IEnumerable<Cell> cells = GetRowCells(row);
                cellCnt = 0;
                blAllBlank = true;
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


                if(intBlankCnt == intBlankLimit)
                {
                    break;
                }
                    
            }

           
            //dataTable.Rows.RemoveAt(0);

            return dataTable;
        }



        
        private static IEnumerable<Cell> GetRowCells(Row row)
        {
            int currentCount = 0;

            foreach (DocumentFormat.OpenXml.Spreadsheet.Cell cell in
                row.Descendants<DocumentFormat.OpenXml.Spreadsheet.Cell>())
            {
                string columnName = GetColumnName(cell.CellReference);

                int currentColumnIndex = ConvertColumnNameToNumber(columnName);

                for (; currentCount < currentColumnIndex; currentCount++)
                {
                    yield return new DocumentFormat.OpenXml.Spreadsheet.Cell();
                }

                yield return cell;
                currentCount++;
            }
        }

        /// <summary>
        /// Given a cell name, parses the specified cell to get the column name.
        /// </summary>
        /// <param name="cellReference">Address of the cell (ie. B2)</param>
        /// <returns>Column Name (ie. B)</returns>
        private static string GetColumnName(string cellReference)
        {
            // Match the column name portion of the cell name.
            var regex = new System.Text.RegularExpressions.Regex("[A-Za-z]+");
            var match = regex.Match(cellReference);

            return match.Value;
        }

        /// <summary>
        /// Given just the column name (no row index),
        /// it will return the zero based column index.
        /// </summary>
        /// <param name="columnName">Column Name (ie. A or AB)</param>
        /// <returns>Zero based index if the conversion was successful</returns>
        /// <exception cref="ArgumentException">thrown if the given string
        /// contains characters other than uppercase letters</exception>
        private static int ConvertColumnNameToNumber(string columnName)
        {
            var alpha = new System.Text.RegularExpressions.Regex("^[A-Z]+$");
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
            string value = (cell.CellValue != null ? cell.CellValue.InnerXml: null);

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
}
