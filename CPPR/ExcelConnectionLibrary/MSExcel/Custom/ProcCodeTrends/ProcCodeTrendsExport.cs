using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Irony.Parsing;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Word;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using VCPortal_Models.Models.MHP;
using VCPortal_Models.Models.ProcCodeTrends;
using Task = System.Threading.Tasks.Task;

namespace FileParsingLibrary.MSExcel.Custom.ProcCodeTrends
{
    public class ProcCodeTrendsExport
    {

        public static async Task<byte[]> ExportProcDataToExcel(CLM_OP_Report_Model clm_op_results, Func<string> getterStatus, Action<string> setterStatus, CancellationToken token)
        {

            //throw new Exception("Oh nooooooo!!!");
            byte[] final = new byte[0];



            XLWorkbook wb = new XLWorkbook();
            IXLWorksheet wsSource = null;
            IXLRange range;
            IXLCell cell;
    
            StringBuilder sbStatus = new StringBuilder();
            sbStatus.Append(getterStatus());

            string header;
            string columnLetter;
            string columnLetterLast;
            string bgcolor = "#D9D9D9";

            //Allowed Amount PMPM START 
            //Allowed Amount PMPM START
            //Allowed Amount PMPM START
            header = "OP Allowed Amount PMPM";

            sbStatus.Append("--Creating sheet for " + header + Environment.NewLine);
            setterStatus(sbStatus.ToString());

            generateGenericWorksheet<Allowed_PMPM_Model>(ref wb, header, bgcolor, clm_op_results.year_quarter_op, clm_op_results.allowed_pmpm_op, clm_op_results.allowed_pmpm_op_comment, display: "Dollars");

            ////Allowed Amount PMPM END
            ////Allowed Amount PMPM END
            ////Allowed Amount PMPM END


            //Allowed Amount PMPM START 
            //Allowed Amount PMPM START
            //Allowed Amount PMPM START
            header = "PHYS Allowed Amount PMPM";

            sbStatus.Append("--Creating sheet for " + header + Environment.NewLine);
            setterStatus(sbStatus.ToString());

            generateGenericWorksheet<Allowed_PMPM_Model>(ref wb, header, bgcolor, clm_op_results.year_quarter_op, clm_op_results.allowed_pmpm_phys, clm_op_results.allowed_pmpm_phys_comment, display: "Dollars");

            ////Allowed Amount PMPM END
            ////Allowed Amount PMPM END
            ////Allowed Amount PMPM END



            //Utilization/000  START 
            //Utilization/000 START
            //Utilization/000 START
            header = "OP Utilization/000";

            sbStatus.Append("--Creating sheet for " + header + Environment.NewLine);
            setterStatus(sbStatus.ToString());

            generateGenericWorksheet<Utilization000_Model>(ref wb, header, bgcolor, clm_op_results.year_quarter_op, clm_op_results.utilization000_op, clm_op_results.utilization000_op_comment);

            ////Utilization/000 END
            ////Utilization/000 END
            ////Utilization/000 END
            ///



            //Utilization/000  START 
            //Utilization/000 START
            //Utilization/000 START
            header = "PHYS Utilization/000";

            sbStatus.Append("--Creating sheet for " + header + Environment.NewLine);
            setterStatus(sbStatus.ToString());

            generateGenericWorksheet<Utilization000_Model>(ref wb, header, bgcolor, clm_op_results.year_quarter_op, clm_op_results.utilization000_phys, clm_op_results.utilization000_phys_comment);

            ////Utilization/000 END
            ////Utilization/000 END
            ////Utilization/000 END

            //Unit Cost 1  START 
            //Unit Cost 1 START
            //Unit Cost 1 START
            header = "OP Event Cost";

            sbStatus.Append("--Creating sheet for " + header + Environment.NewLine);
            setterStatus(sbStatus.ToString());

            generateGenericWorksheet<Unit_Cost1_Model>(ref wb, header, bgcolor, clm_op_results.year_quarter_op, clm_op_results.unit_cost1_op, clm_op_results.unit_cost_op_comment, display: "Dollars");

            ////Unit Cost 1 END
            ////Unit Cost 1 END
            ////Unit Cost 1 END


            //Unit Cost 1  START 
            //Unit Cost 1 START
            //Unit Cost 1 START
            header = "PHYS Event Cost";

            sbStatus.Append("--Creating sheet for " + header + Environment.NewLine);
            setterStatus(sbStatus.ToString());

            generateGenericWorksheet<Unit_Cost1_Model>(ref wb, header, bgcolor, clm_op_results.year_quarter_op, clm_op_results.unit_cost1_phys, clm_op_results.unit_cost_phys_comment, display: "Dollars");

            ////Unit Cost 1 END
            ////Unit Cost 1 END
            ////Unit Cost 1 END


            //Unit Cost 2  START 
            //Unit Cost 2 START
            //Unit Cost 2 START
            header = "OP Unit Cost";

            sbStatus.Append("--Creating sheet for " + header + Environment.NewLine);
            setterStatus(sbStatus.ToString());

            generateGenericWorksheet<Unit_Cost2_Model>(ref wb, header, bgcolor, clm_op_results.year_quarter_op, clm_op_results.unit_cost2_op, clm_op_results.unit_cost_op_comment, display: "Dollars");

            ////Unit Cost 2 END
            ////Unit Cost 2 END
            ////Unit Cost 2 END

            //Unit Cost 2  START 
            //Unit Cost 2 START
            //Unit Cost 2 START
            header = "PHYS Unit Cost";

            sbStatus.Append("--Creating sheet for " + header + Environment.NewLine);
            setterStatus(sbStatus.ToString());

            generateGenericWorksheet<Unit_Cost2_Model>(ref wb, header, bgcolor, clm_op_results.year_quarter_op, clm_op_results.unit_cost2_phys, clm_op_results.unit_cost_phys_comment, display: "Dollars");

            ////Unit Cost 2 END
            ////Unit Cost 2 END
            ////Unit Cost 2 END



            //CLM OP Unique Individual START
            //CLM OP Unique Individual START
            //CLM OP Unique Individual START
            header = "OP Unique Individual";


            sbStatus.Append("--Creating sheet for " + header + Environment.NewLine);
            setterStatus(sbStatus.ToString());
       
            generateGenericWorksheet<Unique_Individual_Model>(ref wb, header, bgcolor, clm_op_results.year_quarter_op, clm_op_results.unique_individual_op, clm_op_results.unique_individual_op_comment, has_trend: false);

            //CLM OP Unique Individual END
            //CLM OP Unique Individual END
            //CLM OP Unique Individual END



            //CLM PHYS Unique Individual START
            //CLM PHYS Unique Individual START
            //CLM PHYS Unique Individual START
            header = "PHYS Unique Individual";


            sbStatus.Append("--Creating sheet for " + header + Environment.NewLine);
            setterStatus(sbStatus.ToString());

            generateGenericWorksheet<Unique_Individual_Model>(ref wb, header, bgcolor, clm_op_results.year_quarter_op, clm_op_results.unique_individual_phys, clm_op_results.unique_individual_phys_comment, has_trend: false);

            //CLM OP Unique Individual END
            //CLM OP Unique Individual END
            //CLM OP Unique Individual END



            //Events START
            //Events START
            //Events START
            header = "OP Events";

            sbStatus.Append("--Creating sheet for " + header + Environment.NewLine);
            setterStatus(sbStatus.ToString());

            generateGenericWorksheet<Events_Model>(ref wb, header, bgcolor, clm_op_results.year_quarter_op, clm_op_results.events_op, clm_op_results.events_op_comment, has_trend: false);


            ////Events END
            ////Events END
            ////Events END



            //Events START
            //Events START
            //Events START
            header = "PHYS Events";

            sbStatus.Append("--Creating sheet for " + header + Environment.NewLine);
            setterStatus(sbStatus.ToString());

            generateGenericWorksheet<Events_Model>(ref wb, header, bgcolor, clm_op_results.year_quarter_op, clm_op_results.events_phys, clm_op_results.events_phys_comment, has_trend: false);


            ////Events END
            ////Events END
            ////Events END




            //Claims START 
            //Claims START
            //Claims START
            header = "OP Claims";

            sbStatus.Append("--Creating sheet for " + header + Environment.NewLine);
            setterStatus(sbStatus.ToString());
            //ADD CUSTOM!!!!!!
            generateClaimsWorksheet<Op_Claims_Model>(ref wb, header, bgcolor, clm_op_results.year_quarter_op, clm_op_results.claims_op, clm_op_results.claims_op_comment, false);

            ////Claims END
            ////Claims END
            ////Claims END


            //Claims START 
            //Claims START
            //Claims START
            header = "PHYS Claims";

            sbStatus.Append("--Creating sheet for " + header + Environment.NewLine);
            setterStatus(sbStatus.ToString());
            //ADD CUSTOM!!!!!!
            generateClaimsWorksheet<Phys_Claims_Model>(ref wb, header, bgcolor, clm_op_results.year_quarter_op, clm_op_results.claims_phys, clm_op_results.claims_phys_comment, true);

            ////Claims END
            ////Claims END
            ////Claims END


            //Allowed Amount START 
            //Allowed Amount START
            //Allowed Amount START
            header = "OP Allowed Amount";

            sbStatus.Append("--Creating sheet for " + header + Environment.NewLine);
            setterStatus(sbStatus.ToString());

            generateGenericWorksheet<Allowed_Model>(ref wb, header, bgcolor, clm_op_results.year_quarter_op, clm_op_results.allowed_op, clm_op_results.allowed_op_comment, display: "Dollars", has_trend: false);

            ////Allowed Amount END
            ////Allowed Amount END
            ////Allowed Amount END
 

             //Allowed Amount START 
            //Allowed Amount START
            //Allowed Amount START
            header = "PHYS Allowed Amount";

            sbStatus.Append("--Creating sheet for " + header + Environment.NewLine);
            setterStatus(sbStatus.ToString());

            generateGenericWorksheet<Allowed_Model>(ref wb, header, bgcolor, clm_op_results.year_quarter_op, clm_op_results.allowed_phys, clm_op_results.allowed_phys_comment, display: "Dollars", has_trend: false);

            ////Allowed Amount END
            ////Allowed Amount END
            ////Allowed Amount END



            //Member Month START
            //Member Month START
            //Member Month START
            header = "Member Month";

            sbStatus.Append("--Creating sheet for " + header + Environment.NewLine);
            setterStatus(sbStatus.ToString());
            //ADD CUSTOM!!!!!!
            genertateMemberMonthWorksheet<Member_Month_Model>(ref wb, header, bgcolor, clm_op_results.year_quarter_op, clm_op_results.member_month_op, clm_op_results.member_month_comment);
            //Member Month END
            //Member Month END
            //Member Month END




            if (token.IsCancellationRequested)
            {
                setterStatus("~~~Report Generation Cancelled~~~");
                token.ThrowIfCancellationRequested();
            }


            sbStatus.Append("--Preparing Excel file for saving" + Environment.NewLine);
            setterStatus(sbStatus.ToString());

            using (var ms = new MemoryStream())
            {
                wb.SaveAs(ms);

                final = ms.ToArray();
            }

            await Task.CompletedTask;
            return final;
        }


