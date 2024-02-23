using ClosedXML.Excel;
using FileParsingLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VCPortal_Models.Models.ProcCodeTrends;

namespace FileParsingLibrary.MSExcel.Custom.TAT
{
    public class TATExport
    {


        public async Task<byte[]> ExportToTATExcelTemplateAsync(string templateNamePath, List<ExcelExport> excelExports)
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
                        ws.Range("A" + rowcnt + ":Z" + (rowcnt + 10000)).Delete(XLShiftDeletedCells.ShiftCellsUp);


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

                        ws.Cell("A1").SetActive();

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


    }
}
