using ClosedXML.Excel;
using ClosedXML.Graphics;
using DocumentFormat.OpenXml.Office2019.Excel.RichData2;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using FileParsingLibrary.Models;
using Microsoft.Office.Interop.Excel;
using NPOI.OpenXmlFormats.Dml.Diagram;
using NPOI.SS.Formula.Functions;
using SixLabors.Fonts;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;

namespace FileParsingLibrary.MSExcel
{
    public class ClosedXMLFunctions : IExcelFunctions
    {
        public List<KeyValuePair<string, string>> Mappings { get; set; }

        public List<T> ImportExcel<T>(string fileName, string sheetName, string columnHeaderRange, int startingRow, string nullCheck = null)
        {
            int rowIndex = 1;

            //STARTS AT 1 SO DETERMING PADDING BASED ON STARTING COLUMN A=0 B=1 C=2
            int columnPadding = ExcelColumnNameToNumber(columnHeaderRange.Substring(0, 1)) - 1;

            List<T> list = new List<T>();

            Type typeOfObject = typeof(T);
            using (IXLWorkbook workbook = new XLWorkbook(fileName))
            {
                var worksheet = workbook.Worksheets.Where(w => w.Name.ToLower().Trim() == sheetName.ToLower().Trim()).FirstOrDefault();
                var properties = typeOfObject.GetProperties();
                //HEADER COLUMN TEXTS
                var columns = worksheet.Range(columnHeaderRange).Cells().Select((v, i) => new { v.Value, Index = i + 1 });//INDEXING CLOSESXML IS 1 NOT 0!!!
                foreach (IXLRow row in worksheet.Rows())//LOOP ROWS
                {
                    T obj = (T)Activator.CreateInstance(typeOfObject);

                    //GET STARTING ROW DEFINE IN startingRow
                    if (row.RangeAddress.FirstAddress.RowNumber < startingRow)
                    {
                        continue;
                    }
                    else
                    {
                        rowIndex = row.RangeAddress.FirstAddress.RowNumber;
                    }

                    //LOOP THROUGH MODEL PROPS
                    foreach (var prop in properties)
                    {
                        //FIND MAPPING FOR THE PROP
                        // var mapping = Mappings.SingleOrDefault(m => m.Value.ToLower().Trim() == prop.Name.ToLower().Trim());
                        var colName = "";
                        if (Mappings == null)
                        {
                            colName = prop.Name;
                        }
                        else
                        {
                            var mapping = Mappings.Where(m => m.Value.ToLower().Trim() == prop.Name.ToLower().Trim());

                            if (mapping.Count() > 1)
                            {
                                foreach (var m in mapping)
                                {
                                    var c = columns.SingleOrDefault(c => c.Value.ToString().ToLower().Trim() == m.Key.ToLower().Trim());
                                    if (c != null)
                                    {
                                        colName = c.Value.ToString();
                                        break;
                                    }
                                }

                            }
                            else
                            {
                                colName = mapping.FirstOrDefault().Key;
                            }
                        }





                        //IF NO MAPPING FOUND JUST ASSUME MATCH
                        if (colName == null)
                        {
                            colName = prop.Name;
                        }
                        //SEARCH MATCHING EXCEL COLUMNS
                        var col = columns.SingleOrDefault(c => c.Value.ToString().ToLower().Trim() == colName.ToLower().Trim());
                        if (col == null)
                        {
                            continue;
                        }

                        //var colIndex = col.Index + 1;
                        var colIndex = col.Index + columnPadding;
                        XLCellValue value;
                        try//CHECK FOR BAD FORMULAS
                        {
                            value = row.Cell(colIndex).Value;
                        }
                        catch
                        {
                            value = "";
                        }
                        var propType = prop.PropertyType;

                        if (propType == typeof(DateTime))
                        {
                            DateTime temp;
                            if (DateTime.TryParse(value.ToString(), out temp)) //CLEANS EXCEL DATE Ex. 44556
                            {
                                value = temp.ToShortDateString();
                            }
                            else
                            {
                                value = "";
                            }
                        }
                        else if (propType == typeof(Boolean))
                        {
                            Boolean temp;
                            if (Boolean.TryParse(value.ToString(), out temp)) //CLEANS EXCEL DATE Ex. 44556
                            {
                                value = temp;
                            }
                            else
                            {
                                if (value.ToString().ToLower().Trim() == "n")
                                    value = false.ToString();
                                else if (value.ToString().ToLower().Trim() == "y")
                                    value = true.ToString();
                                else
                                    value = "";
                            }
                        }
                        else if (propType == typeof(TimeSpan))
                        {
                            TimeSpan temp;
                            if (TimeSpan.TryParse(value.ToString(), out temp)) //CLEANS EXCEL DATE Ex. 44556
                            {
                                value = temp.ToString();
                            }
                            else
                            {
                                value = "";
                            }
                        }
                        else if (propType == typeof(Decimal))
                        {
                            Decimal temp;
                            if (Decimal.TryParse(value.ToString(), out temp)) //CLEANS EXCEL DATE Ex. 44556
                            {
                                value = temp.ToString();
                            }
                            else
                            {
                                value = "";
                            }
                        }
                        else if (propType == typeof(Double))
                        {
                            Double temp;
                            if (Double.TryParse(value.ToString(), out temp)) //CLEANS EXCEL DATE Ex. 44556
                            {
                                value = temp.ToString();
                            }
                            else
                            {
                                value = "";
                            }
                        }
                        else if (propType == typeof(Single))
                        {
                            Single temp;
                            if (Single.TryParse(value.ToString(), out temp)) //CLEANS EXCEL DATE Ex. 44556
                            {
                                value = temp.ToString();
                            }
                            else
                            {
                                value = "";
                            }
                        }
                        else if (propType == typeof(Int32))
                        {
                            Int32 temp;
                            if (Int32.TryParse(value.ToString(), out temp)) //CLEANS EXCEL DATE Ex. 44556
                            {
                                value = temp.ToString();
                            }
                            else
                            {
                                value = "";
                            }
                        }
                        else if (propType == typeof(Int64))
                        {
                            Int64 temp;
                            if (Int64.TryParse(value.ToString(), out temp)) //CLEANS EXCEL DATE Ex. 44556
                            {
                                value = temp.ToString();
                            }
                            else
                            {
                                value = "";
                            }
                        }
                        else if (propType == typeof(Int16))
                        {
                            Int16 temp;
                            if (Int16.TryParse(value.ToString(), out temp)) //CLEANS EXCEL DATE Ex. 44556
                            {
                                value = temp.ToString();
                            }
                            else
                            {
                                value = "";
                            }
                        }
                        else if (propType == typeof(Byte))
                        {
                            Byte temp;
                            if (Byte.TryParse(value.ToString(), out temp)) //CLEANS EXCEL DATE Ex. 44556
                            {
                                value = temp.ToString();
                            }
                            else
                            {
                                value = "";
                            }
                        }

                        //CHECK FOR END OF ENDLESS FILES
                        if (nullCheck != null)
                        {
                            //if (colName == nullCheck && rowIndex > 30000)
                            //{
                            //    return list;
                            //}




                            if (colName == nullCheck && string.IsNullOrEmpty(value.ToString()))
                            {
                                return list;
                            }
                        }

                        try
                        {

                            if (value.ToString() == "")
                            {
                                prop.SetValue(obj, null);
                            }
                            else
                            {

                                var converter = TypeDescriptor.GetConverter(propType);
                                var val = converter.ConvertFrom(value.ToString());
                                prop.SetValue(obj, val);
                            }

                        }
                        catch (FormatException)
                        {
                            //EXCEL PUT 'NA' IN DOUBLE FIELD LETS NULL IT
                        }




                    }
                    //rowIndex++;
                    //if(list.Count == 59)
                    //{
                    //    var s = "";
                    //}

                    list.Add(obj);

                }
            }

            return list;
        }

