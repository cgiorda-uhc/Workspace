using ClosedXML.Excel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace IR_SAS_QA
{
    class IR_SAS_QA
    {

        //MISSING NDB SPECIALTY = inner join  PH34.UHN_MAY6_DEMOG as d on d.mpin=a.attr_mpin


        static void Main(string[] args)
        {

            string strPlaceHolder = null;
            string strSampleCSV = null;
            string strMeasureName = null;
            string strFinalXLPath = null;
            string strType = null;
            string strSQL = null;
            int intSampleSize;

            DataTable dtResults = null;
            DataTable dtPathMap = null;
            DataTable dtSampleMap = null;
            DataTable dtQueryMap = null;

            OleDbDataReader oleDr = null;

            string strILUCA_ConnectionString = ConfigurationManager.AppSettings["ILUCA_Database"];
            string strUGAP_ConnectionString = ConfigurationManager.AppSettings["UGAP_Database"];
            string strUHN_ConnectionString = ConfigurationManager.AppSettings["UHN_Database"];

            string strInputPath = ConfigurationManager.AppSettings["SAS_QA_Map_Input_Path"];
            string strOutputPath = ConfigurationManager.AppSettings["SAS_QA_Map_Output_Path"];

            IR_SAS_Connect.strSASHost = ConfigurationManager.AppSettings["SAS_Host"];
            IR_SAS_Connect.intSASPort = int.Parse(ConfigurationManager.AppSettings["SAS_Port"]);
            IR_SAS_Connect.strSASClassIdentifier = ConfigurationManager.AppSettings["SAS_ClassIdentifier"];
            IR_SAS_Connect.strSASUserName = ConfigurationManager.AppSettings["SAS_UN"];
            IR_SAS_Connect.strSASPassword = ConfigurationManager.AppSettings["SAS_PW"];
            IR_SAS_Connect.strSASUserNameUnix = ConfigurationManager.AppSettings["SAS_UN_Unix"];
            IR_SAS_Connect.strSASPasswordUnix = ConfigurationManager.AppSettings["SAS_PW_Unix"];

            try
            {




                //dtStartTime = DateTime.Now;
                ////TRANSFER PAIR.ACO_Exec_FullPositiveRegistry_OHP DATA TO ILUCA.ACO_COMM
                //SQLServerBulkImport(strACO_PIAR_ConnectionString, strILUCA_ConnectionString, strSQL, "ACO_COMM"); //BULK DATA LOAD
                //dtEndTime = DateTime.Now;
                //tsTimeSpan = dtEndTime.Subtract(dtStartTime);
                //strTimeMessage = (tsTimeSpan.Hours == 0 ? "" : tsTimeSpan.Hours + "hr:") + (tsTimeSpan.Minutes == 0 ? "" : tsTimeSpan.Minutes + "min:") + (tsTimeSpan.Seconds == 0 ? "" : tsTimeSpan.Seconds + "sec");
                //Console.Write("\r" + strMessageGlobal.Replace("{$rowCnt}", String.Format("{0:n0}", intResultCnt)).Replace("...", ""));

                string strDateTime = null;
                string[] files = Directory.GetFiles(strInputPath, "*.xlsx", SearchOption.TopDirectoryOnly);
                foreach (string sFile in files)
                {

                    strDateTime = DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss");

                    var strFileName = Path.GetFileName(sFile).ToLower();
                    var strFinalPath = strOutputPath + "\\" + strFileName.Replace(".xlsx", "") + "_" + strDateTime + "\\";
                    if (Directory.Exists(strFinalPath))
                    {
                        Directory.Delete(strFinalPath);
                    }

                    Console.WriteLine("Gathering SAS QA Mapping...");
                    var workbook = new XLWorkbook(sFile);
                    dtPathMap = workbook.Worksheet("AliasPath").RangeUsed().AsTable().AsNativeDataTable();
                    dtSampleMap = workbook.Worksheet("Samples").RangeUsed().AsTable().AsNativeDataTable();
                    dtQueryMap = workbook.Worksheet("Queries").RangeUsed().AsTable().AsNativeDataTable();

                    Console.WriteLine("Connecting to SAS Server...");
                    IR_SAS_Connect.create_SAS_instance(dtPathMap);

                    foreach (DataRow drSample in dtSampleMap.Rows)
                    {

                        try
                        {
                            strPlaceHolder = drSample["Placeholder"].ToString();
                            strMeasureName = drSample["Measure"].ToString();

                            if (!int.TryParse(drSample["SampleSize"].ToString(), out intSampleSize))
                                intSampleSize = 5;

                            strSQL = drSample["Query"].ToString().Replace("\n"," ").Trim().Replace("  ", " ");
                            Console.WriteLine(strMeasureName + ": Collecting samples...");
                            oleDr = DBConnection32.getOleDbDataReader(IR_SAS_Connect.strSASConnectionString, strSQL);
                            strSampleCSV = getCSV_Sampling(oleDr, intSampleSize, "attr_mpin");
                            workbook = new XLWorkbook();
                            foreach (DataRow dr in dtQueryMap.Select("Measure = '" + strMeasureName + "'"))
                            {

                                try
                                {
                                    strType = dr["Type"].ToString();
                                    strSQL = dr["Query"].ToString().Replace("\n", " ").Replace(strPlaceHolder, strSampleCSV).Trim().Replace("  ", " ");
                                    Console.WriteLine(strMeasureName + ": Getting data for " + strType + " sheet...");
                                    dtResults = DBConnection32.getOleDbDataTable(IR_SAS_Connect.strSASConnectionString, strSQL);
                                    if (dtResults == null)
                                    {
                                        var w = workbook.Worksheets.Add(strType);
                                        w.Cell("A1").Comment.AddText(strSQL);
                                        w.SetTabColor(XLColor.Purple);
                                    }
                                    else
                                    {
                                        dtResults.TableName = strType;
                                        var ws = workbook.Worksheets.Add(dtResults);
                                        ws.Cell("A1").Comment.AddText(strSQL);
                                        //ws.Cell("A1").Comment.Style.Alignment.SetAutomaticSize();
                                        ws.Cell("A1").Comment.Style.Size.SetHeight(72); // The height is set in the same units as row.Height
                                        ws.Cell("A1").Comment.Style.Size.SetWidth(123); // The width is set in the same units as row.Width
                                    }
                                }
                                catch (Exception ex)
                                {
                                   // var w = workbook.Worksheets.Add(strType);
                                    //w.Cell("A1").Comment.AddText(strSQL);
                                    //w.Cell(1, 1).Value = "ERROR";
                                    //w.Cell(1, 2).Value = ex.Message;
                                    //w.Cell(2, 1).Value = "SQL";
                                    //w.Cell(2, 2).Value = strSQL;
                                    //w.Cell(3, 1).Value = "ERROR_DETAILS";
                                    //w.Cell(3, 2).Value = ex.ToString();

                                    //w.Cell(1,1).Style.Fill.BackgroundColor = XLColor.DarkRed;
                                    //w.Cell(2, 1).Style.Fill.BackgroundColor = XLColor.DarkRed;
                                    //w.Cell(3, 1).Style.Fill.BackgroundColor = XLColor.DarkRed;
                                    //w.Cell(1, 1).Style.Font.FontColor = XLColor.White;
                                    //w.Cell(2, 1).Style.Font.FontColor = XLColor.White;
                                    //w.Cell(3, 1).Style.Font.FontColor = XLColor.White;
                                    ////w.Range("A2:A2").Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                                    ////w.Range("B2:B2").Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                                    ////w.Range("C2:C2").Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

                                    //w.SetTabColor(XLColor.Red);

                                    addErrorSheet(ref workbook, ex.Message, strSQL, ex.ToString(), strType);
                                    strFinalXLPath = strFinalPath + strMeasureName.getSafeFileName() + "_ERROR_" + strDateTime + ".xlsx";
                                }

                            }

                            Console.WriteLine("Saving excel file" + strMeasureName + ".xlsx");
                            if(strFinalXLPath == null)
                                strFinalXLPath = strFinalPath + strMeasureName.getSafeFileName() + "_" + strDateTime + ".xlsx";

                        }
                        catch (Exception ex)
                        {
                            workbook = new XLWorkbook();
                            //var w = workbook.Worksheets.Add("Fatal Error");
                            ////w.Cell("A1").Comment.AddText(strSQL + "");
                            //w.Cell(1, 1).Value = "ERROR";
                            //w.Cell(1, 2).Value = ex.Message;
                            //w.Cell(2, 1).Value = "SQL";
                            //w.Cell(2, 2).Value = strSQL;
                            //w.Cell(3, 1).Value = "ERROR_DETAILS";
                            //w.Cell(3, 2).Value =  ex.ToString();

                            //w.Cell(1, 1).Style.Fill.BackgroundColor = XLColor.DarkRed;
                            //w.Cell(2, 1).Style.Fill.BackgroundColor = XLColor.DarkRed;
                            //w.Cell(3, 1).Style.Fill.BackgroundColor = XLColor.DarkRed;
                            //w.Cell(1, 1).Style.Font.FontColor = XLColor.White;
                            //w.Cell(2, 1).Style.Font.FontColor = XLColor.White;
                            //w.Cell(3, 1).Style.Font.FontColor = XLColor.White;
                            ////w.Range("A2:A2").Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                            ////w.Range("B2:B2").Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                            ////w.Range("C2:C2").Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

                            //w.SetTabColor(XLColor.Red);

                            addErrorSheet(ref workbook, ex.Message, strSQL, ex.ToString(), "Fatal Error");

                            strFinalXLPath = strFinalPath + strMeasureName.getSafeFileName() + "_FATAL_ERROR_" + strDateTime + ".xlsx";


                        }
                        finally
                        {
                            if (workbook.Worksheets.Count <= 0)
                            {
                                var w = workbook.Worksheets.Add("Queries Missing");


                                //w.Cell("A1").Comment.AddText(strSQL + "");
                                w.Cell(1, 1).Value = "ALERT";
                                w.Cell(1, 2).Value = "Cannot find any query matches for {" + strMeasureName + "}!!! Be sure the spelling is consistent between Samples.Measure and Queries.Measure";
                                w.Cell(1, 1).Style.Fill.BackgroundColor = XLColor.DarkRed;
                                w.Cell(1, 1).Style.Font.FontColor = XLColor.White;
                                w.Range("A1:A1").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                                w.Range("B1:B1").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                                w.SetTabColor(XLColor.Red);//XLColor.RedPigment
                                strFinalXLPath = strFinalPath + strMeasureName.getSafeFileName() + "_QUERIES_MISSING_" + strDateTime + ".xlsx";
                            }


                            foreach (var ws in workbook.Worksheets)
                            {
                                

                                if (ws.TabColor == XLColor.Red)
                                {
                                    ws.Cells().Style.Alignment.WrapText = true;

                                    //ws.Rows().Height = 123;
                                    ws.Cell("A2").SetActive();

                                    ws.Columns().AdjustToContents();

                                   

                                    //ws.Cell().Style.Alignment.WrapText = true;
                                }
                                else
                                {
                                    ws.Rows().Height = 20;
                                    ws.Cell("A1").SetActive();

                                    ws.Columns().AdjustToContents();
                                    ws.Tables.FirstOrDefault().ShowAutoFilter = false;
                                }


                                //ws.Rows().AdjustToContents();

                                //REMOVE AUTO FILTERS??
                                //ws.Tables.FirstOrDefault().ShowAutoFilter = false;

                            }

                            if (File.Exists(strFinalXLPath))
                                File.Delete(strFinalXLPath);

                            workbook.SaveAs(strFinalXLPath);


                            strFinalXLPath = null;
                        }
                    }


                    File.Move(sFile, strInputPath + "\\Archive\\" + strFileName.Replace(".xlsx", "_" + strDateTime + ".xlsx"));

                    Console.WriteLine("Disconnecting from SAS Server...");
                    try
                    {
                        IR_SAS_Connect.destroy_SAS_instance();
                    }
                    catch(Exception)
                    {

                    }



                    Console.WriteLine("Done!");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                if (oleDr != null)
                {
                    oleDr.Close();
                    oleDr.Dispose();

                }
                oleDr = null;


                try
                {
                    IR_SAS_Connect.destroy_SAS_instance();
                }
                catch (Exception)
                {

                }

            }


            Console.Read();
        }



        private static void addErrorSheet(ref XLWorkbook workbook, string strError, string strSQL, string strDetails, string strSheetname)
        {
            var w = workbook.Worksheets.Add(strSheetname);
            //w.Cell("A1").Comment.AddText(strSQL + "");
            w.Cell(1, 1).Value = "ERROR";
            w.Cell(1, 2).Value = strError;
            w.Cell(2, 1).Value = "SQL";
            w.Cell(2, 2).Value = strSQL;
            w.Cell(3, 1).Value = "ERROR_DETAILS";
            w.Cell(3, 2).Value = strDetails;

            w.Cell(1, 1).Style.Fill.BackgroundColor = XLColor.DarkRed;
            w.Cell(2, 1).Style.Fill.BackgroundColor = XLColor.DarkRed;
            w.Cell(3, 1).Style.Fill.BackgroundColor = XLColor.DarkRed;
            w.Cell(1, 1).Style.Font.FontColor = XLColor.White;
            w.Cell(2, 1).Style.Font.FontColor = XLColor.White;
            w.Cell(3, 1).Style.Font.FontColor = XLColor.White;
            w.Range("A1:A1").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            w.Range("A2:A2").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            w.Range("A3:A3").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            w.Range("B1:B1").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            w.Range("B2:B2").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            w.Range("B3:B3").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            w.SetTabColor(XLColor.Red);

        }

        private static string getCSV_Sampling(OleDbDataReader oleDr, int intLimit = 5, string strColumnName = "attr_mpin")
        {
            StringBuilder sbSample = new StringBuilder();
            int rowCnt = 0;

            while (oleDr.Read())
            {
                rowCnt++;
                sbSample.Append(oleDr.GetValue(0) + ",");

                if (intLimit == rowCnt)
                    break;
                //sbSample.Append(oleDr.GetValue(oleDr.GetOrdinal(strColumnName)) + ",");
                //for (int colIndex = 0; colIndex < oleDr.FieldCount; colIndex++)
                //{
                //    Console.WriteLine("Row #" + rowCnt + ": " + oleDr.GetName(colIndex) + " = " + oleDr.GetValue(colIndex));

                    //}

            }

            if (!oleDr.IsClosed)
                oleDr.Close();

            return sbSample.ToString().TrimEnd(',');

        }



    }

   

}
