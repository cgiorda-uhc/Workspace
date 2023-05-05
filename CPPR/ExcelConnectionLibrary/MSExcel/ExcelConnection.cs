using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Office.Interop.Excel;
using System.Data;
using System.Data.OleDb;
using System.Text.RegularExpressions;
using DataTable = System.Data.DataTable;
using Sheets = DocumentFormat.OpenXml.Spreadsheet.Sheets;
using Worksheet = DocumentFormat.OpenXml.Spreadsheet.Worksheet;

namespace FileParsingLibrary.MSExcel
{
    public class ExcelConnection
    {
        //MERGED SO IGNORE THESE IF NOT NULL
        //SEEMS LIKE HACK, FIND BETTER WAY!!!
        public static string[]? strCellsToIgnoreArr = null;

        public static SheetData? sheetData = null;
        public static WorkbookPart? workbookPart = null;

        public static DataTable ReadAsDataTable(SpreadsheetDocument spreadSheetDocument, string strSheetName, int intStartingColumnRow = 1, int intStartingDataRow = 2, int intStartingDataCell = 0, bool blSheetsWild = false)
        {
            DataTable dataTable = new DataTable();

            workbookPart = spreadSheetDocument.WorkbookPart;
            // IEnumerable<Sheet> sheets = spreadSheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>();

            //WorkbookPart bkPart = spreadSheetDocument.WorkbookPart;
            DocumentFormat.OpenXml.Spreadsheet.Workbook? workbook = workbookPart.Workbook;

            Sheet wsSheet;

            if (!blSheetsWild) //DEFAULT!!!
                wsSheet = workbook.Descendants<Sheet>().Where(sht => sht.Name == strSheetName).FirstOrDefault();
            else
                wsSheet = workbook.Descendants<Sheet>().Where(sht => sht.Name.ToString().ToLower().StartsWith(strSheetName.ToLower())).FirstOrDefault();


            //string relationshipId = sheets.First().Id.Value;
            string relationshipId = wsSheet.Id.Value;
            WorksheetPart worksheetPart = (WorksheetPart)spreadSheetDocument.WorkbookPart.GetPartById(relationshipId);
            Worksheet workSheet = worksheetPart.Worksheet;
            sheetData = workSheet.GetFirstChild<SheetData>();
            IEnumerable<Row> rows = sheetData.Descendants<Row>();




            //foreach (Cell cell in rows.ElementAt(0))
            foreach (Cell cell in rows.ElementAt(intStartingColumnRow - 1))
            {
                var value = GetCellValue(spreadSheetDocument, cell);
                //if (String.IsNullOrEmpty(value))
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
            short intBlankCnt = 0;
            short intBlankLimit = 10;
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


        public static Sheets GetAllWorksheets(string fileName)
        {
            Sheets theSheets = null;

            using (SpreadsheetDocument document =
                SpreadsheetDocument.Open(fileName, false))
            {
                WorkbookPart wbPart = document.WorkbookPart;
                theSheets = wbPart.Workbook.Sheets;
            }
            return theSheets;
        }

        private static IEnumerable<Cell> GetRowCells(Row row, int intStartingDataCell = 0)
        {
            int currentCount = intStartingDataCell;

            foreach (Cell cell in
                row.Descendants<Cell>())
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
                    yield return new Cell();
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
            var regex = new Regex("[A-Za-z]+");
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
            string value = cell.CellValue != null ? cell.CellValue.InnerXml : null;

            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                return stringTablePart.SharedStringTable.ChildElements[int.Parse(value)].InnerText;
            }
            else
            {
                return value;
            }
        }




        public static Cell GetCell(SheetData sheetData, string cellAddress)
        {
            uint rowIndex = uint.Parse(Regex.Match(cellAddress, @"[0-9]+").Value);
            return sheetData.Descendants<Row>().FirstOrDefault(p => p.RowIndex == rowIndex).Descendants<Cell>().FirstOrDefault(p => p.CellReference == cellAddress);
        }

        public static string GetCellValue(Cell cell, WorkbookPart wbPart)
        {
            string value = cell.InnerText;
            if (cell.DataType != null)
            {
                switch (cell.DataType.Value)
                {
                    case CellValues.SharedString:
                        var stringTable = wbPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
                        if (stringTable != null)
                        {
                            value = stringTable.SharedStringTable.ElementAt(int.Parse(value)).InnerText;
                        }
                        break;

                    case CellValues.Boolean:
                        switch (value)
                        {
                            case "0":
                                value = "FALSE";
                                break;
                            default:
                                value = "TRUE";
                                break;
                        }
                        break;
                }
            }
            return value;
        }



        public static void ConvertFromXLSBToXLSX(string filepath, string filepathFinal)
        {

            try
            {
                Application excelApplication = new Application();
                excelApplication.DisplayAlerts = false;
                Workbooks workbooks = excelApplication.Workbooks;
                // open book in any format
                Microsoft.Office.Interop.Excel.Workbook workbook = workbooks.Open(filepath, XlUpdateLinks.xlUpdateLinksNever, true, Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                // save in XlFileFormat.xlExcel12 format which is XLSB
                workbook.SaveAs(filepathFinal, XlFileFormat.xlOpenXMLWorkbook, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                    XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                // close workbook
                workbook.Close(false, Type.Missing, Type.Missing);

                excelApplication.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(workbooks);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApplication);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                //foreach (System.Diagnostics.Process proc in System.Diagnostics.Process.GetProcessesByName("EXCEL"))
                //{
                //    proc.Kill();
                //}
            }

        }



        private static void fixCorrupedXLSX(string filepath)
        {

            try
            {
                Application excelApplication = new Application();
                //Missing missing = Missing.Value;
                excelApplication.DisplayAlerts = false;

                Microsoft.Office.Interop.Excel.Workbook workbook = excelApplication.Workbooks.Open(filepath, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, XlCorruptLoad.xlRepairFile);
                workbook.SaveAs(filepath, XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing, Type.Missing, Type.Missing, XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                workbook.Close(true, Type.Missing, Type.Missing);



                excelApplication.DisplayAlerts = true;
                excelApplication.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApplication);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                //foreach (System.Diagnostics.Process proc in System.Diagnostics.Process.GetProcessesByName("EXCEL"))
                //{
                //    proc.Kill();
                //}
            }

        }


        public static OleDbDataReader ConvertExcelToDataReader(string FileName, string strSheetname, string strStart = "", string strWhere = "", string strColumns = "*")
        {

            //int totalSheet = 0; //No of sheets on excel file  

            //NEED ACE HEADER ROW FOR WHERE CLAUSE
            string strHDR = "NO";
            if (strWhere != "" || strColumns != "*")
                strHDR = "YES";

            OleDbConnection objConn = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.16.0;Data Source=" + FileName + ";Extended Properties='Excel 12.0;HDR=YES;IMEX=1;';");


            objConn.Open();
            OleDbCommand cmd = new OleDbCommand();
            OleDbDataAdapter oleda = new OleDbDataAdapter();
            OleDbDataReader oleDr = null;
            cmd.Connection = objConn;
            cmd.CommandType = CommandType.Text;
            //cmd.CommandText = "SELECT * FROM [" + strSheetname + "$]";
            cmd.CommandText = "SELECT " + strColumns + " FROM [" + strSheetname + "$" + strStart + "]" + strWhere;
            cmd.CommandTimeout = 99999999;
            oleda = new OleDbDataAdapter(cmd);
            try
            {
                cmd.CommandTimeout = 9999999;
                oleDr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return oleDr;

        }

        public static DataTable ConvertExcelToDataTable(string FileName, string strSheetname, string strStart = "", string strWhere = "", string strColumns = "*")
        {
            DataTable dtResult = null;
            //int totalSheet = 0; //No of sheets on excel file  

            //NEED ACE HEADER ROW FOR WHERE CLAUSE
            string strHDR = "NO";
            if (strWhere != "" || strColumns != "*")
                strHDR = "YES";

            using (OleDbConnection objConn = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.16.0;Data Source=" + FileName + ";Extended Properties='Excel 12.0;HDR=" + strHDR + ";IMEX=1;';"))
            {

                objConn.Open();
                OleDbCommand cmd = new OleDbCommand();
                OleDbDataAdapter oleda = new OleDbDataAdapter();
                DataSet ds = new DataSet();
                DataTable dt = objConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                cmd.Connection = objConn;
                cmd.CommandType = CommandType.Text;
                //cmd.CommandText = "SELECT * FROM [" + strSheetname + "$]";
                cmd.CommandText = "SELECT " + strColumns + " FROM [" + strSheetname + "$" + strStart + "]" + strWhere;
                cmd.CommandTimeout = 99999999;
                oleda = new OleDbDataAdapter(cmd);

                oleda.Fill(ds, "excelData");
                dtResult = ds.Tables["excelData"];
                objConn.Close();

                //MUST CREATE OUR OWN HEADERS!!!
                if (strWhere == "" && strColumns == "*")
                {
                    //THIS MAKES ALL TYPES TO STRING SO NO NULLS FOR MISMATCHES!!!
                    //SINCE WE ADDED  HDR=NO TO HEADER WE MUST MANUALLY NAME EACH COLUMN WITH ROW 1
                    foreach (DataColumn column in dtResult.Columns)
                    {
                        string cName = dtResult.Rows[0][column.ColumnName].ToString();
                        if (!dtResult.Columns.Contains(cName) && cName != "")
                        {
                            column.ColumnName = cName;
                        }
                    }
                    dtResult.Rows[0].Delete();
                    dtResult.AcceptChanges();
                }

                //Microsoft.ACE.OLEDB WONT IGNORE EMPTY ROWS
                //BUT THIS WILL!!!
                dtResult = dtResult.Rows.Cast<DataRow>().Where(row => !row.ItemArray.All(field => field is DBNull || string.IsNullOrWhiteSpace(field as string))).CopyToDataTable();


                return dtResult; //Returning Dattable  
            }
        }
    }
}