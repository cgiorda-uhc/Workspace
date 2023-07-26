using ClosedXML.Excel;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DataTable = System.Data.DataTable;

namespace CS_Scorecard_Reporting
{
    class CS_Scorecard_Reporting
    {
        static void Main(string[] args)
        {

            //https://uhgazure.sharepoint.com/sites/csapmo/CPPR%20Files/Forms/AllItems.aspx?csf=1&web=1&e=t9MgnY&xsdata=MDV8MDF8fDk4NjM0ZWYxZDc5MTQ4MzA1MWUyMDhkYWMxYjIwNTA2fGRiMDVmYWNhYzgyYTRiOWRiOWM1MGY2NGI2NzU1NDIxfDF8MHw2MzgwMzUyNjc1NTg5ODYyMTB8R29vZHxWR1ZoYlhOVFpXTjFjbWwwZVZObGNuWnBZMlY4ZXlKV0lqb2lNQzR3TGpBd01EQWlMQ0pRSWpvaVYybHVNeklpTENKQlRpSTZJazkwYUdWeUlpd2lWMVFpT2pFeGZRPT18MXxNVGs2YldWbGRHbHVaMTlhYW1Sb1RXcFZlVTFVWjNSUFJHUnFXVk13TUU5VVNUTk1WR3MxV2xSSmRFMVhUbWxPYlZreFQxZGFiRnBVUW0xQWRHaHlaV0ZrTG5ZeXx8&sdata=aU90bHV4NDhZdTV5TjRRd0dJc2dNMlArYlRYU3hTbmtqQVdPT2hPVm1KZz0%3D&ovuser=db05faca%2Dc82a%2D4b9d%2Db9c5%2D0f64b6755421%2Cchris%5Fgiordano%40uhc%2Ecom&OR=Teams%2DHL&CT=1667929959300&clickparams=eyJBcHBOYW1lIjoiVGVhbXMtRGVza3RvcCIsIkFwcFZlcnNpb24iOiIyNy8yMjEwMjgwNzIwMCIsIkhhc0ZlZGVyYXRlZFVzZXIiOmZhbHNlfQ%3D%3D&cid=25505a2a%2D4b43%2D4b29%2Db08a%2Dc40b25147fcc&RootFolder=%2Fsites%2Fcsapmo%2FCPPR%20Files%2FEvicore%20Reporting%20Products%2FC%26S%20Maryland%20Quarterly%20Report&FolderCTID=0x012000D32D49C0CC2F374B9E88AFCF72986975


            string strILUCAConnectionString = ConfigurationManager.AppSettings["ILUCA"];
            string strSQL = getDrivingSQL(isRad:true);

            DataTable dtRad = DBConnection32.getMSSQLDataTable(strILUCAConnectionString, strSQL);

            strSQL = getDrivingSQL(isRad: false);
            DataTable dtCard = DBConnection32.getMSSQLDataTable(strILUCAConnectionString, strSQL);

            // NOTE: Don't call Excel objects in here... 
            // Debugger would keep alive until end, preventing GC cleanup
            // Call a separate function that talks to Excel
            GenerateReportXML(dtRad, dtCard);


            // Now let the GC clean up (repeat, until no more)
            do
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            while (Marshal.AreComObjectsAvailableForCleanup());
        }