        public byte[] ExportToExcel<T>(List<T> list, string worksheetTitle, List<string[]> columns)
        {
            //List<string> s = new List<string>();
            //foreach (var fontFamily in SixLabors.Fonts.SystemFonts.Collection.Families)
            //    s.Add(fontFamily.Name);

            //Load the font(s)


            //var assembly = Assembly.GetExecutingAssembly();
            //Gets the names of the Embeded Resources (handy to detect full path of the embeded files)
            //var resources = assembly.GetManifestResourceNames();
            //foreach (var resource in resources) Console.WriteLine(resource);

            //Create stream to inject to Graphics engine
            //using Stream fontStream = assembly.GetManifestResourceStream($"yourResourcePath")
            //?? throw new Exception("Resource not Found");
            ////Load the font(s)
            //LoadOptions.DefaultGraphicEngine = new DefaultGraphicEngine(fontStream);

            // var assembly = Assembly.GetExecutingAssembly();
            // var path = "Fonts/calibri.ttf";
            // using Stream stream = assembly.GetManifestResourceStream(path); 
            // LoadOptions.DefaultGraphicEngine = new DefaultGraphicEngine.CreateWithFontsAndSystemFonts(stream);

            // // or also 
            // //FontCollection fonts = new();
            // //...
            // //Create any fonts needed by the app as per [SixLabors Docs](https://docs.sixlabors.com/articles/fonts/gettingstarted.html)
            // //...
            //// LoadOptions.DefaultGraphicEngine = new DefaultGraphicEngine(fonts);
            ///


            //var assembly = Assembly.GetExecutingAssembly();

            ////Gets the names of the Embeded Resources (handy to detect full path of the embeded files)
            //var resources = assembly.GetManifestResourceNames();
            //foreach (var resource in resources)
            //{
            //    //using Stream fontStream = assembly.GetManifestResourceStream(resource) ?? throw new Exception("Resource not Found");

            //    //Loads the font and generates a new default Graphics Engine
            //    //LoadOptions.DefaultGraphicEngine = new DefaultGraphicEngine.CreateOnlyWithFonts(fontStream);
            //}


            using (var fallbackFontStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("FileParsingLibrary.Fonts.calibri.ttf"))
            {
                LoadOptions.DefaultGraphicEngine = DefaultGraphicEngine.CreateWithFontsAndSystemFonts(fallbackFontStream);
            }


            //using (var fallbackFontStream = File.OpenRead("//WP000003507/csg_share/Fonts/calibri.ttf"))
            //{
            //    LoadOptions.DefaultGraphicEngine = DefaultGraphicEngine.CreateOnlyWithFonts(fallbackFontStream);
            //}




            //DefaultGraphicEngine.CreateOnlyWithFonts(new MemoryStream(File.ReadAllBytes("//WP000003507/csg_share/Fonts/calibri.ttf")));


            //using (var fallbackFontStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Fonts/calibri.ttf"))
            //{
            //    LoadOptions.DefaultGraphicEngine = DefaultGraphicEngine.CreateWithFontsAndSystemFonts(fallbackFontStream);
            //}




            using var wb = new XLWorkbook(); //create workbook
            var ws = wb.Worksheets.Add(worksheetTitle); //add worksheet to workbook

            var rangeTitle = ws.Cell(1, 1).InsertData(columns); //insert titles to first row
            rangeTitle.AddToNamed("Titles");
            var titlesStyle = wb.Style;
            titlesStyle.Font.Bold = true; //font must be bold
            titlesStyle.Fill.BackgroundColor = XLColor.Yellow;
            titlesStyle.Border.OutsideBorder = XLBorderStyleValues.Thin;
            titlesStyle.Border.BottomBorder = XLBorderStyleValues.Thin;
            titlesStyle.Border.TopBorder = XLBorderStyleValues.Thin;
            titlesStyle.Border.LeftBorder = XLBorderStyleValues.Thin;
            titlesStyle.Border.RightBorder = XLBorderStyleValues.Thin;
            // titlesStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center; //align text to center


            wb.NamedRanges.NamedRange("Titles").Ranges.Style = titlesStyle; //attach style to the range

            if (list != null && list.Count() > 0)
            {
                //insert data to from second row on
                ws.Cell(2, 1).InsertData(list);
                // ws.Columns().AdjustToContents();
            }

            ws.Columns().AdjustToContents(1, 100);

            //save file to memory stream and return it as byte array
            using (var ms = new MemoryStream())
            {
                wb.SaveAs(ms);

                return ms.ToArray();
            }
        }