        private static void generateGenericWorksheet<T>(ref XLWorkbook wb, string header, string bgcolor, List<YearQuarter_Model> year_quarter, List<T> data_list, string comment, string note = null, string display = null, bool has_trend = true)
        {


            string columnLetter;
            string columnLetterLast;
            IXLRange range;
            IXLCell cell;


            Int16 colCnt = 1;
            Int16 rowCnt = 1;


            var sheet_name =  header.Replace("/", "");

            var wsSource = wb.Worksheets.Add(sheet_name);


            //MAIN HEADER 'Events' ROW
            rowCnt = 1;

            colCnt = 0;
            columnLetter = SharedExcelFunctions.GetColumnName(colCnt);
            cell = wsSource.Cell(columnLetter + rowCnt);
            cell.Value = header + (note != null ? " *" : "");
            //cell.Style.Font.SetBold(true);

            colCnt = 2;
            columnLetter = SharedExcelFunctions.GetColumnName(colCnt);
            cell = wsSource.Cell(columnLetter + rowCnt);
            cell.Value = header;
            cell.Style.Fill.SetBackgroundColor(XLColor.FromHtml(bgcolor)); //217 217 217
            SharedExcelFunctions.AddClosedXMLBorders(ref cell);

            //COLUMN HEADER ROW
            rowCnt = 2;

            colCnt = 0;
            columnLetter = SharedExcelFunctions.GetColumnName(colCnt);
            cell = wsSource.Cell(columnLetter + rowCnt);
            cell.Value = "Proc" + Environment.NewLine + "Code";
            cell.Style.Fill.SetBackgroundColor(XLColor.FromHtml(bgcolor)); //217 217 217
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            SharedExcelFunctions.AddClosedXMLBorders(ref cell);


            colCnt = 1;
            columnLetter = SharedExcelFunctions.GetColumnName(colCnt);
            cell = wsSource.Cell(columnLetter + rowCnt);
            cell.Value = "Proc Desc";
            cell.Style.Fill.SetBackgroundColor(XLColor.FromHtml(bgcolor)); //217 217 217
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            SharedExcelFunctions.AddClosedXMLBorders(ref cell);

            colCnt = 2;
            //LOOP YQ COLUMN HEADERS
            foreach (var yq in year_quarter)
            {
                columnLetter = SharedExcelFunctions.GetColumnName(colCnt);
                cell = wsSource.Cell(columnLetter + rowCnt);

                cell.Value = yq.year + "Q" + yq.quarter;
                cell.Style.Fill.SetBackgroundColor(XLColor.FromHtml(bgcolor)); //217 217 217
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                SharedExcelFunctions.AddClosedXMLBorders(ref cell);

                colCnt++;
            }

            //MERGE YQ "Unique Individual"
            columnLetter = SharedExcelFunctions.GetColumnName(colCnt - 1);
            columnLetterLast = SharedExcelFunctions.GetColumnName(colCnt);
            range = wsSource.Range("C1:" + columnLetter + "1");
            range.Merge();
            range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            range.Style.Font.SetBold(true);

            if(!string.IsNullOrEmpty(comment))
            {
                wsSource.Cell("C1").CreateComment().AddText(comment);
            }
            

            if(has_trend)
            {
                //ADD MAIN HEADER 'Trend' ROW
                rowCnt = 1;
                columnLetter = SharedExcelFunctions.GetColumnName(colCnt);
                cell = wsSource.Cell(columnLetter + rowCnt);
                cell.Value = "Trend";
                cell.Style.Fill.SetBackgroundColor(XLColor.FromHtml(bgcolor)); //217 217 217


                //COLUMN HEADER ROW
                rowCnt = 2;
                //LOOP TREND COLUMN HEADERS

                var t = year_quarter[0].year.ToString().Substring(2, 2) + "Q" + year_quarter[0].quarter + "/" + year_quarter[4].year.ToString().Substring(2, 2) + "Q" + year_quarter[4].quarter;
                cell = wsSource.Cell(columnLetter + rowCnt);
                cell.Value = t;
                cell.Style.Fill.SetBackgroundColor(XLColor.FromHtml(bgcolor)); //217 217 217
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                SharedExcelFunctions.AddClosedXMLBorders(ref cell);


                t = year_quarter[1].year.ToString().Substring(2, 2) + "Q" + year_quarter[1].quarter + "/" + year_quarter[5].year.ToString().Substring(2, 2) + "Q" + year_quarter[5].quarter;
                colCnt++;
                columnLetter = SharedExcelFunctions.GetColumnName(colCnt);
                cell = wsSource.Cell(columnLetter + rowCnt);
                cell.Value = t;
                cell.Style.Fill.SetBackgroundColor(XLColor.FromHtml(bgcolor)); //217 217 217
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                SharedExcelFunctions.AddClosedXMLBorders(ref cell);



                t = year_quarter[2].year.ToString().Substring(2, 2) + "Q" + year_quarter[2].quarter + "/" + year_quarter[6].year.ToString().Substring(2, 2) + "Q" + year_quarter[6].quarter;
                colCnt++;
                columnLetter = SharedExcelFunctions.GetColumnName(colCnt);
                cell = wsSource.Cell(columnLetter + rowCnt);
                cell.Value = t;
                cell.Style.Fill.SetBackgroundColor(XLColor.FromHtml(bgcolor)); //217 217 217
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                SharedExcelFunctions.AddClosedXMLBorders(ref cell);



                t = year_quarter[3].year.ToString().Substring(2, 2) + "Q" + year_quarter[3].quarter + "/" + year_quarter[7].year.ToString().Substring(2, 2) + "Q" + year_quarter[7].quarter;
                colCnt++;
                columnLetter = SharedExcelFunctions.GetColumnName(colCnt);
                cell = wsSource.Cell(columnLetter + rowCnt);
                cell.Value = t;
                cell.Style.Fill.SetBackgroundColor(XLColor.FromHtml(bgcolor)); //217 217 217
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                SharedExcelFunctions.AddClosedXMLBorders(ref cell);


                //MERGE YQ "Trend"
                range = wsSource.Range(columnLetterLast + "1:" + columnLetter + "1");
                range.Merge();
                range.Style.Border.RightBorderColor = XLColor.Black;
                range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                range.Style.Font.SetBold(true);
            }

            

            //DATA ROW
            rowCnt = 3;

            //START FROM FIRST COLUMN
            colCnt = 0;
            //POPULATE DATA
            foreach (var c in data_list)
            {

                foreach (PropertyInfo propertyInfo in c.GetType().GetProperties())
                {

                    object val = propertyInfo.GetValue(c, null);
                    columnLetter = SharedExcelFunctions.GetColumnName(colCnt);
                    cell = wsSource.Cell(columnLetter + rowCnt);

                    if (propertyInfo.PropertyType == typeof(int) || propertyInfo.PropertyType == typeof(int?))
                    {
                        if (val != null)
                        {
                            cell.Value = int.Parse(val.ToString());
                            if(display==null)
                            {
                                cell.Style.NumberFormat.Format = "_(#,##0_)";
                            }
                            else if(display == "Dollars")
                            {

                                cell.Style.NumberFormat.Format = "_($#,##0_)";
                            }

                        }
                        else
                        {
                            cell.Value = ".";
                        }
                    }
                    else if (propertyInfo.PropertyType == typeof(float) || propertyInfo.PropertyType == typeof(float?))
                    {
                        if (val != null)
                        {
                            cell.Value = double.Parse(val.ToString());

                            if (display == "Dollars")
                            {

                                cell.Style.NumberFormat.Format = "_($#,##0.00_)";
                            }

                        }
                        else
                        {
                            cell.Value = ".";
                        }
                    }
                    else if (propertyInfo.PropertyType == typeof(double) || propertyInfo.PropertyType == typeof(double?))
                    {
                        if (val != null)
                        {
                            cell.Value = double.Parse(val.ToString());
                            cell.Style.NumberFormat.Format = "_(0%_)";

                        }
                        else
                        {
                            cell.Value = ".";
                        }

                    }
                    else
                    {
                        cell.Value = val + "";
                    }


                    SharedExcelFunctions.AddClosedXMLBorders(ref cell);

                    colCnt++;
                }


                rowCnt++;
                colCnt = 0;
            }
            wsSource.Columns().AdjustToContents();


            if(note != null )
            {
                columnLetter = SharedExcelFunctions.GetColumnName(colCnt);
                cell = wsSource.Cell(columnLetter + rowCnt);
                cell.Value = note;
            }

        }

