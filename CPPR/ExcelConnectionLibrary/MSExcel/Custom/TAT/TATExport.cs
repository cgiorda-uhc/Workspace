using ClosedXML.Excel;
using FileParsingLibrary.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VCPortal_Models.Models.ProcCodeTrends;
using VCPortal_Models.Models.TAT;

namespace FileParsingLibrary.MSExcel.Custom.TAT
{
    public class TATExport
    {


        public async Task<byte[]> ExportToTATExcelTemplateAsync(string templateNamePath, List<ExcelExport> excelExports, string current, string previous, int current_col, int previous_col, int starting_row)
        {
            //StringBuilder sbStatus = new StringBuilder();
            //sbStatus.Append("--Exporting to Excel..." + Environment.NewLine);
            //sbStatus.Append("-------------------------------------------" + Environment.NewLine);
            byte[] final = new byte[0];
            Task t = Task.Run(async () =>
            {
                using var wb = new XLWorkbook(templateNamePath); //create workbook
                int rowcnt = 0;
                int currentCol = 1;
                int totalcnt = 0;

                


                foreach (var ex in excelExports)
                {

                    var ws = wb.Worksheet(ex.SheetName); //add worksheet to workbook
                    var type = ex.ExportList!.FirstOrDefault()!.GetType();
                    PropertyInfo[] properties = type.GetProperties();

                    if(ex.SheetName == "SLA summary, penalties")
                    {
                        int section_cnt = 0;
                        
                        List<string> summary_type = new List<string>();
                        summary_type.Add("Radiology");
                        summary_type.Add("Cardiology");
                        summary_type.Add("Gastroenterology");
                        foreach (var st in summary_type)
                        {

                            var lstSummary = ex.ExportList.Cast<TAT_Summary_Model>().Where(x => x.rpt_Modality == st).ToList();
                            rowcnt = 8;
                   
    
                            foreach(var l in lstSummary)
                            {

                                currentCol = (st == "Radiology" ? 4 : (st == "Cardiology" ? 21 : 38));

                                ws.Cell(rowcnt, currentCol).Value = l.YTD_Penalty;

                                currentCol++;
                                ws.Cell(rowcnt, currentCol).Value = l.Jan;

                                currentCol++;
                                ws.Cell(rowcnt, currentCol).Value = l.Feb;

                                currentCol++;
                                ws.Cell(rowcnt, currentCol).Value = l.Mar;

                                currentCol++;
                                ws.Cell(rowcnt, currentCol).Value = l.Apr;

                                currentCol++;
                                ws.Cell(rowcnt, currentCol).Value = l.May;

                                currentCol++;
                                ws.Cell(rowcnt, currentCol).Value = l.Jun;

                                currentCol++;
                                ws.Cell(rowcnt, currentCol).Value = l.Jul;

                                currentCol++;
                                ws.Cell(rowcnt, currentCol).Value = l.Aug;

                                currentCol++;
                                ws.Cell(rowcnt, currentCol).Value = l.Sep;

                                currentCol++;
                                ws.Cell(rowcnt, currentCol).Value = l.Oct;

                                currentCol++;
                                ws.Cell(rowcnt, currentCol).Value = l.Nov;

                                currentCol++;
                                ws.Cell(rowcnt, currentCol).Value = l.Dec;


                                section_cnt++;
                                if(section_cnt == 5)
                                {
                                    rowcnt = rowcnt + 3;
                                    section_cnt = 0;
                                }

                                rowcnt++;
                            }

                        }
                    }
                    else
                    {
                        List<string> current_previous = new List<string>();
                        current_previous.Add("Current");
                        current_previous.Add("Previous");
                        List<TAT_Model> lstTat = null;


                        foreach (var cp in current_previous)
                        {


                            if (rowcnt == 0)
                            {
                                lstTat = ex.ExportList.Cast<TAT_Model>().Where(x => x.section == cp).ToList();
                                rowcnt = starting_row;
                                currentCol = (cp == "Current" ? current_col : previous_col);
                                ws.Cell(1, currentCol).Value = (cp == "Current" ? "Current Month " + current : "Prior Month " + previous);

                            }


                            foreach (var item in lstTat)
                            {
                                //setterStatus(sbStatus.ToString() + "--Adding " + (rowcnt - 1).ToString("N0") + " out of " + totalcnt.ToString("N0") + " rows..." + Environment.NewLine);
                                // currentCol = 1;
                                foreach (var prop in properties)
                                {
                                    if (prop.Name == "section")
                                    {
                                        continue;
                                    }

                                    object val = prop.GetValue(item, null);

                                    if (prop.PropertyType == typeof(string))
                                    {

                                        ws.Cell(rowcnt, currentCol).Value = prop.GetValue(item, null) + "";
                                    }
                                    else if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(int?))
                                    {

                                        if (val != null)
                                            ws.Cell(rowcnt, currentCol).Value = int.Parse(val.ToString());
                                    }
                                    else if (prop.PropertyType == typeof(float) || prop.PropertyType == typeof(float?))
                                    {
                                        if (val != null)
                                            ws.Cell(rowcnt, currentCol).Value = double.Parse(val.ToString());
                                    }

                                    currentCol++;
                                }
                                rowcnt++;
                                currentCol = (cp == "Current" ? current_col : previous_col);


                            }
                            rowcnt = 0;
                            ws.Cell("A1").SetActive();

                        }
                    }
                    //ws.Columns().AdjustToContents(1, 20);
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


    }
}