        public async Task<byte[]> ExportToExcelAsync<T>(List<T> list, string worksheetTitle,  Func<string> getterStatus, Action<string> setterStatus)
        {
            StringBuilder sbStatus = new StringBuilder();
            sbStatus.Append("--Exporting to Excel..." + Environment.NewLine);
            sbStatus.Append("-------------------------------------------" + Environment.NewLine);
            byte[] final = new byte[0];
            Task t = Task.Run(async () =>
            {
                using var wb = new XLWorkbook(); //create workbook
                int rowcnt = 1;
                int colcnt = 1;
                int totalcnt = 0;
                
                sbStatus.Append("--Generating Sheet: " + worksheetTitle + "..." + Environment.NewLine);
                setterStatus(sbStatus.ToString());
                var ws = wb.Worksheets.Add(worksheetTitle); //add worksheet to workbook
                var type = list!.FirstOrDefault()!.GetType();

                PropertyInfo[] properties = type.GetProperties();

                colcnt = 1;
                foreach (var prop in properties)
                {
                    ws.Cell(1, colcnt).Value = prop.Name;
                    ws.Cell(1, colcnt).Style.Font.Bold = true; //font must be bold
                    ws.Cell(1, colcnt).Style.Fill.BackgroundColor = XLColor.Yellow;
                    ws.Cell(1, colcnt).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    ws.Cell(1, colcnt).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell(1, colcnt).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell(1, colcnt).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell(1, colcnt).Style.Border.RightBorder = XLBorderStyleValues.Thin;

                    colcnt++;
                }

                totalcnt = list.Count();
                if (list != null && totalcnt > 0)
                {
                    rowcnt = 2;
                    foreach (var item in list)
                    {
                        setterStatus(sbStatus.ToString() + "--Adding " + rowcnt.ToString() + " out of " + totalcnt + " rows..." + Environment.NewLine);
                        colcnt = 1;
                        foreach (var prop in properties)
                        {
                            ws.Cell(rowcnt, colcnt).Value = prop.GetValue(item, null) + "";
                            colcnt++;
                        }
                        rowcnt++;
                    }


                    sbStatus.Clear();
                    sbStatus.Append(getterStatus());
                    sbStatus.Append("--" + worksheetTitle + " has beeen generated." + Environment.NewLine);
                    sbStatus.Append("-------------------------------------------" + Environment.NewLine);
                    setterStatus(sbStatus.ToString());
                }

                ws.Columns().AdjustToContents(1, 20);
               

                sbStatus.Append(Environment.NewLine + "Opening spreadsheet...");
                setterStatus(sbStatus.ToString());

                //save file to memory stream and return it as byte array
                using (var ms = new MemoryStream())
                {
                    wb.SaveAs(ms);

                    final = ms.ToArray();
                }

            });
            t.Wait(); // Wait until the above task is complete, email is sent
            return final;
        }