        private static void generateClaimsWorksheet<T>(ref XLWorkbook wb, string header, string bgcolor, List<YearQuarter_Model> year_quarter, List<T> data_list,  string comment, bool is_phy, string note = null, string display = null)
        {


            string columnLetter;
            string columnLetterFirst;
            string columnLetterLast;
            IXLRange range;
            IXLCell cell;


            Int16 colCnt = 1;
            Int16 rowCnt = 1;


            var sheet_name = header.Replace("/", "");

            var wsSource = wb.Worksheets.Add(sheet_name);


            List<string> claims = new List<string>();


            if(!is_phy)
            {
                claims.Add("Total");
                claims.Add("Other");
            }
            else
            {
                var s = "";

            }
                

            claims.Add(is_phy ? "Physician" : "Facility");
  



            colCnt = 0;
            foreach (var claim in claims)
            {

                //MAIN HEADER 'Events' ROW
                rowCnt = 1;

                if (claim == "Total" || claim == "Physician")
                {
                    columnLetter = SharedExcelFunctions.GetColumnName(colCnt);
                    cell = wsSource.Cell(columnLetter + rowCnt);
                    cell.Value = header.Replace("OP ", "").Replace("PHYS ", "") + (note != null ? " *" : "");
                    //cell.Style.Font.SetBold(true);
                    colCnt += 2;
                }
                else
                {
                    //colCnt += 1;
                }

                columnLetter = SharedExcelFunctions.GetColumnName(colCnt);
                columnLetterFirst = columnLetter;
                cell = wsSource.Cell(columnLetter + rowCnt);
                cell.Value = header.Replace("OP", claim).Replace("PHYS", claim);
                cell.Style.Fill.SetBackgroundColor(XLColor.FromHtml(bgcolor)); //217 217 217
                SharedExcelFunctions.AddClosedXMLBorders(ref cell);

                //IF FIRST PASS
                if(claim == "Total" || claim == "Physician")
                {
                    //COLUMN HEADER ROW
                    rowCnt = 2;

                    colCnt = 0;
                    columnLetter = SharedExcelFunctions.GetColumnName(0);
                    cell = wsSource.Cell(columnLetter + rowCnt);
                    cell.Value = "Proc" + Environment.NewLine + "Code";
                    cell.Style.Fill.SetBackgroundColor(XLColor.FromHtml(bgcolor)); //217 217 217
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    SharedExcelFunctions.AddClosedXMLBorders(ref cell);


                    colCnt = 1;
                    columnLetter = SharedExcelFunctions.GetColumnName(1);
                    cell = wsSource.Cell(columnLetter + rowCnt);
                    cell.Value = "Proc Desc";
                    cell.Style.Fill.SetBackgroundColor(XLColor.FromHtml(bgcolor)); //217 217 217
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    SharedExcelFunctions.AddClosedXMLBorders(ref cell);

                }


                if (claim == "Total" || claim == "Physician")
                {
                    colCnt += 1;
                }
                else
                {
                    rowCnt = 2;
                }
              
                //LOOP YQ COLUMN HEADERS
                foreach (var yq in year_quarter)
                {
                    columnLetter = SharedExcelFunctions.GetColumnName(colCnt);
                    cell = wsSource.Cell(columnLetter + rowCnt);

                    cell.Value = yq.year + "Q" + yq.quarter;
                    cell.Style.Fill.SetBackgroundColor(XLColor.FromHtml(bgcolor)); //217 217 217
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    SharedExcelFunctions.AddClosedXMLBorders(ref cell);

                    colCnt++;
                }

                //MERGE YQ "Unique Individual"
                columnLetter = SharedExcelFunctions.GetColumnName(colCnt - 1);
                columnLetterLast = SharedExcelFunctions.GetColumnName(colCnt);
                range = wsSource.Range(columnLetterFirst + "1:" + columnLetter + "1");
                range.Merge();
                range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                range.Style.Font.SetBold(true);
                if (!string.IsNullOrEmpty(comment))
                {
                    wsSource.Cell(columnLetterFirst + "1").CreateComment().AddText(comment);
                }

                ////ADD MAIN HEADER 'Trend' ROW
                //rowCnt = 1;
                //columnLetter = SharedExcelFunctions.GetColumnName(colCnt);
                //cell = wsSource.Cell(columnLetter + rowCnt);
                //cell.Value = header.Replace("OP", claim).Replace("PHYS", claim) + " Trend";
                //cell.Style.Fill.SetBackgroundColor(XLColor.FromHtml(bgcolor)); //217 217 217


                ////COLUMN HEADER ROW
                //rowCnt = 2;
                ////LOOP TREND COLUMN HEADERS

                //var t = year_quarter[0].year.ToString().Substring(2, 2) + "Q" + year_quarter[0].quarter + "/" + year_quarter[4].year.ToString().Substring(2, 2) + "Q" + year_quarter[4].quarter;
                //cell = wsSource.Cell(columnLetter + rowCnt);
                //cell.Value = t;
                //cell.Style.Fill.SetBackgroundColor(XLColor.FromHtml(bgcolor)); //217 217 217
                //cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                //cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                //SharedExcelFunctions.AddClosedXMLBorders(ref cell);


                //t = year_quarter[1].year.ToString().Substring(2, 2) + "Q" + year_quarter[1].quarter + "/" + year_quarter[5].year.ToString().Substring(2, 2) + "Q" + year_quarter[5].quarter;
                //colCnt++;
                //columnLetter = SharedExcelFunctions.GetColumnName(colCnt);
                //cell = wsSource.Cell(columnLetter + rowCnt);
                //cell.Value = t;
                //cell.Style.Fill.SetBackgroundColor(XLColor.FromHtml(bgcolor)); //217 217 217
                //cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                //cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                //SharedExcelFunctions.AddClosedXMLBorders(ref cell);



                //t = year_quarter[2].year.ToString().Substring(2, 2) + "Q" + year_quarter[2].quarter + "/" + year_quarter[6].year.ToString().Substring(2, 2) + "Q" + year_quarter[6].quarter;
                //colCnt++;
                //columnLetter = SharedExcelFunctions.GetColumnName(colCnt);
                //cell = wsSource.Cell(columnLetter + rowCnt);
                //cell.Value = t;
                //cell.Style.Fill.SetBackgroundColor(XLColor.FromHtml(bgcolor)); //217 217 217
                //cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                //cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                //SharedExcelFunctions.AddClosedXMLBorders(ref cell);



                //t = year_quarter[3].year.ToString().Substring(2, 2) + "Q" + year_quarter[3].quarter + "/" + year_quarter[7].year.ToString().Substring(2, 2) + "Q" + year_quarter[7].quarter;
                //colCnt++;
                //columnLetter = SharedExcelFunctions.GetColumnName(colCnt);
                //cell = wsSource.Cell(columnLetter + rowCnt);
                //cell.Value = t;
                //cell.Style.Fill.SetBackgroundColor(XLColor.FromHtml(bgcolor)); //217 217 217
                //cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                //cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                //SharedExcelFunctions.AddClosedXMLBorders(ref cell);


                ////MERGE YQ "Trend"
                //range = wsSource.Range(columnLetterLast + "1:" + columnLetter + "1");
                //range.Merge();
                //range.Style.Border.RightBorderColor = XLColor.Black;
                //range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                //range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                //range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                //range.Style.Font.SetBold(true);

            }



            //DATA ROW
            rowCnt = 3;

            //START FROM FIRST COLUMN
            colCnt = 0;
            //POPULATE DATA
            foreach (var c in data_list)
            {

                foreach (PropertyInfo propertyInfo in c.GetType().GetProperties())
                {

                    object val = propertyInfo.GetValue(c, null);
                    columnLetter = SharedExcelFunctions.GetColumnName(colCnt);
                    cell = wsSource.Cell(columnLetter + rowCnt);

                    if (propertyInfo.PropertyType == typeof(int) || propertyInfo.PropertyType == typeof(int?))
                    {
                        if (val != null)
                        {
                            cell.Value = int.Parse(val.ToString());
                            if (display == null)
                            {
                                cell.Style.NumberFormat.Format = "_(#,##0_)";
                            }
                            else if (display == "Dollars")
                            {

                                cell.Style.NumberFormat.Format = "_($#,##0_)";
                            }

                        }
                        else
                        {
                            cell.Value = ".";
                        }
                    }
                    else if (propertyInfo.PropertyType == typeof(float) || propertyInfo.PropertyType == typeof(float?))
                    {
                        if (val != null)
                        {
                            cell.Value = double.Parse(val.ToString());

                            if (display == "Dollars")
                            {

                                cell.Style.NumberFormat.Format = "_($#,##0.00_)";
                            }


                        }
                        else
                        {
                            cell.Value = ".";
                        }
                    }
                    else if (propertyInfo.PropertyType == typeof(double) || propertyInfo.PropertyType == typeof(double?) )
                    {
                        if (val != null)
                        {
                            cell.Value = double.Parse(val.ToString());
                             cell.Style.NumberFormat.Format = "_(0%_)";
             
                        }
                        else
                        {
                            cell.Value = ".";
                        }
                    }
                    else
                    {
                        cell.Value = val + "";
                    }


                    SharedExcelFunctions.AddClosedXMLBorders(ref cell);

                    colCnt++;
                }


                rowCnt++;
                colCnt = 0;
            }
            wsSource.Columns().AdjustToContents();


            if (note != null)
            {
                columnLetter = SharedExcelFunctions.GetColumnName(colCnt);
                cell = wsSource.Cell(columnLetter + rowCnt);
                cell.Value = note;
            }

        }