        static void GenerateReportXML(System.Data.DataTable dtRad, System.Data.DataTable dtCard)
        {

            string strYQ = null;

            string strYQLatest = null;
            DataView dv = new DataView(dtCard.Select("file_year + '-' + file_month + '-01' >= '2018-07-01'").CopyToDataTable()); //LIMIT TO AFTER NC WAS ADDED!!!
            dv.Sort = "file_year DESC, file_month DESC";
            DataTable dtMain = dv.ToTable(true, "file_year", "file_month");
            dtMain.Columns.Add("file_quarter");
            foreach(DataRow dr in dtMain.Rows)
            {
                if (int.Parse(dr["file_month"].ToString()) == 1 || int.Parse(dr["file_month"].ToString()) == 2 || int.Parse(dr["file_month"].ToString()) == 3)
                    dr["file_quarter"] = "Q1";
                else if (int.Parse(dr["file_month"].ToString()) == 4 || int.Parse(dr["file_month"].ToString()) == 5 || int.Parse(dr["file_month"].ToString()) == 6)
                    dr["file_quarter"] = "Q2";
                else if (int.Parse(dr["file_month"].ToString()) == 7 || int.Parse(dr["file_month"].ToString()) == 8 || int.Parse(dr["file_month"].ToString()) == 9)
                    dr["file_quarter"] = "Q3";
                else if (int.Parse(dr["file_month"].ToString()) == 10 || int.Parse(dr["file_month"].ToString()) == 11 || int.Parse(dr["file_month"].ToString()) == 12)
                    dr["file_quarter"] = "Q4";
            }


            string strPath = AppDomain.CurrentDomain.BaseDirectory + "template\\CS_Reporting_Template.xlsx";

            XLWorkbook wb = new XLWorkbook(strPath);
            IXLWorksheet wsSource = null;

            //int intSheetCnt = 1;
            //foreach (IXLWorksheet worksheet in wb.Worksheets)
            //{
            //    intSheetCnt++;
            //}
            //intSheetCnt = intSheetCnt * -1;

            //COPY NEW SHEETS NEEDED
            foreach (DataRow dr in dtMain.Rows)
            {

                strYQ = dr["file_year"].ToString() + dr["file_quarter"].ToString();
                if (strYQLatest == null)
                    strYQLatest = strYQ;

                try 
                {
                    var t = wb.Worksheet(strYQ + " Summary");
                }
                catch(Exception)
                {
                    wsSource = wb.Worksheet("YQ Summary");
                    // Copy the worksheet to a new sheet in this workbook
                    wsSource.CopyTo(strYQ + " Summary").SetTabColor(XLColor.Orange);
                }

                try
                {
                    var t = wb.Worksheet("RAD " + strYQ);
                }
                catch (Exception)
                {
                    wsSource = wb.Worksheet("RAD YQ");
                    // Copy the worksheet to a new sheet in this workbook
                    wsSource.CopyTo("RAD " + strYQ);
                }

                try
                {
                    var t = wb.Worksheet("CARD " + strYQ);
                }
                catch (Exception)
                {
                    wsSource = wb.Worksheet("CARD YQ");
                    // Copy the worksheet to a new sheet in this workbook
                    wsSource.CopyTo("CARD " + strYQ);
                }



                try
                {
                    var t = wb.Worksheet("MD RAD " + strYQ);
                }
                catch (Exception)
                {
                    wsSource = wb.Worksheet("MD RAD YQ");
                    // Copy the worksheet to a new sheet in this workbook
                    wsSource.CopyTo("MD RAD " + strYQ);
                }

                try
                {
                    var t = wb.Worksheet("MD CARD " + strYQ);
                }
                catch (Exception)
                {
                    wsSource = wb.Worksheet("MD CARD YQ");
                    // Copy the worksheet to a new sheet in this workbook
                    wsSource.CopyTo("MD CARD " + strYQ);
                }


            }


            //POPULATE NEW SHEETS
            int intRowCnt = 3;
            string strChar = "";
            string strCurrentStartChar = "";
            bool blFirstPass = true;
            DataRow[] drCurrent;
            strYQ = null;
            foreach (DataRow drM in dtMain.Rows)
            {


                #region RAD YQ
                if (strYQ != drM["file_year"].ToString() + drM["file_quarter"].ToString())
                {
                    strYQ = drM["file_year"].ToString() + drM["file_quarter"].ToString();
                }


                wsSource = wb.Worksheet("RAD " + strYQ);

                drCurrent = dtRad.Select("file_year = '" + drM["file_year"] + "' AND  file_month = '" + drM["file_month"] + "'");
                //NEW DATA START AT TOP!
                intRowCnt = 3;
                //EXCEL DATE HEADERS
                if (int.TryParse(drM["file_month"].ToString(), out int intTest))
                {
                    if (intTest == 1 || intTest == 4 || intTest == 7 || intTest == 10)
                    {
                        //NEW SHEET RESET CELL
                        strCurrentStartChar = "C";
                        wsSource.Cell("C1").Value = drM["file_year"] + "_" + drM["file_month"];
                    }
                    else if (intTest == 2 || intTest == 5 || intTest == 8 || intTest == 11)
                    {
                        //NEW SHEET RESET CELL
                        strCurrentStartChar = "BB";
                        wsSource.Cell("BB1").Value = drM["file_year"] + "_" + drM["file_month"];
                    }
                    else if (intTest == 3 || intTest == 6 || intTest == 9 || intTest == 12)
                    {
                        strCurrentStartChar = "DA";
                        wsSource.Cell("DA1").Value = drM["file_year"] + "_" + drM["file_month"];
                    }
                }

                foreach (DataRow dr in drCurrent)
                {
                    
                    //if (dr["ratio"].ToString().ToLower().Equals("1-routine_cases") || dr["ratio"].ToString().ToLower().Equals("4-requestsper1000"))
                    //if (dr["ratio"].ToString().ToLower().Equals("3-fax"))
                    //{
                    //    intRowCnt++;
                    //}
                    strChar = strCurrentStartChar;


                    //wsSource.Cell(strChar + intRowCnt).Value = dr["AZ"].ToString();
                    //strChar = Increment(strChar);
                    //wsSource.Cell(strChar + intRowCnt).Value = dr["FL"].ToString();
                    //strChar = Increment(strChar);
                    //wsSource.Cell(strChar + intRowCnt).Value = dr["LA"].ToString();
                    //strChar = Increment(strChar);
                    //wsSource.Cell(strChar + intRowCnt).Value = dr["MD"].ToString();
                    //strChar = Increment(strChar);
                    //wsSource.Cell(strChar + intRowCnt).Value = dr["MS"].ToString();
                    //strChar = Increment(strChar);
                    //wsSource.Cell(strChar + intRowCnt).Value = dr["NJ"].ToString();
                    //strChar = Increment(strChar);
                    //wsSource.Cell(strChar + intRowCnt).Value = dr["NY"].ToString();
                    //strChar = Increment(strChar);
                    //wsSource.Cell(strChar + intRowCnt).Value = dr["OH"].ToString();
                    //strChar = Increment(strChar);
                    //wsSource.Cell(strChar + intRowCnt).Value = dr["PA"].ToString();
                    //strChar = Increment(strChar);
                    //wsSource.Cell(strChar + intRowCnt).Value = dr["RI"].ToString();
                    //strChar = Increment(strChar);
                    //wsSource.Cell(strChar + intRowCnt).Value = dr["TN"].ToString();
                    //strChar = Increment(strChar);
                    //wsSource.Cell(strChar + intRowCnt).Value = dr["TX"].ToString();
                    //strChar = Increment(strChar);
                    //wsSource.Cell(strChar + intRowCnt).Value = dr["WA"].ToString();
                    //strChar = Increment(strChar);
                    //wsSource.Cell(strChar + intRowCnt).Value = dr["WI"].ToString();
                    //strChar = Increment(strChar);
                    //wsSource.Cell(strChar + intRowCnt).Value = dr["MO"].ToString();
                    //strChar = Increment(strChar);
                    //wsSource.Cell(strChar + intRowCnt).Value = dr["VA"].ToString();
                    //strChar = Increment(strChar);
                    //wsSource.Cell(strChar + intRowCnt).Value = dr["CA"].ToString();
                    //strChar = Increment(strChar);
                    //wsSource.Cell(strChar + intRowCnt).Value = dr["KY"].ToString();
                    //strChar = Increment(strChar);
                    //wsSource.Cell(strChar + intRowCnt).Value = dr["NC"].ToString();

                    wsSource.Cell(strChar + intRowCnt).Value = dr["AL"].ToString(); strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["AK"].ToString(); strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["AZ"].ToString(); strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["AR"].ToString(); strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["CA"].ToString(); strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["CO"].ToString(); strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["CT"].ToString(); strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["DC"].ToString(); strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["DE"].ToString(); strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["FL"].ToString(); strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["GA"].ToString(); strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["HI"].ToString(); strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["ID"].ToString(); strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["IL"].ToString(); strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["IN"].ToString(); strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["IA"].ToString(); strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["KS"].ToString(); strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["KY"].ToString(); strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["LA"].ToString(); strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["ME"].ToString(); strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["MD"].ToString(); strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["MA"].ToString(); strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["MI"].ToString(); strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["MN"].ToString(); strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["MS"].ToString(); strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["MO"].ToString(); strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["MT"].ToString(); strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["NE"].ToString(); strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["NV"].ToString(); strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["NH"].ToString(); strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["NJ"].ToString(); strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["NM"].ToString(); strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["NY"].ToString(); strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["NC"].ToString(); strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["ND"].ToString(); strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["OH"].ToString(); strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["OK"].ToString(); strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["OR"].ToString(); strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["PA"].ToString(); strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["RI"].ToString(); strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["SC"].ToString(); strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["SD"].ToString(); strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["TN"].ToString(); strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["TX"].ToString(); strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["UT"].ToString(); strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["VT"].ToString(); strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["VA"].ToString(); strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["WA"].ToString(); strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["WV"].ToString(); strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["WI"].ToString(); strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["WY"].ToString(); strChar = Increment(strChar);



                    //if (dr["ratio"].ToString().ToLower().Equals("3-fax"))
                    //{
                    //    intRowCnt++;
                    //}



                    if (dr["ratio"].ToString().ToLower().Contains("approvalsper1000"))
                    {
                        intRowCnt = intRowCnt + 4;
                    }
                    else if (dr["ratio"].ToString().ToLower().Contains("others") || dr["ratio"].ToString().ToLower().Contains("fax"))
                    {
                        intRowCnt = intRowCnt + 2;
                    }
                    else
                        intRowCnt++;

                }



                #endregion

                #region MD RAD YQ

               
                if (intTest == 3 || intTest == 6 || intTest == 9 || intTest == 12 || blFirstPass)
                {
                    wsSource = wb.Worksheet("MD RAD " + strYQ);
                    wsSource.Cell("C1").Value = drM["file_year"].ToString() + "_" + (intTest -2);
                    wsSource.Cell("D1").Value = drM["file_year"].ToString() + "_" + (intTest - 1);
                    wsSource.Cell("E1").Value = drM["file_year"].ToString() + "_" + intTest;
                    for (int i= 3; i <= 100; i++)
                    {
                        wsSource.Cell("C" + i).FormulaA1 = wsSource.Cell("C" + i).FormulaA1.ToString().Replace("YQ", strYQ);
                        wsSource.Cell("D" + i).FormulaA1 = wsSource.Cell("D" + i).FormulaA1.ToString().Replace("YQ", strYQ);
                        wsSource.Cell("E" + i).FormulaA1 = wsSource.Cell("E" + i).FormulaA1.ToString().Replace("YQ", strYQ);
                    }

                }


                #endregion


                #region CARD YQ
                wsSource = wb.Worksheet("CARD " + strYQ);

                drCurrent = dtCard.Select("file_year = '" + drM["file_year"] + "' AND  file_month = '" + drM["file_month"] + "'");
                //NEW DATA START AT TOP!
                intRowCnt = 3;
                //EXCEL DATE HEADERS
                if (intTest == 1 || intTest == 4 || intTest == 7 || intTest == 10)
                {
                    //NEW SHEET RESET CELL
                    strCurrentStartChar = "C";
                    wsSource.Cell("C1").Value = drM["file_year"] + "_" + drM["file_month"];
                }
                else if (intTest == 2 || intTest == 5 || intTest == 8 || intTest == 11)
                {
                    //NEW SHEET RESET CELL
                    strCurrentStartChar = "BB";
                    wsSource.Cell("BB1").Value = drM["file_year"] + "_" + drM["file_month"];
                }
                else if (intTest == 3 || intTest == 6 || intTest == 9 || intTest == 12)
                {
                    strCurrentStartChar = "DA";
                    wsSource.Cell("DA1").Value = drM["file_year"] + "_" + drM["file_month"];
                }
            

                foreach (DataRow dr in drCurrent)
                {

                    //if (dr["ratio"].ToString().ToLower().Equals("1-routine_cases") || dr["ratio"].ToString().ToLower().Equals("4-requestsper1000"))
                    //{
                    //    intRowCnt++;
                    //}
                    strChar = strCurrentStartChar;

                    //wsSource.Cell(strChar + intRowCnt).Value = dr["MD"].ToString();
                    //strChar = Increment(strChar);
                    //wsSource.Cell(strChar + intRowCnt).Value = dr["MS"].ToString();
                    //strChar = Increment(strChar);
                    //wsSource.Cell(strChar + intRowCnt).Value = dr["NJ"].ToString();
                    //strChar = Increment(strChar);
                    //wsSource.Cell(strChar + intRowCnt).Value = dr["NY"].ToString();
                    //strChar = Increment(strChar);
                    //wsSource.Cell(strChar + intRowCnt).Value = dr["RI"].ToString();
                    //strChar = Increment(strChar);
                    //wsSource.Cell(strChar + intRowCnt).Value = dr["TN"].ToString();
                    //strChar = Increment(strChar);
                    //wsSource.Cell(strChar + intRowCnt).Value = dr["TX"].ToString();
                    //strChar = Increment(strChar);
                    //wsSource.Cell(strChar + intRowCnt).Value = dr["WA"].ToString();
                    //strChar = Increment(strChar);
                    //wsSource.Cell(strChar + intRowCnt).Value = dr["PA"].ToString();
                    //strChar = Increment(strChar);
                    //wsSource.Cell(strChar + intRowCnt).Value = dr["AZ"].ToString();
                    //strChar = Increment(strChar);
                    //wsSource.Cell(strChar + intRowCnt).Value = dr["MO"].ToString();
                    //strChar = Increment(strChar);
                    //wsSource.Cell(strChar + intRowCnt).Value = dr["VA"].ToString();
                    //strChar = Increment(strChar);
                    //wsSource.Cell(strChar + intRowCnt).Value = dr["KY"].ToString();
                    //strChar = Increment(strChar);
                    //wsSource.Cell(strChar + intRowCnt).Value = dr["NC"].ToString();

                    wsSource.Cell(strChar + intRowCnt).Value = dr["AL"].ToString();strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["AK"].ToString();strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["AZ"].ToString();strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["AR"].ToString();strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["CA"].ToString();strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["CO"].ToString();strChar = Increment(strChar);
                     wsSource.Cell(strChar + intRowCnt).Value = dr["CT"].ToString();strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["DC"].ToString();strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["DE"].ToString();strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["FL"].ToString();strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["GA"].ToString();strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["HI"].ToString();strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["ID"].ToString();strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["IL"].ToString();strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["IN"].ToString();strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["IA"].ToString();strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["KS"].ToString();strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["KY"].ToString();strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["LA"].ToString();strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["ME"].ToString();strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["MD"].ToString();strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["MA"].ToString();strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["MI"].ToString();strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["MN"].ToString();strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["MS"].ToString();strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["MO"].ToString();strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["MT"].ToString();strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["NE"].ToString();strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["NV"].ToString();strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["NH"].ToString();strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["NJ"].ToString();strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["NM"].ToString();strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["NY"].ToString();strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["NC"].ToString();strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["ND"].ToString();strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["OH"].ToString();strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["OK"].ToString();strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["OR"].ToString();strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["PA"].ToString();strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["RI"].ToString();strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["SC"].ToString();strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["SD"].ToString();strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["TN"].ToString();strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["TX"].ToString();strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["UT"].ToString();strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["VT"].ToString();strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["VA"].ToString();strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["WA"].ToString();strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["WV"].ToString();strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["WI"].ToString();strChar = Increment(strChar);
                    wsSource.Cell(strChar + intRowCnt).Value = dr["WY"].ToString();strChar = Increment(strChar);



                    //if (dr["ratio"].ToString().ToLower().Equals("5-approvalsper1000"))
                    //{
                    //    intRowCnt = intRowCnt + 4;
                    //}
                    //else
                    //    intRowCnt++;



                    if (dr["ratio"].ToString().ToLower().Contains("approvalsper1000"))
                    {
                        intRowCnt = intRowCnt + 4;
                    }
                    else if (dr["ratio"].ToString().ToLower().Contains("others") || dr["ratio"].ToString().ToLower().Contains("fax"))
                    {
                        intRowCnt = intRowCnt + 2;
                    }
                    else
                        intRowCnt++;




                }


                #endregion


                #region MD CARD YQ
                if (intTest == 3 || intTest == 6 || intTest == 9 || intTest == 12 || blFirstPass)
                {
                    wsSource = wb.Worksheet("MD CARD " + strYQ);
                    wsSource.Cell("C1").Value = drM["file_year"].ToString() + "_" + (intTest - 2);
                    wsSource.Cell("D1").Value = drM["file_year"].ToString() + "_" + (intTest - 1);
                    wsSource.Cell("E1").Value = drM["file_year"].ToString() + "_" + intTest;
                    for (int i = 3; i <= 71; i++)
                    {
                        wsSource.Cell("C" + i).FormulaA1 = wsSource.Cell("C" + i).FormulaA1.ToString().Replace("YQ", strYQ);
                        wsSource.Cell("D" + i).FormulaA1 = wsSource.Cell("D" + i).FormulaA1.ToString().Replace("YQ", strYQ);
                        wsSource.Cell("E" + i).FormulaA1 = wsSource.Cell("E" + i).FormulaA1.ToString().Replace("YQ", strYQ);
                    }

                }
                #endregion



                #region YQ Summary
                if (intTest == 3 || intTest == 6 || intTest == 9 || intTest == 12 || blFirstPass)
                {
                    wsSource = wb.Worksheet(strYQ + " Summary");
                    wsSource.Cell("B4").Value = wsSource.Cell("B4").Value.ToString().Replace("YQ", strYQ.Substring(0, 4) + " " + strYQ.Substring(4, 2));
                    wsSource.Cell("B8").Value = wsSource.Cell("B8").Value.ToString().Replace("YQ", strYQ.Substring(0, 4) + " " + strYQ.Substring(4, 2));
                    wsSource.Cell("B18").Value = wsSource.Cell("B18").Value.ToString().Replace("YQ", strYQ.Substring(0, 4) + " " + strYQ.Substring(4, 2));

                    wsSource.Cell("G8").Value = wsSource.Cell("G8").Value.ToString().Replace("YQ", strYQ.Substring(0, 4) + " " + strYQ.Substring(4, 2));
                    wsSource.Cell("G18").Value = wsSource.Cell("G18").Value.ToString().Replace("YQ", strYQ.Substring(0, 4) + " " + strYQ.Substring(4, 2));

                    for (int i = 11; i <= 31; i++)
                    {
                        wsSource.Cell("C" + i).FormulaA1 = wsSource.Cell("C" + i).FormulaA1.ToString().Replace("YQ", strYQ);
                        wsSource.Cell("D" + i).FormulaA1 = wsSource.Cell("D" + i).FormulaA1.ToString().Replace("YQ", strYQ);
                        wsSource.Cell("H" + i).FormulaA1 = wsSource.Cell("H" + i).FormulaA1.ToString().Replace("YQ", strYQ);
                        wsSource.Cell("I" + i).FormulaA1 = wsSource.Cell("I" + i).FormulaA1.ToString().Replace("YQ", strYQ);
                    }

                }
                #endregion



                blFirstPass = false;
            }



            wb.Worksheet("YQ Summary").Delete();
            wb.Worksheet("RAD YQ").Delete();
            wb.Worksheet("CARD YQ").Delete();
            wb.Worksheet("MD RAD YQ").Delete();
            wb.Worksheet("MD CARD YQ").Delete();



            int intPosition = 1;
            //REORDER NEW SHEETS
            foreach (IXLWorksheet worksheet in wb.Worksheets)
            {
                foreach (DataRow dr in dtMain.Rows)
                {
                    strYQ = dr["file_year"].ToString() + dr["file_quarter"].ToString();
                    if (worksheet.Name.ToString().Contains(strYQ))
                    {
                        worksheet.Position = intPosition;
                        intPosition++;
                        break;
                    }

                }
            }



            wsSource = wb.Worksheet(strYQLatest + " Summary");
            DataTable dt = getSummaryValues(wsSource);
    
            //// We're going to open another workbook to show that you can
            //// copy a sheet from one workbook to another:
            //var wbSource = new XLWorkbook("BasicTable.xlsx");
            //wbSource.Worksheet(1).CopyTo(wb, "Copy From Other");
            string strFinal = @"C:\Users\cgiorda\Desktop\Projects\CS_Scorecard_Reports\";
            // Save the workbook with the 2 copies
            wb.SaveAs(strFinal + "CS_Report_" + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + ".xlsx");



            List<string> lstNames = new List<string>();
            //LOOP DELETE
            foreach (IXLWorksheet ws in wb.Worksheets)
            {
                lstNames.Add(ws.Name);

            }

            foreach (string s in lstNames)
            {

                if (s != strYQLatest + " Summary")
                    wb.Worksheet(s).Delete();
            }




            wsSource = wb.Worksheet(strYQLatest + " Summary");
            foreach(DataRow d in dt.Rows)
            {
                wsSource.Cell(d["RequestsCell"].ToString()).Value = d["Requests"].ToString();
                wsSource.Cell(d["ApprovedCell"].ToString()).Value = d["Approved"].ToString();
            }


            wb.SaveAs(strFinal + "CS_Report_FINAL" + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + ".xlsx");



            //var wbSource = new XLWorkbook();
            //wsSource = wbSource.Worksheet(0);
            //wbSource.Worksheets.Add(strYQLatest + " Summary");

            //var wstmp = wbSource.Worksheets.Add(strYQLatest + " Summary");
            //wbSource.Worksheet(0).CopyTo(wb, strYQLatest + " Summary");
            //wbSource.SaveAs(strFinal + "CS_Report_FINAL" + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + ".xlsx");
        }