       public async Task<byte[]> ExportToExcelAsync(List<ExcelExport> excelExports)
        {
            byte[] final = new byte[0];
            Task t = Task.Run(async () =>
            {
                using var wb = new XLWorkbook(); //create workbook
                int rowcnt = 1;
                int colcnt = 1;
                foreach (var ex in excelExports)
                {
                    var ws = wb.Worksheets.Add(ex.SheetName); //add worksheet to workbook
                    var type = ex.ExportList!.FirstOrDefault()!.GetType();

                    PropertyInfo[] properties = type.GetProperties();

                    colcnt = 1;
                    foreach (var prop in properties)
                    {
                        ws.Cell(1, colcnt).Value = prop.Name;
                        ws.Cell(1, colcnt).Style.Font.Bold = true; //font must be bold
                        ws.Cell(1, colcnt).Style.Fill.BackgroundColor = XLColor.Yellow;
                        ws.Cell(1, colcnt).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(1, colcnt).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell(1, colcnt).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell(1, colcnt).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell(1, colcnt).Style.Border.RightBorder = XLBorderStyleValues.Thin;

                        colcnt++;
                    }

                    if (ex.ExportList != null && ex.ExportList.Count() > 0)
                    {
                        rowcnt = 2;
                        foreach (var item in ex.ExportList)
                        {
                            colcnt = 1;
                            foreach (var prop in properties)
                            {
                                ws.Cell(rowcnt, colcnt).Value = prop.GetValue(item, null) + "";
                                colcnt++;
                            }
                            rowcnt++;
                        }
                    }

                    ws.Columns().AdjustToContents(1, 100);
                }

                //save file to memory stream and return it as byte array
                using (var ms = new MemoryStream())
                {
                    wb.SaveAs(ms);

                    final = ms.ToArray();
                }

            });
            t.Wait(); // Wait until the above task is complete, email is sent
            return final;
        }