        private static void genertateMemberMonthWorksheet<T>(ref XLWorkbook wb, string header, string bgcolor, List<YearQuarter_Model> year_quarter, List<T> data_list, string comment)
        {


            string columnLetter;
            string columnLetterLast;
            IXLRange range;
            IXLCell cell;


            Int16 colCnt = 1;
            Int16 rowCnt = 1;


            var wsSource = wb.Worksheets.Add(header);


            //MAIN HEADER 'Events' ROW
            rowCnt = 1;

            colCnt = 1;
            //LOOP YQ COLUMN HEADERS
            foreach (var yq in year_quarter)
            {
                columnLetter = SharedExcelFunctions.GetColumnName(colCnt);
                cell = wsSource.Cell(columnLetter + rowCnt);

                cell.Value = yq.year + "Q" + yq.quarter;
                cell.Style.Fill.SetBackgroundColor(XLColor.FromHtml(bgcolor)); //217 217 217
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                SharedExcelFunctions.AddClosedXMLBorders(ref cell);

                colCnt++;
            }


            //COLUMN HEADER ROW
            rowCnt = 1;
            //LOOP TREND COLUMN HEADERS

            var t = year_quarter[0].year.ToString().Substring(2, 2) + "Q" + year_quarter[0].quarter + "/" + year_quarter[4].year.ToString().Substring(2, 2) + "Q" + year_quarter[4].quarter;
            columnLetter = SharedExcelFunctions.GetColumnName(colCnt);
            cell = wsSource.Cell(columnLetter + rowCnt);
            cell.Value = t;
            cell.Style.Fill.SetBackgroundColor(XLColor.FromHtml(bgcolor)); //217 217 217
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            SharedExcelFunctions.AddClosedXMLBorders(ref cell);


            t = year_quarter[1].year.ToString().Substring(2, 2) + "Q" + year_quarter[1].quarter + "/" + year_quarter[5].year.ToString().Substring(2, 2) + "Q" + year_quarter[5].quarter;
            colCnt++;
            columnLetter = SharedExcelFunctions.GetColumnName(colCnt);
            cell = wsSource.Cell(columnLetter + rowCnt);
            cell.Value = t;
            cell.Style.Fill.SetBackgroundColor(XLColor.FromHtml(bgcolor)); //217 217 217
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            SharedExcelFunctions.AddClosedXMLBorders(ref cell);



            t = year_quarter[2].year.ToString().Substring(2, 2) + "Q" + year_quarter[2].quarter + "/" + year_quarter[6].year.ToString().Substring(2, 2) + "Q" + year_quarter[6].quarter;
            colCnt++;
            columnLetter = SharedExcelFunctions.GetColumnName(colCnt);
            cell = wsSource.Cell(columnLetter + rowCnt);
            cell.Value = t;
            cell.Style.Fill.SetBackgroundColor(XLColor.FromHtml(bgcolor)); //217 217 217
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            SharedExcelFunctions.AddClosedXMLBorders(ref cell);



            t = year_quarter[3].year.ToString().Substring(2, 2) + "Q" + year_quarter[3].quarter + "/" + year_quarter[7].year.ToString().Substring(2, 2) + "Q" + year_quarter[7].quarter;
            colCnt++;
            columnLetter = SharedExcelFunctions.GetColumnName(colCnt);
            cell = wsSource.Cell(columnLetter + rowCnt);
            cell.Value = t;
            cell.Style.Fill.SetBackgroundColor(XLColor.FromHtml(bgcolor)); //217 217 217
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            SharedExcelFunctions.AddClosedXMLBorders(ref cell);

            //DATA ROW
            rowCnt = 2;

            //START FROM FIRST COLUMN
            colCnt = 0;

            //POPULATE DATA
            foreach (var c in data_list)
            {

                
                foreach (PropertyInfo propertyInfo in c.GetType().GetProperties())
                {

                    object val = propertyInfo.GetValue(c, null);
                    columnLetter = SharedExcelFunctions.GetColumnName(colCnt);
                    cell = wsSource.Cell(columnLetter + rowCnt);

                    if (propertyInfo.PropertyType == typeof(int) || propertyInfo.PropertyType == typeof(int?))
                    {
                        if (val != null)
                        {
                            cell.Value = int.Parse(val.ToString());
                            cell.Style.NumberFormat.Format = "_( #,##0_)";
                        }
                        else
                        {
                            cell.Value = ".";
                        }
                    }
                    else if (propertyInfo.PropertyType == typeof(float) || propertyInfo.PropertyType == typeof(float?))
                    {
                        if (val != null)
                        {
                            cell.Value = double.Parse(val.ToString());
                        }
                        else
                        {
                            cell.Value = ".";
                        }
                    }
                    else if (propertyInfo.PropertyType == typeof(double) || propertyInfo.PropertyType == typeof(double?))
                    {
                        if (val != null)
                        {
                            cell.Value = double.Parse(val.ToString());
                            cell.Style.NumberFormat.Format = "_(0%_)";

                        }
                        else
                        {
                            cell.Value = ".";
                        }
                    }
                    else
                    {
                        cell.Value = val + "";
                    }


                    SharedExcelFunctions.AddClosedXMLBorders(ref cell);

                    colCnt++;
                }


                rowCnt++;
                colCnt = 0;
            }
            wsSource.Columns().AdjustToContents();


        }

    }
}