        public static string Increment(string input)
        {
            List<char> chars = input.ToList();

            // Loop over the characters in the string, backwards
            for (int i = chars.Count - 1; i >= 0; i--)
            {
                if (chars[i] < 'A' || chars[i] > 'Z')
                {
                    throw new ArgumentException("Input must contain only A-Z", nameof(input));
                }

                // Increment this character
                chars[i]++;

                if (chars[i] > 'Z')
                {
                    // Oops, we overflowed past Z. Set it back to A, and ...
                    chars[i] = 'A';

                    // ... if this is the first character in the string, add a 'A' preceeding it
                    if (i == 0)
                    {
                        chars.Add('A');
                    }
                    // ... otherwise we'll continue looping, and increment the next character on
                    // the next loop iteration
                }
                else
                {
                    // If we didn't overflow, we're done. Stop looping.
                    break;
                }
            }

            return string.Concat(chars);
        }


        static void GenerateReportInterop(System.Data.DataTable dt = null)
        {
            string strPath = AppDomain.CurrentDomain.BaseDirectory + "template\\CS_Reporting_Template.xlsx";


            Application xlApp = new Application();
            Workbook workbook = xlApp.Workbooks.Open(strPath);


            Worksheet worksheet = workbook.Worksheets["RAD YQ"];
            worksheet = workbook.Worksheets["CARD YQ"];
            worksheet = workbook.Worksheets["MD RAD YQ"];
            worksheet = workbook.Worksheets["MD CARD YQ"];
            xlApp.Visible = true;
            for (int i = 1; i <= 10; i++)
            {
                worksheet.Cells.Range["A" + i].Value = "Hello";
            }
            workbook.SaveAs(@"C:\Users\cgiorda\Desktop\CS Report Test\CS_Report_" + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + ".xlsb");
            workbook.Close();
            xlApp.Quit();

            // NOTE: No calls the Marshal.ReleaseComObject() are ever needed
        }



        
        static string getDrivingSQL( bool isRad = true)
        {
            //string strStates = "AZ,FL,LA,MD,MS,NJ,NY,OH,PA,RI,TN,TX,WA,WI,MO,VA,CA,KY,NC";
            string strStates = "[AL],[AK],[AZ],[AR],[CA],[CO],[CT],[DC],[DE],[FL],[GA],[HI],[ID],[IL],[IN],[IA],[KS],[KY],[LA],[ME],[MD],[MA],[MI],[MN],[MS],[MO],[MT],[NE],[NV],[NH],[NJ],[NM],[NY],[NC],[ND],[OH],[OK],[OR],[PA],[RI],[SC],[SD],[TN],[TX],[UT],[VT],[VA],[WA],[WV],[WI],[WY]";
            string strSheetName = "RAD"; 
            if(!isRad)
            {
                //strStates = "MD,MS,NJ,NY,RI,TN,TX,WA,PA,AZ,MO,VA,KY,NC";
                strSheetName = "CARD";
            }

            StringBuilder sbSQL = new StringBuilder();

            sbSQL.Append(" select[file_year],[file_month], 1 as orderby, [Modality], ratio = col, ");
sbSQL.Append(strStates);
            sbSQL.Append(" from ");
            sbSQL.Append("( ");
            sbSQL.Append(" select[file_year], [file_month], [Modality], [State], col, value ");
            sbSQL.Append(" FROM (SELECT* FROM [IL_UCA].[stg].[EviCore_CS_Scorecard]  WHERE[sheet_name] = '"+ strSheetName + "' AND[Modality] = 'ALL') tmp ");
            sbSQL.Append(" cross apply ");
            sbSQL.Append("( ");

            if (!isRad)
            {
                sbSQL.Append(" select '1-Phone', [Phone]  union all select '2-Web', [Web]  union all select '3-Fax', [Fax] union all select '4-RequestsPer1000', [RequestsPer1000]  union all select '5-ApprovalsPer1000', [ApprovalsPer1000]");
                sbSQL.Append("  union all select '6-Approved', [Approved]  union all select '7-Auto_Approved', [Auto_Approved]  union all select '8-Denied', [Denied] union all select '999-Withdrawn', [Withdrawn] union all select '99-Expired', [Expired] union all select '9-Others', [Others]");
            }
            else
            {
                sbSQL.Append(" select '1-Phone', [Phone]  union all select '2-Web', [Web]  union all select '3-Fax', [Fax] union all select '4-RequestsPer1000', [RequestsPer1000]  union all select '5-ApprovalsPer1000', [ApprovalsPer1000]");
                sbSQL.Append("  union all select '6-Approved', [Approved] union all select '7-Denied', [Denied] union all select '8-Withdrawn', [Withdrawn] union all select '99-Expired', [Expired] union all select '9-Others', [Others]");
            }

            sbSQL.Append(") c(col, value) ");
            sbSQL.Append(") d ");
            sbSQL.Append(" pivot ");
            sbSQL.Append("( ");
            sbSQL.Append(" max(value) ");
            sbSQL.Append(" for [State] in (" + strStates + ") ");
            sbSQL.Append(") piv ");

            sbSQL.Append(" UNION ALL ");
            sbSQL.Append(" select[file_year],[file_month],2 as orderby, [Modality], ratio = col, ");
            sbSQL.Append(strStates);
            sbSQL.Append(" from ");
            sbSQL.Append("( ");
            sbSQL.Append(" select[file_year], [file_month], [Modality], [State], col, value ");
            sbSQL.Append(" FROM (SELECT* FROM [IL_UCA].[stg].[EviCore_CS_Scorecard]  WHERE[sheet_name] = '" + strSheetName + "' AND[Modality] <> 'ALL') tmp ");
            sbSQL.Append(" cross apply ");
            sbSQL.Append("( ");


            if (!isRad)
            {
                sbSQL.Append(" select '1-Approved', [Approved]  union all select '2-Auto_Approved', [Auto_Approved]   union all select '3-Denied', [Denied] union all select '4-Withdrawn', [Withdrawn] union all select '5-Expired', [Expired] union all select '6-Others', [Others] ");
            }
            else
            {
                sbSQL.Append(" select '1-Approved', [Approved] union all select '2-Denied', [Denied] union all select '3-Withdrawn', [Withdrawn] union all select '4-Expired', [Expired] union all select '5-Others', [Others] ");
            }

            sbSQL.Append(") c(col, value) ");
            sbSQL.Append(") d ");
            sbSQL.Append(" pivot ");
            sbSQL.Append("( ");
            sbSQL.Append(" max(value) ");
            sbSQL.Append(" for [State] in ("+ strStates + ") ");
            sbSQL.Append(") piv ");

            sbSQL.Append(" UNION ALL ");

            sbSQL.Append(" select[file_year],[file_month],3 as orderby, [Modality], ratio = col, ");
            sbSQL.Append(strStates);
            sbSQL.Append(" from ");
            sbSQL.Append("( ");
            sbSQL.Append(" select[file_year], [file_month], [Modality], [State], col, value ");
            sbSQL.Append(" FROM (SELECT* FROM [IL_UCA].[stg].[EviCore_CS_Scorecard]  WHERE[sheet_name] = '" + strSheetName + "' AND[Modality] = 'ALL') tmp ");
            sbSQL.Append(" cross apply ");
            sbSQL.Append("( ");
            sbSQL.Append(" select '1-Routine_Cases', [Routine_Cases]  union all select '2-Compliant_Routine_Cases', [Compliant_Routine_Cases]  union all select '3-Urgent_Cases', [Urgent_Cases] union all select '4-Compliant_Urgent_Cases', [Compliant_Urgent_Cases] ");

            sbSQL.Append(") c(col, value) ");
            sbSQL.Append(") d ");
            sbSQL.Append(" pivot ");
            sbSQL.Append("( ");
            sbSQL.Append("  max(value) ");
            sbSQL.Append(" for [State] in (" + strStates + ") ");
            sbSQL.Append(") piv ");
            sbSQL.Append(" ORDER BY[file_year] DESC,[file_month] DESC,orderby, [Modality],ratio ");


            return sbSQL.ToString();


        }