        public async Task<byte[]> ExportToExcelAsync(List<ExcelExport> excelExports, Func<string> getterStatus, Action<string> setterStatus)
        {
            StringBuilder sbStatus = new StringBuilder();
            sbStatus.Append("--Exporting to Excel..." + Environment.NewLine);
            sbStatus.Append("-------------------------------------------" + Environment.NewLine);
            byte[] final = new byte[0];
            Task t = Task.Run(async () =>
            {
                using var wb = new XLWorkbook(); //create workbook
                int rowcnt = 1;
                int colcnt = 1;
                int totalcnt = 0;
                foreach (var ex in excelExports)
                {
                    sbStatus.Append("--Generating Sheet: " + ex.SheetName + "..." +Environment.NewLine);
                    setterStatus(sbStatus.ToString());
                    var ws = wb.Worksheets.Add(ex.SheetName); //add worksheet to workbook
                    var type = ex.ExportList!.FirstOrDefault()!.GetType();

                    PropertyInfo[] properties = type.GetProperties();

                    colcnt = 1;
                    foreach (var prop in properties)
                    {
                        ws.Cell(1, colcnt).Value = prop.Name;
                        ws.Cell(1, colcnt).Style.Font.Bold = true; //font must be bold
                        ws.Cell(1, colcnt).Style.Fill.BackgroundColor = XLColor.Yellow;
                        ws.Cell(1, colcnt).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(1, colcnt).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell(1, colcnt).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell(1, colcnt).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell(1, colcnt).Style.Border.RightBorder = XLBorderStyleValues.Thin;

                        colcnt++;
                    }

                    totalcnt = ex.ExportList.Count();
                    if (ex.ExportList != null && totalcnt > 0)
                    {
                        rowcnt = 2;
                        foreach (var item in ex.ExportList)
                        {
                            setterStatus(sbStatus.ToString() + "--Adding " + (rowcnt - 1).ToString("N0") + " out of "+ totalcnt.ToString("N0") + " rows..." + Environment.NewLine);
                            colcnt = 1;
                            foreach (var prop in properties)
                            {
                                ws.Cell(rowcnt, colcnt).Value = prop.GetValue(item, null) + "";
                                colcnt++;
                            }
                            rowcnt++;
                        }

                        sbStatus.Clear();
                        sbStatus.Append(getterStatus());
                        sbStatus.Append("--" + ex.SheetName + " has beeen generated." + Environment.NewLine);
                        sbStatus.Append("-------------------------------------------" + Environment.NewLine);
                        setterStatus(sbStatus.ToString());
                    }

                    ws.Columns().AdjustToContents(1, 20);
                }
                sbStatus.Append( "--Opening spreadsheet..." + Environment.NewLine + Environment.NewLine);
                setterStatus(sbStatus.ToString());

                //save file to memory stream and return it as byte array
                using (var ms = new MemoryStream())
                {
                    wb.SaveAs(ms);

                    final = ms.ToArray();
                }

            });
            t.Wait(); // Wait until the above task is complete, email is sent
            return final;
        }




        public async Task<byte[]> ExportToExcelTemplateAsync(string templateNamePath, List<ExcelExport> excelExports)
        {
            //StringBuilder sbStatus = new StringBuilder();
            //sbStatus.Append("--Exporting to Excel..." + Environment.NewLine);
            //sbStatus.Append("-------------------------------------------" + Environment.NewLine);
            byte[] final = new byte[0];
            Task t = Task.Run(async () =>
            {
                using var wb = new XLWorkbook(templateNamePath); //create workbook
                int rowcnt = 1;
                int currentCol = 1;
                int totalcnt = 0;
                foreach (var ex in excelExports)
                {
                    //if(ex.SheetName == "LOB")
                    //{
                    //    var s = "";
                    //}
                    
                    
                    
                    //sbStatus.Append("--Generating Sheet: " + ex.SheetName + "..." + Environment.NewLine);
                    //setterStatus(sbStatus.ToString());
                    var ws = wb.Worksheet(ex.SheetName); //add worksheet to workbook
                    var type = ex.ExportList!.FirstOrDefault()!.GetType();

                    PropertyInfo[] properties = type.GetProperties();

                    var colNameIdList = new List<ExcelColumnNameId>();
                    foreach (var prop in properties)
                    {
                        var colName = prop.Name.Replace("_", " ");
                        var cell = ws.RangeUsed().AsTable().HeadersRow().CellsUsed(c => c.Value.ToString() == colName).FirstOrDefault().Address.ColumnNumber;
                        colNameIdList.Add(new ExcelColumnNameId { ColumnId = cell, ColumnName = prop.Name });
                    }


                    totalcnt = ex.ExportList.Count();
                    if (ex.ExportList != null && totalcnt > 0)
                    {
                        rowcnt = 2;
                        foreach (var item in ex.ExportList)
                        {
                            //setterStatus(sbStatus.ToString() + "--Adding " + (rowcnt - 1).ToString("N0") + " out of " + totalcnt.ToString("N0") + " rows..." + Environment.NewLine);
                           // currentCol = 1;
                            foreach (var prop in properties)
                            {
                                // prop.Name

                                currentCol = colNameIdList.Where(x => x.ColumnName == prop.Name).Select(x => x.ColumnId).FirstOrDefault();

                                if (prop.PropertyType == typeof(string))
                                {


                                    ws.Cell(rowcnt, currentCol).Value = prop.GetValue(item, null) + "";
                                }
                                else
                                {

                                    ws.Cell(rowcnt, currentCol).Value = int.Parse(prop.GetValue(item, null) + "");
                                }


                                currentCol++;
                            }
                            rowcnt++;
                        }

                        //DELETE LEFTOVER TEMPLATE GARBAGE
                        ws.Range("A" + rowcnt  + ":Z" + (rowcnt + 200)).Delete(XLShiftDeletedCells.ShiftCellsUp);


                        var rows = ws.RangeUsed().RowsUsed().Skip(1); // Skip header row
                        foreach (var row in rows)
                        {
                            var rowNumber = row.RowNumber();
                            // Process the row
                            var cells = row.Cells();
                            var cnt = 1;
                            foreach (var cell in cells)
                            {
                                if (cnt == cells.Count() - 1)
                                {
                                    break;
                                }

                                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                                cnt++;
                            }    
                        }



                       // ws.RangeUsed().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        //for (int i = 0; i < 200; i++)
                        //{
                        //    ws.Range("A" + (rowcnt + i) + ":Z" + (rowcnt + i) ).Delete(XLShiftDeletedCells.ShiftCellsUp);

                        //}




                        //sbStatus.Clear();
                        //sbStatus.Append(getterStatus());
                        //sbStatus.Append("--" + ex.SheetName + " has beeen generated." + Environment.NewLine);
                        //sbStatus.Append("-------------------------------------------" + Environment.NewLine);
                        //setterStatus(sbStatus.ToString());
                    }

                    //ws.Columns().AdjustToContents(1, 20);
                }
                //sbStatus.Append("--Opening spreadsheet..." + Environment.NewLine + Environment.NewLine);
                //setterStatus(sbStatus.ToString());

                //save file to memory stream and return it as byte array
                using (var ms = new MemoryStream())
                {
                    wb.SaveAs(ms);

                    final = ms.ToArray();
                }

            });
            t.Wait(); // Wait until the above task is complete, email is sent
            return final;
        }





        public object GetValueFromExcel(string fileName, string sheetName, string cell)
        {
            object value = null;
            using (IXLWorkbook workbook = new XLWorkbook(fileName))
            {
                var worksheet = workbook.Worksheets.Where(w => w.Name == sheetName).FirstOrDefault();

                value = worksheet.Cell(cell).Value;
            }

            return value;
        }


        private int ExcelColumnNameToNumber(string columnName)
        {
            if (string.IsNullOrEmpty(columnName)) throw new ArgumentNullException("columnName");

            columnName = columnName.ToUpperInvariant();

            int sum = 0;

            for (int i = 0; i < columnName.Length; i++)
            {
                sum *= 26;
                sum += (columnName[i] - 'A' + 1);
            }

            return sum;
        }



    }
}