        private static DataTable getSummaryValues(IXLWorksheet wsSource)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Requests");
            dt.Columns.Add("RequestsCell");
            dt.Columns.Add("Approved");
            dt.Columns.Add("ApprovedCell");
            DataRow d = null;

            for(int i = 11; i <=15;i++)
            {
                d = dt.NewRow();
                d["Requests"] = wsSource.Row(i).Cell(3).Value;
                d["RequestsCell"] = "C" + i;
                d["Approved"] = wsSource.Row(i).Cell(4).Value;
                d["ApprovedCell"] = "D" + i;
                dt.Rows.Add(d);

                d = dt.NewRow();
                d["Requests"] = wsSource.Row(i).Cell(8).Value;
                d["RequestsCell"] = "H" + i;
                d["Approved"] = wsSource.Row(i).Cell(9).Value;
                d["ApprovedCell"] = "I" + i;
                dt.Rows.Add(d);

            }


            for (int i = 21; i <= 30; i++)
            {
                d = dt.NewRow();
                d["Requests"] = wsSource.Row(i).Cell(3).Value;
                d["RequestsCell"] = "C" + i;
                d["Approved"] = wsSource.Row(i).Cell(4).Value;
                d["ApprovedCell"] = "D" + i;
                dt.Rows.Add(d);

                d = dt.NewRow();
                d["Requests"] = wsSource.Row(i).Cell(8).Value;
                d["RequestsCell"] = "H" + i;
                d["Approved"] = wsSource.Row(i).Cell(9).Value;
                d["ApprovedCell"] = "I" + i;
                dt.Rows.Add(d);

            }

            return dt;
        }


    }
}
