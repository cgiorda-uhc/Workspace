using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ClosedXML.Excel;
using IronOcr;

namespace PDF_Parser
{
    class PDF_Parser
    {
        static Dictionary<string, string> dictionaryGLOBAL = null;
        static DataTable dtResultsGLOBAL = null;
        static string strConnectionStringGLOBAL = null;
        static string strExcelTemplatePathGLOBAL = null;
        static string strReportDestinationGLOBAL = null;
        static string strORCTextPathGLOBAL = null;
        static string strPDFPathGLOBAL = null;
        static string strPDFImagePathGLOBAL = null;
        static string strPDFPathLinksGLOBAL = null;
        static int intErrorCntGlobal = 0; 
        static void Main(string[] args)
        {
            strPDFPathGLOBAL = ConfigurationManager.AppSettings["PDFPath"];
            strPDFPathLinksGLOBAL = ConfigurationManager.AppSettings["PDFPathLinks"];
            strExcelTemplatePathGLOBAL = ConfigurationManager.AppSettings["ReportTemplate"];
            strConnectionStringGLOBAL = ConfigurationManager.AppSettings["ILUCA_Database"];
            strReportDestinationGLOBAL = ConfigurationManager.AppSettings["ReportDestination"];
            strORCTextPathGLOBAL = ConfigurationManager.AppSettings["ORCTextPath"];


            strPDFImagePathGLOBAL = ConfigurationManager.AppSettings["PDFImagePath"];



           Console.WriteLine("STARTING PDF PROCESS..");

            GenerateReportNew();

           // ProcessPDFS();





            //ProcessPDFSForTraining();






            //GenerateReportNew();

            return;


            ProcessPDFSForTraining();
            //ProcessPDFS();



            return;

            Utilities.NLP.FeaturizeText();

            testTokens();
            return;




        }

        static void GenerateReportNew()
        {


            string strSQL = "SELECT [pdf_id] ,[Admit_ICU_Status] ,[Prone_Position] ,[Ventilator] ,[Ventilator_Split] ,[Hydroxychloroquine] ,[Azithromycin] ,[Azithro_Hydroxychlor] ,[Azithro_Hydroxychl_Zinc] ,[Steroid_Use] ,[Remdesivir] ,[EIDD_2801] ,[Ceftriax_Rocephin] ,[Other_Antibiotics] ,[Zinc_Suppl] ,[Plasma_Use] ,[Hyperbaric_O2] ,[Avigan_Favipiravir] ,[Actemra_Tociliz] ,[Kevzara_Sarilumb] ,[Monteluk_Singulair] ,[Vit_C] ,[Vit_D] ,[Magnesium] ,[Anticoagulant] ,[Aspirin] ,[Atazanavir] ,[Tenofov_Lam_Riton] ,[pdf_text] ,    'file:" + strPDFPathLinksGLOBAL + "\\text_results\\' +  [pdf_id] + '.txt' as ocr_text_path ,  'file:" + strPDFPathLinksGLOBAL + "\\' +  [pdf_id] + '.pdf'  as pdf_url FROM [IL_UCA].[dbo].[covid19_pdf_mbr]  WHERE pdf_folder <> '\\\\nasv0048\\ucs_ca\\PHS_DATA_NEW\\Home Directory - COVID19\\ECAA_Documentation\\Random_Sample' ";


            DataTable dtResults = DBConnection32.getMSSQLDataTable(strConnectionStringGLOBAL, strSQL);


            DataTable dtSASDemographics = null;


            string[] strFiltersArr = dtResults.AsEnumerable().Select(r => r.Field<string>("pdf_id")).ToArray();


            dtSASDemographics = getSASData(strFiltersArr);


            if (!Directory.Exists(strPDFPathLinksGLOBAL + "\\text_results\\"))
            {
                Directory.CreateDirectory(strPDFPathLinksGLOBAL + "\\text_results\\");
            }

            foreach (DataRow dr in dtResults.Rows)
            {
                string path = strPDFPathLinksGLOBAL + "\\text_results\\" + dr["pdf_id"] + ".txt";
                if (!File.Exists(path))
                {
                    //File.Delete(path);

                    // Create a file to write to.
                    using (StreamWriter sw = File.CreateText(path))
                    {
                        sw.Write(dr["pdf_text"]);
                    }

                }
            }

            //EXCEL FUNCTION 2 tables merged!!!!
            generateExcel(dtResults, dtSASDemographics);

        }

        static void ProcessPDFS()
        {
            Infinite:

            object objExists = null;
            string strPDFID = null;
            string strText = null;


            foreach (string f in Directory.GetFiles(strPDFPathGLOBAL, "*.pdf", SearchOption.TopDirectoryOnly))
            {
                strPDFID = Path.GetFileNameWithoutExtension(f);
                objExists = DBConnection32.getMSSQLExecuteScalar(strConnectionStringGLOBAL, "SELECT 1 from covid19_pdf_mbr where pdf_id = '" + strPDFID + "' AND  pdf_folder = '" + strPDFPathGLOBAL + "'");

                if (objExists != null)
                {
                    Console.WriteLine("Already exist, skipping pdf scrub...");
                    continue;
                }

                var fi = new FileInfo(f);
                Console.WriteLine("Processing: " + f + "...");
                Console.WriteLine("FileSize: " + BytesToString(fi.Length));
                try
                {
                    //if (!f.Contains("A089982124.pdf"))
                    // var orc = new Utilities.ORC();
                    // strText = orc.PDFtoText(f);


                    //strText = Utilities.ORC_CSG.getTextFromPDF(f, null, strPDFImagePathGLOBAL, true);
                    strText = Utilities.PDF.GetTextFromAllPages(f, null, strPDFImagePathGLOBAL, true);



                    DBConnection32.ExecuteMSSQL(strConnectionStringGLOBAL, "INSERT INTO covid19_pdf_mbr (pdf_id, pdf_text, pdf_folder ) VALUES ( '" + strPDFID + "','" + strText.Replace("'", "''") + "','" + strPDFPathGLOBAL.Replace("'", "''") + "')");


                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR:" + ex.Message);
                    continue;

                }
            }
            goto Infinite;


        }






        

        static void ProcessPDFSForTraining()
        {
            Infinite:


            string strPDFID = null;
            string strText = null;
            StringBuilder sbFinal = new StringBuilder();


            string strSQL = "SELECT LTRIM(RTRIM(SRN_ECAA)) as [pdf_id] FROM [dbo].[covid19_pdf_mbr_train] WHERE  isnull(pdf_text,'') = '' IS NULL ORDER BY [pdf_id] ";

            DataTable dtTrain = DBConnection32.getMSSQLDataTable(strConnectionStringGLOBAL, strSQL);

            foreach(DataRow dr in dtTrain.Rows)
            {

                strPDFID = dr["pdf_id"].ToString();

                foreach (string f in Directory.GetFiles(strPDFPathGLOBAL, strPDFID + "_*.pdf", SearchOption.TopDirectoryOnly))
                {
                    var fi = new FileInfo(f);
                    Console.WriteLine("Processing: " + f + "...");
                    Console.WriteLine("FileSize: " + BytesToString(fi.Length));

                    if (fi.Length == 0)
                        continue;

                    try
                    {
                        //if (!f.Contains("A089982124.pdf"))
                        // var orc = new Utilities.ORC();
                        // strText = orc.PDFtoText(f);

                        sbFinal.AppendLine("-------------------------------------FILENAME="+fi.Name +"------------------------------------------------------------");



                        //strText = Utilities.ORC_CSG.getTextFromPDF(f, null, strPDFImagePathGLOBAL, true);
                        strText = Utilities.PDF.GetTextFromAllPages(f, null, strPDFImagePathGLOBAL, true);

                        sbFinal.AppendLine(strText);



                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("ERROR:" + ex.Message);
                        sbFinal.AppendLine("ERROR:" + ex.Message);
                        //continue;

                    }

                }

                if(sbFinal.Length > 0)
                {
                    DBConnection32.ExecuteMSSQL(strConnectionStringGLOBAL, "UPDATE covid19_pdf_mbr_train SET pdf_text  =  '" + sbFinal.ToString().Replace("'", "''") + "' WHERE LTRIM(RTRIM(SRN_ECAA)) =  '" + strPDFID + "';");
                    sbFinal.Remove(0, sbFinal.Length);
                }

            }


            goto Infinite;

        }





        static void testTokens()
        {

            object objExists = null;
            string strPDFID = null;
            string strFinalText = null;


            foreach (string f in Directory.GetFiles(strPDFPathLinksGLOBAL, "*.pdf", SearchOption.TopDirectoryOnly))
            {
                Console.WriteLine("Processing  = " + f + "...");
                try
                {
                    //if (!f.Contains("A089982124.pdf"))
                    //    continue;

                    strPDFID = Path.GetFileNameWithoutExtension(f);

                    objExists = DBConnection32.getMSSQLExecuteScalar(strConnectionStringGLOBAL, "SELECT pdf_text from covid19_pdf_members where pdf_id = '" + strPDFID + "'");

                    if (objExists == null)
                    {
                        Console.WriteLine("Not PDF processed yet, skipping...");
                        continue;
                    }

                    strFinalText = objExists.ToString();

                    Utilities.NLP.TextToStringTokens(strFinalText);

                }
                catch (Exception e)
                {
                    Console.WriteLine("CSG GENERAL ERROR:" + e.Message);

                }
                finally
                {

                }


            }

        }



       


        //static void GenerateReport()
        //{

        //    string strUpdateTemplate = "UPDATE covid19_pdf_members SET {$update_combo} WHERE pdf_id = '{$pdf_id}';";

        //    StringBuilder sbSQLValues = new StringBuilder();
        //    StringBuilder sbSQLColumns = new StringBuilder();
        //    StringBuilder sbSQLUpdateCombo = new StringBuilder();
        //    DataRow _results = null;

        //    DataTable dtSASDemographics = null;

        //    object objExists = null;


        //    bool blFoundMatch = false;
        //    string[] strKeywordsArr = null;
        //    string strFinalText = null;
        //    string strPDFID = null;
            

           
        //    foreach (string f in Directory.GetFiles(strPDFPathLinksGLOBAL, "*.pdf", SearchOption.TopDirectoryOnly))
        //    {
        //        Console.WriteLine("Processing  = " + f + "...");
        //        try
        //        {
        //            //if (!f.Contains("A089982124.pdf"))
        //            //    continue;

        //            strPDFID = Path.GetFileNameWithoutExtension(f);

        //            _results = dtResultsGLOBAL.NewRow();
        //            _results["FileId"] = strPDFID;
        //            _results["pdf_url"] = "file:" + f;


        //            objExists = DBConnection.getMSSQLExecuteScalar(strConnectionStringGLOBAL, "SELECT pdf_text from covid19_pdf_members where pdf_id = '" + strPDFID + "'");

        //            if(objExists == null)
        //            {
        //                Console.WriteLine("Not PDF processed yet, skipping...");
        //                continue;
        //            }
    
        //            strFinalText = objExists.ToString();
                  


        //            //POPULATE TABLE VIA dictionaryGLOBAL against SB TEXT
        //            foreach (KeyValuePair<string, string> entry in dictionaryGLOBAL)
        //            {
        //                // do something with entry.Value or entry.Key
        //                strKeywordsArr = entry.Value.Split(',');
        //                foreach (string strKeyword in strKeywordsArr)
        //                {
        //                    //string test = "";
        //                    //if (strKeyword.ToLower() == " pe ")
        //                    //    test = "PE";


        //                    blFoundMatch = strFinalText.Contains(strKeyword.ToLower());

        //                    if (blFoundMatch)
        //                        break;
        //                }


        //                sbSQLValues.Append((blFoundMatch ? 1 : 0) + ",");
        //                sbSQLColumns.Append("[" + entry.Key + "]" + ",");
        //                sbSQLUpdateCombo.Append("[" + entry.Key + "] = " + (blFoundMatch ? 1 : 0) + ",");
        //                _results[entry.Key] = blFoundMatch;

        //                blFoundMatch = false;
        //            }



                 

        //                string path = strORCTextPathGLOBAL + "\\" + strPDFID + ".txt";
        //                if (!File.Exists(path))
        //                {
        //                    //File.Delete(path);

        //                    // Create a file to write to.
        //                    using (StreamWriter sw = File.CreateText(path))
        //                    {
        //                        sw.Write(strFinalText);
        //                    }
                            
        //                }

        //            _results["ocr_text_path"] = "file:" + path;


        //            dtResultsGLOBAL.Rows.Add(_results);

                    
        //            DBConnection.ExecuteMSSQL(strConnectionStringGLOBAL, strUpdateTemplate.Replace("{$pdf_id}", strPDFID).Replace("{$update_combo}", sbSQLUpdateCombo.ToString().TrimEnd(',')));




        //        }
        //        catch (Exception e)
        //        {
        //            Console.WriteLine("CSG GENERAL ERROR:" + e.Message);

        //        }
        //        finally
        //        {
        //            sbSQLColumns.Remove(0, sbSQLColumns.Length);
        //            sbSQLValues.Remove(0, sbSQLValues.Length);
        //            sbSQLUpdateCombo.Remove(0, sbSQLUpdateCombo.Length);
        //        }


        //    }

        //    string[] strFiltersArr = dtResultsGLOBAL.AsEnumerable().Select(r => r.Field<string>("FileId")).ToArray();


        //    dtSASDemographics = getSASData(strFiltersArr);

        //    //EXCEL FUNCTION 2 tables merged!!!!
        //    generateExcel(dtResultsGLOBAL, dtSASDemographics);

        //}

        //static void PDFParser(string strPath)
        //{

        //    Start:


        //    string strInsertTemplate = "BEGIN IF NOT EXISTS (SELECT * FROM covid19_pdf_members WHERE pdf_id = '{$pdf_id}') BEGIN INSERT INTO covid19_pdf_members (pdf_id, pdf_text, {$columns} ) VALUES ( '{$pdf_id}','{$pdf_text}', {$values}) END END";


        //    string strUpdateTemplate = "UPDATE covid19_pdf_members SET {$update_combo} WHERE pdf_id = '{$pdf_id}';";

        //    StringBuilder sbSQLValues = new StringBuilder();
        //    StringBuilder sbSQLColumns = new StringBuilder();
        //    StringBuilder sbSQLUpdateCombo = new StringBuilder();
        //    DataRow _results = null;

        //    DataTable dtSASDemographics = null;

        //    object objExists = null;

        //    DateTime startTime;
        //    DateTime endTime;
        //    TimeSpan span;

        //    bool blFoundMatch = false;
        //    string[] strKeywordsArr = null;
        //    string strFinalText = null;
        //    string strPDFID = null;


        //    IronOcr.AdvancedOcr Ocr = new IronOcr.AdvancedOcr()
        //    {
        //        CleanBackgroundNoise = true,
        //        EnhanceContrast = true,
        //        EnhanceResolution = true,
        //        Language = IronOcr.Languages.English.OcrLanguagePack,
        //        Strategy = IronOcr.AdvancedOcr.OcrStrategy.Advanced,
        //        ColorSpace = AdvancedOcr.OcrColorSpace.Color,
        //        DetectWhiteTextOnDarkBackgrounds = true,
        //        InputImageType = AdvancedOcr.InputTypes.AutoDetect,
        //        RotateAndStraighten = true,
        //        ReadBarCodes = true,
        //        ColorDepth = 4
        //    };



        //    bool blTest = false;
        //    foreach (string f in Directory.GetFiles(strPath, "*.pdf", SearchOption.TopDirectoryOnly))
        //    {
        //        Console.WriteLine("Processing  = " + f + "...");
        //        try
        //        {
        //            //if (!f.Contains("A089982124.pdf"))
        //            //    continue;

        //            strPDFID = Path.GetFileNameWithoutExtension(f);

        //            _results = dtResultsGLOBAL.NewRow();
        //            _results["FileId"] = strPDFID;
        //            _results["pdf_url"] = "file:" + f;


        //            objExists = DBConnection.getMSSQLExecuteScalar(strConnectionStringGLOBAL, "SELECT pdf_text from covid19_pdf_members where pdf_id = '" + strPDFID + "'");



        //            if (objExists == null)
        //            {
        //                //continue;

        //                startTime = DateTime.Now;

        //                ////var Ocr = new IronOcr.AutoOcr();
        //                //var Ocr = new IronOcr.AdvancedOcr()
        //                //{
        //                //    CleanBackgroundNoise = true,
        //                //    EnhanceContrast = true,
        //                //    EnhanceResolution = true,
        //                //    Language = IronOcr.Languages.English.OcrLanguagePack,
        //                //    Strategy = IronOcr.AdvancedOcr.OcrStrategy.Advanced,
        //                //    ColorSpace = AdvancedOcr.OcrColorSpace.Color,
        //                //    DetectWhiteTextOnDarkBackgrounds = true,
        //                //    InputImageType = AdvancedOcr.InputTypes.AutoDetect,
        //                //    RotateAndStraighten = true,
        //                //    ReadBarCodes = true,
        //                //    ColorDepth = 4
        //                //};

                  
        //                try
        //                {
        //                    //var Results = Ocr.Read(f);
        //                    var Results = Ocr.ReadPdf(f);

        //                    //Console.WriteLine(Results.Text);

        //                    strFinalText = Results.Text.ToLower();
        //                }
        //                catch(Exception ex)
        //                {

                           
        //                        goto Start;
                         
                            
        //                }

        //                endTime = DateTime.Now;
        //                span = endTime.Subtract(startTime);
        //                Console.WriteLine("Total time  = " + (span.Hours == 0 ? "" : span.Hours + " hr, ") + (span.Minutes == 0 ? "" : span.Minutes + " min, ") + (span.Seconds == 0 ? "" : span.Seconds + " sec, ") + (span.Milliseconds == 0 ? "" : span.Milliseconds + " ms "));
        //            }
        //            else
        //            {
        //                Console.WriteLine("Already exist, skipping pdf scrub...");
        //                strFinalText = objExists.ToString();
        //            }
                        


        //            //POPULATE TABLE VIA dictionaryGLOBAL against SB TEXT
        //            foreach (KeyValuePair<string, string> entry in dictionaryGLOBAL)
        //            {
        //                // do something with entry.Value or entry.Key
        //                strKeywordsArr = entry.Value.Split(',');
        //                foreach(string strKeyword in strKeywordsArr)
        //                {
        //                    //string test = "";
        //                    //if (strKeyword.ToLower() == " pe ")
        //                    //    test = "PE";


        //                    blFoundMatch = strFinalText.Contains(strKeyword.ToLower());

        //                    if (blFoundMatch)
        //                        break;
        //                }


        //                sbSQLValues.Append((blFoundMatch ? 1 : 0) + ",");
        //                sbSQLColumns.Append("[" + entry.Key + "]" + ",");
        //                sbSQLUpdateCombo.Append("[" + entry.Key + "] = " + (blFoundMatch ? 1 : 0) + ",");
        //                _results[entry.Key] = blFoundMatch;

        //                blFoundMatch = false;
        //            }



        //            if (objExists == null)
        //            {

        //                string path = strORCTextPathGLOBAL + "\\" + strPDFID + ".txt";
        //                if (File.Exists(path))
        //                    File.Delete(path);

        //                // Create a file to write to.
        //                using (StreamWriter sw = File.CreateText(path))
        //                {
        //                    sw.Write(strFinalText);
        //                }
        //                _results["ocr_text_path"] = "file:" + path;
        //            }


        //            dtResultsGLOBAL.Rows.Add(_results);

        //            if (objExists == null)
        //                 DBConnection.ExecuteMSSQL(strConnectionStringGLOBAL, strInsertTemplate.Replace("{$pdf_id}", strPDFID).Replace("{$pdf_text}", strFinalText.Replace("'","''")).Replace("{$columns}", sbSQLColumns.ToString().TrimEnd(',')).Replace("{$values}", sbSQLValues.ToString().TrimEnd(',')));
        //            else
        //                DBConnection.ExecuteMSSQL(strConnectionStringGLOBAL, strUpdateTemplate.Replace("{$pdf_id}", strPDFID).Replace("{$update_combo}", sbSQLUpdateCombo.ToString().TrimEnd(',')));




        //        }
        //        catch (Exception e)
        //        {
        //            Console.WriteLine("CSG GENERAL ERROR:" + e.Message);

        //        }
        //        finally
        //        {
        //            sbSQLColumns.Remove(0, sbSQLColumns.Length);
        //            sbSQLValues.Remove(0, sbSQLValues.Length);
        //            sbSQLUpdateCombo.Remove(0, sbSQLUpdateCombo.Length);
        //        }


        //    }

        //    string[] strFiltersArr =  dtResultsGLOBAL.AsEnumerable().Select(r => r.Field<string>("FileId")).ToArray();


        //    dtSASDemographics = getSASData(strFiltersArr);

        //    //EXCEL FUNCTION 2 tables merged!!!!
        //    generateExcel(dtResultsGLOBAL, dtSASDemographics);

        //}



        private static DataTable getSASData(string[] strFiltersArr)
        {

            StringBuilder sbFilters = new StringBuilder();



            string strSQL = "select DISTINCT a.SRVC_REF_NBR,a.MBR_ID, a.FST_NM, a.LST_NM, PUT(a.BTH_DT, MMDDYY10.) as BTH_DT, a.Ethnicity, a.Race, a.AGE, a.Age_Category, PUT(a.ADMITDATE, MMDDYY10.) as ADMITDATE, PUT(a.DEATH_DATE, MMDDYY10.) as DEATH_DATE FROM COVID.COVID_ICUE_OCR a where a.SRVC_REF_NBR in ({$filters}) ORDER BY a.MBR_ID";
            DataTable dtFinal = null;

            foreach (string s in strFiltersArr)
            {
                sbFilters.Append("'" + s.Trim() + "',");
            }


            try
            {
                IR_SAS_Connect.strSASHost = ConfigurationManager.AppSettings["SAS_Host"];
                IR_SAS_Connect.intSASPort = int.Parse(ConfigurationManager.AppSettings["SAS_Port"]);
                IR_SAS_Connect.strSASClassIdentifier = ConfigurationManager.AppSettings["SAS_ClassIdentifier"];
                IR_SAS_Connect.strSASUserName = ConfigurationManager.AppSettings["SAS_UN"];
                IR_SAS_Connect.strSASPassword = ConfigurationManager.AppSettings["SAS_PW"];
                IR_SAS_Connect.strSASUserNameUnix = ConfigurationManager.AppSettings["SAS_UN_Unix"];
                IR_SAS_Connect.strSASPasswordUnix = ConfigurationManager.AppSettings["SAS_PW_Unix"];


                Console.WriteLine("Connecting to SAS Server...");
                IR_SAS_Connect.create_SAS_instance(IR_SAS_Connect.getLib());

                dtFinal = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL.Replace("{$filters}", sbFilters.ToString().TrimEnd(',')));
            }
            catch(Exception ex)
            {
                Console.WriteLine("CSG SAS ERROR:" + ex.Message);
                Console.ReadKey();
            }
           finally
            {
                try
                {
                    DBConnection32.getOleDbDataTableGlobalClose();
                    IR_SAS_Connect.destroy_SAS_instance();


                }
                catch (Exception)
                {

                }
            }



            return dtFinal;
        }




        private static void generateExcel(DataTable dtMain, DataTable dtDemographics)
        {

            XLWorkbook workbook = new XLWorkbook(strExcelTemplatePathGLOBAL);
            IXLWorksheet ws;
            IXLRange rng;

            //SUMMARY SHEET
            //SUMMARY SHEET
            //SUMMARY SHEET
            DataRow[] drSummary = null;
            int intRow = 1;
            int intColCnt = 0;
            string strMemberId = null;
            string strSRVC_REF_NBR = null;
            StringBuilder sbRef = new StringBuilder();
            ws = workbook.Worksheet(1);
            foreach (DataRow dr in dtDemographics.Rows)
            {
                if(strMemberId == null || strMemberId != dr["MBR_ID"].ToString())
                {

                    if (strMemberId != null)
                    {
                        drSummary = dtMain.Select("pdf_id in (" + sbRef.ToString().TrimEnd(',') + ")");
                        addExcelSummary(ws, intRow, drSummary);
                        sbRef.Remove(0, sbRef.Length);
                    }

                    strMemberId = dr["MBR_ID"].ToString();
                    intRow++;
                    ws.Cell(intRow, 1).Value = strMemberId;
                    ws.Cell(intRow, 2).Value = (dr["FST_NM"] != DBNull.Value ? dr["FST_NM"].ToString().Trim() : "");
                    ws.Cell(intRow, 3).Value = (dr["LST_NM"] != DBNull.Value ? dr["LST_NM"].ToString().Trim() : "");
                    ws.Cell(intRow, 4).Value = (dr["BTH_DT"] != DBNull.Value ? dr["BTH_DT"].ToString().Trim() : "");
                    ws.Cell(intRow, 5).Value = (dr["Ethnicity"] != DBNull.Value ? dr["Ethnicity"].ToString().Trim() : "");
                    ws.Cell(intRow, 6).Value = (dr["Race"] != DBNull.Value ? dr["Race"].ToString().Trim() : "");
                    ws.Cell(intRow, 7).Value = (dr["AGE"] != DBNull.Value ? dr["AGE"].ToString().Trim() : "");
                    ws.Cell(intRow, 8).Value = (dr["Age_Category"] != DBNull.Value ? dr["Age_Category"].ToString().Trim() : "");
                    ws.Cell(intRow, 9).Value = (dr["ADMITDATE"] != DBNull.Value ? dr["ADMITDATE"].ToString().Trim() : "");
                    ws.Cell(intRow, 10).Value = (dr["DEATH_DATE"] != DBNull.Value ? dr["DEATH_DATE"].ToString().Trim() : "");
                }

                //string str;
                //if (sbRef.Length > 0)
                //    str = "breakHere!!!";

                sbRef.Append("'" + dr["SRVC_REF_NBR"].ToString() + "',");

            }
            drSummary = dtMain.Select("pdf_id in (" + sbRef.ToString().TrimEnd(',') + ")");
            addExcelSummary(ws, intRow, drSummary);
            sbRef.Remove(0, sbRef.Length);

            foreach (var item in ws.ColumnsUsed())
            {
                //item.Width = 15.00;
                item.AdjustToContents();// this not working so instead of AdjustToContents() I use .Width
            }


            //DETAILS SHEET
            //DETAILS SHEET
            //DETAILS SHEET
            ws = workbook.Worksheet(2);



            var test = JoinDataTables(dtMain, dtDemographics,
                           (row1, row2) =>
                           row1.Field<string>("pdf_id") == row2.Field<string>("SRVC_REF_NBR"));


            dtMain = resort(test.Select("pdf_text IS NOT NULL").CopyToDataTable(), "MBR_ID", "DESC");
            // rng = ws.Range("L1:AU1");
            strSRVC_REF_NBR = null;
            intRow = 2;
            intColCnt = 0;
            foreach (DataRow dr in dtMain.Rows)
            {

                ws.Cell(intRow, 1).Value = (dr["pdf_id"] != DBNull.Value ? dr["pdf_id"].ToString().Trim() : "");
                ws.Cell(intRow, 2).Value = (dr["MBR_ID"] != DBNull.Value ? dr["MBR_ID"].ToString().Trim() : "");
                ws.Cell(intRow, 3).Value = (dr["FST_NM"] != DBNull.Value ? dr["FST_NM"].ToString().Trim() : "");
                ws.Cell(intRow, 4).Value = (dr["LST_NM"] != DBNull.Value ? dr["LST_NM"].ToString().Trim() : "");
                ws.Cell(intRow, 5).Value = (dr["BTH_DT"] != DBNull.Value ? dr["BTH_DT"].ToString().Trim() : "");
                ws.Cell(intRow, 6).Value = (dr["Ethnicity"] != DBNull.Value ? dr["Ethnicity"].ToString().Trim() : "");
                ws.Cell(intRow, 7).Value = (dr["Race"] != DBNull.Value ? dr["Race"].ToString().Trim() : "");
                ws.Cell(intRow, 8).Value = (dr["AGE"] != DBNull.Value ? dr["AGE"].ToString().Trim() : "");
                ws.Cell(intRow, 9).Value = (dr["Age_Category"] != DBNull.Value ? dr["Age_Category"].ToString().Trim() : "");
                ws.Cell(intRow, 10).Value = (dr["ADMITDATE"] != DBNull.Value ? dr["ADMITDATE"].ToString().Trim() : "");
                ws.Cell(intRow, 11).Value = (dr["DEATH_DATE"] != DBNull.Value ? dr["DEATH_DATE"].ToString().Trim() : "");
                ws.Cell(intRow, 12).Value = (dr["Admit_ICU_Status"] != DBNull.Value ? dr["Admit_ICU_Status"] : "NULL");
                ws.Cell(intRow, 13).Value = (dr["Prone_Position"] != DBNull.Value ? dr["Prone_Position"] : "NULL");
                ws.Cell(intRow, 14).Value = (dr["Ventilator"] != DBNull.Value ? dr["Ventilator"] : "NULL");
                ws.Cell(intRow, 15).Value = (dr["Ventilator_Split"] != DBNull.Value ? dr["Ventilator_Split"] : "NULL");
                ws.Cell(intRow, 16).Value = (dr["Hydroxychloroquine"] != DBNull.Value ? dr["Hydroxychloroquine"] : "NULL");
                ws.Cell(intRow, 17).Value = (dr["Azithromycin"] != DBNull.Value ? dr["Azithromycin"] : "NULL");
                ws.Cell(intRow, 18).Value = (dr["Azithro_Hydroxychlor"] != DBNull.Value ? dr["Azithro_Hydroxychlor"] : "NULL");
                ws.Cell(intRow, 19).Value = (dr["Azithro_Hydroxychl_Zinc"] != DBNull.Value ? dr["Azithro_Hydroxychl_Zinc"] : "NULL");
                ws.Cell(intRow, 20).Value = (dr["Steroid_Use"] != DBNull.Value ? dr["Steroid_Use"] : "NULL");
                ws.Cell(intRow, 21).Value = (dr["Remdesivir"] != DBNull.Value ? dr["Remdesivir"] : "NULL");
                ws.Cell(intRow, 22).Value = (dr["EIDD_2801"] != DBNull.Value ? dr["EIDD_2801"] : "NULL");
                ws.Cell(intRow, 23).Value = (dr["Ceftriax_Rocephin"] != DBNull.Value ? dr["Ceftriax_Rocephin"] : "NULL");
                ws.Cell(intRow, 24).Value = (dr["Other_Antibiotics"] != DBNull.Value ? dr["Other_Antibiotics"] : "NULL");
                ws.Cell(intRow, 25).Value = (dr["Zinc_Suppl"] != DBNull.Value ? dr["Zinc_Suppl"] : "NULL");
                ws.Cell(intRow, 26).Value = (dr["Plasma_Use"] != DBNull.Value ? dr["Plasma_Use"] : "NULL");
                ws.Cell(intRow, 27).Value = (dr["Hyperbaric_O2"] != DBNull.Value ? dr["Hyperbaric_O2"] : "NULL");
                ws.Cell(intRow, 28).Value = (dr["Avigan_Favipiravir"] != DBNull.Value ? dr["Avigan_Favipiravir"] : "NULL");
                ws.Cell(intRow, 29).Value = (dr["Actemra_Tociliz"] != DBNull.Value ? dr["Actemra_Tociliz"] : "NULL");
                ws.Cell(intRow, 30).Value = (dr["Kevzara_Sarilumb"] != DBNull.Value ? dr["Kevzara_Sarilumb"] : "NULL");
                ws.Cell(intRow, 31).Value = (dr["Monteluk_Singulair"] != DBNull.Value ? dr["Monteluk_Singulair"] : "NULL");
                ws.Cell(intRow, 32).Value = (dr["Vit_C"] != DBNull.Value ? dr["Vit_C"] : "NULL");
                ws.Cell(intRow, 33).Value = (dr["Vit_D"] != DBNull.Value ? dr["Vit_D"] : "NULL");
                ws.Cell(intRow, 34).Value = (dr["Magnesium"] != DBNull.Value ? dr["Magnesium"] : "NULL");
                ws.Cell(intRow, 35).Value = (dr["Anticoagulant"] != DBNull.Value ? dr["Anticoagulant"] : "NULL");
                ws.Cell(intRow, 36).Value = (dr["Aspirin"] != DBNull.Value ? dr["Aspirin"] : "NULL");
                ws.Cell(intRow, 37).Value = (dr["Atazanavir"] != DBNull.Value ? dr["Atazanavir"] : "NULL");
                ws.Cell(intRow, 38).Value = (dr["Tenofov_Lam_Riton"] != DBNull.Value ? dr["Tenofov_Lam_Riton"] : "NULL");


                ws.Cell(intRow, 39).Value = (dr["pdf_url"] != DBNull.Value ? dr["pdf_url"].ToString().Trim() : "");
                ws.Cell(intRow, 39).Hyperlink = new XLHyperlink(dr["pdf_url"].ToString().Trim());
                ws.Cell(intRow, 40).Value = (dr["ocr_text_path"] != DBNull.Value ? dr["ocr_text_path"].ToString().Trim() : "");
                ws.Cell(intRow, 40).Hyperlink = new XLHyperlink(dr["ocr_text_path"].ToString().Trim());



                intRow++;
            }


            //// rng = ws.Range("L1:AU1");
            //strSRVC_REF_NBR = null;
            //intRow = 2;
            //intColCnt = 0;
            //DataRow drDemographics = null;
            //foreach (DataRow dr  in dtMain.Rows)
            //{

            //    strSRVC_REF_NBR = dr["pdf_id"].ToString().Trim();
            //    ws.Cell(intRow, 1).Value = strSRVC_REF_NBR;

            //    drDemographics = dtDemographics.Select("SRVC_REF_NBR = '"+ strSRVC_REF_NBR + "'").FirstOrDefault();
            //    ws.Cell(intRow, 2).Value = (drDemographics["MBR_ID"] != DBNull.Value ? drDemographics["MBR_ID"].ToString().Trim() : "");
            //    ws.Cell(intRow, 3).Value = (drDemographics["FST_NM"] != DBNull.Value ? drDemographics["FST_NM"].ToString().Trim() : "");
            //    ws.Cell(intRow, 4).Value = (drDemographics["LST_NM"] != DBNull.Value ? drDemographics["LST_NM"].ToString().Trim() : "");
            //    ws.Cell(intRow, 5).Value = (drDemographics["BTH_DT"] != DBNull.Value ? drDemographics["BTH_DT"].ToString().Trim() : "");
            //    ws.Cell(intRow, 6).Value = (drDemographics["Ethnicity"] != DBNull.Value ? drDemographics["Ethnicity"].ToString().Trim() : "");
            //    ws.Cell(intRow, 7).Value = (drDemographics["Race"] != DBNull.Value ? drDemographics["Race"].ToString().Trim() : "");
            //    ws.Cell(intRow, 8).Value = (drDemographics["AGE"] != DBNull.Value ? drDemographics["AGE"].ToString().Trim() : "");
            //    ws.Cell(intRow, 9).Value = (drDemographics["Age_Category"] != DBNull.Value ? drDemographics["Age_Category"].ToString().Trim() : "");
            //    ws.Cell(intRow, 10).Value = (drDemographics["ADMITDATE"] != DBNull.Value ? drDemographics["ADMITDATE"].ToString().Trim() : "");
            //    ws.Cell(intRow, 11).Value = (drDemographics["DEATH_DATE"] != DBNull.Value ? drDemographics["DEATH_DATE"].ToString().Trim() : "");


            //    ws.Cell(intRow, 12).Value = (dr["Admit_ICU_Status"] != DBNull.Value ? dr["Admit_ICU_Status"] :"NULL");
            //    ws.Cell(intRow, 13).Value = (dr["Prone_Position"] != DBNull.Value ? dr["Prone_Position"] :"NULL");
            //    ws.Cell(intRow, 14).Value = (dr["Ventilator"] != DBNull.Value ? dr["Ventilator"] : "NULL");
            //    ws.Cell(intRow, 15).Value = (dr["Ventilator_Split"] != DBNull.Value ? dr["Ventilator_Split"] :"NULL");
            //    ws.Cell(intRow, 16).Value = (dr["Hydroxychloroquine"] != DBNull.Value ? dr["Hydroxychloroquine"] :"NULL");
            //    ws.Cell(intRow, 17).Value = (dr["Azithromycin"] != DBNull.Value ? dr["Azithromycin"] :"NULL");
            //    ws.Cell(intRow, 18).Value = (dr["Azithro_Hydroxychlor"] != DBNull.Value ? dr["Azithro_Hydroxychlor"] :"NULL");
            //    ws.Cell(intRow, 19).Value = (dr["Azithro_Hydroxychl_Zinc"] != DBNull.Value ? dr["Azithro_Hydroxychl_Zinc"] :"NULL");
            //    ws.Cell(intRow, 20).Value = (dr["Steroid_Use"] != DBNull.Value ? dr["Steroid_Use"] :"NULL");
            //    ws.Cell(intRow, 21).Value = (dr["Remdesivir"] != DBNull.Value ? dr["Remdesivir"] :"NULL");
            //    ws.Cell(intRow, 22).Value = (dr["EIDD_2801"] != DBNull.Value ? dr["EIDD_2801"] :"NULL");
            //    ws.Cell(intRow, 23).Value = (dr["Ceftriax_Rocephin"] != DBNull.Value ? dr["Ceftriax_Rocephin"] :"NULL");
            //    ws.Cell(intRow, 24).Value = (dr["Other_Antibiotics"] != DBNull.Value ? dr["Other_Antibiotics"] :"NULL");
            //    ws.Cell(intRow, 25).Value = (dr["Zinc_Suppl"] != DBNull.Value ? dr["Zinc_Suppl"] :"NULL");
            //    ws.Cell(intRow, 26).Value = (dr["Plasma_Use"] != DBNull.Value ? dr["Plasma_Use"] :"NULL");
            //    ws.Cell(intRow, 27).Value = (dr["Hyperbaric_O2"] != DBNull.Value ? dr["Hyperbaric_O2"] :"NULL");
            //    ws.Cell(intRow, 28).Value = (dr["Avigan_Favipiravir"] != DBNull.Value ? dr["Avigan_Favipiravir"] :"NULL");
            //    ws.Cell(intRow, 29).Value = (dr["Actemra_Tociliz"] != DBNull.Value ? dr["Actemra_Tociliz"] :"NULL");
            //    ws.Cell(intRow, 30).Value = (dr["Kevzara_Sarilumb"] != DBNull.Value ? dr["Kevzara_Sarilumb"] :"NULL");
            //    ws.Cell(intRow, 31).Value = (dr["Monteluk_Singulair"] != DBNull.Value ? dr["Monteluk_Singulair"] :"NULL");
            //    ws.Cell(intRow, 32).Value = (dr["Vit_C"] != DBNull.Value ? dr["Vit_C"] :"NULL");
            //    ws.Cell(intRow, 33).Value = (dr["Vit_D"] != DBNull.Value ? dr["Vit_D"] :"NULL");
            //    ws.Cell(intRow, 34).Value = (dr["Magnesium"] != DBNull.Value ? dr["Magnesium"] :"NULL");
            //    ws.Cell(intRow, 35).Value = (dr["Anticoagulant"] != DBNull.Value ? dr["Anticoagulant"] :"NULL");
            //    ws.Cell(intRow, 36).Value = (dr["Aspirin"] != DBNull.Value ? dr["Aspirin"] :"NULL");
            //    ws.Cell(intRow, 37).Value = (dr["Atazanavir"] != DBNull.Value ? dr["Atazanavir"] :"NULL");
            //    ws.Cell(intRow, 38).Value = (dr["Tenofov_Lam_Riton"] != DBNull.Value ? dr["Tenofov_Lam_Riton"] :"NULL");



            //    ws.Cell(intRow, 39).Value = (dr["pdf_url"] != DBNull.Value ? dr["pdf_url"].ToString().Trim() : "");
            //    ws.Cell(intRow, 39).Hyperlink = new XLHyperlink(dr["pdf_url"].ToString().Trim());

            //    intColCnt++;
            //    ws.Cell(intRow, 40).Value = (dr["ocr_text_path"] != DBNull.Value ? dr["ocr_text_path"].ToString().Trim() : "");
            //    ws.Cell(intRow, 40).Hyperlink = new XLHyperlink(dr["ocr_text_path"].ToString().Trim());



            //     intRow++;
            //}


           // ws.Rows("A1:BC10").AdjustToContents();
            //ws.Columns("A1:BC" + intRow).AdjustToContents();
            foreach (var item in ws.ColumnsUsed())
            {
                //item.Width = 15.00;
                item.AdjustToContents();// this not working so instead of AdjustToContents() I use .Width
            }
            //HACK DUE TO AUTOSIZE BUG :(
            ws.Columns("O:O").Width = 10.00;
            ws.Columns("A:A").Width = 15.00;

            var strFinalFile = strReportDestinationGLOBAL + "test.xlsx";
            if (File.Exists(strFinalFile))
                File.Delete(strFinalFile);

            workbook.SaveAs(strFinalFile);

        }

        private static DataTable JoinDataTables(DataTable t1, DataTable t2, params Func<DataRow, DataRow, bool>[] joinOn)
        {
            DataTable result = new DataTable();
            foreach (DataColumn col in t1.Columns)
            {
                if (result.Columns[col.ColumnName] == null)
                    result.Columns.Add(col.ColumnName, col.DataType);
            }
            foreach (DataColumn col in t2.Columns)
            {
                if (result.Columns[col.ColumnName] == null)
                    result.Columns.Add(col.ColumnName, col.DataType);
            }
            foreach (DataRow row1 in t1.Rows)
            {
                var joinRows = t2.AsEnumerable().Where(row2 =>
                {
                    foreach (var parameter in joinOn)
                    {
                        if (!parameter(row1, row2)) return false;
                    }
                    return true;
                });
                foreach (DataRow fromRow in joinRows)
                {
                    DataRow insertRow = result.NewRow();
                    foreach (DataColumn col1 in t1.Columns)
                    {
                        insertRow[col1.ColumnName] = row1[col1.ColumnName];
                    }
                    foreach (DataColumn col2 in t2.Columns)
                    {
                        insertRow[col2.ColumnName] = fromRow[col2.ColumnName];
                    }
                    result.Rows.Add(insertRow);
                }
            }
            return result;
        }


        public static DataTable resort(DataTable dt, string colName, string direction)
        {
            DataTable dtOut = null;
            dt.DefaultView.Sort = colName + " " + direction;
            dtOut = dt.DefaultView.ToTable();
            return dtOut;
        }



        private static void addExcelSummary(IXLWorksheet ws, int intRow, DataRow[] drSummary)
        {
            ws.Cell(intRow, 11).Value = (drSummary.CopyToDataTable().Compute("max([Admit_ICU_Status])", "[Admit_ICU_Status] IS NOT NULL") != DBNull.Value ? Convert.ToBoolean(drSummary.CopyToDataTable().Compute("max([Admit_ICU_Status])", string.Empty)).ToString() : "NULL");
            ws.Cell(intRow, 12).Value = (drSummary.CopyToDataTable().Compute("max([Prone_Position])", "[Prone_Position] IS NOT NULL") != DBNull.Value ? Convert.ToBoolean(drSummary.CopyToDataTable().Compute("max([Prone_Position])", string.Empty)).ToString() : "NULL");
            ws.Cell(intRow, 13).Value = (drSummary.CopyToDataTable().Compute("max([Ventilator])", "[Ventilator] IS NOT NULL") != DBNull.Value ? Convert.ToBoolean(drSummary.CopyToDataTable().Compute("max([Ventilator])", string.Empty)).ToString() : "NULL");
            ws.Cell(intRow, 14).Value = (drSummary.CopyToDataTable().Compute("max([Ventilator_Split])", "[Ventilator_Split] IS NOT NULL") != DBNull.Value ? Convert.ToBoolean(drSummary.CopyToDataTable().Compute("max([Ventilator_Split])", string.Empty)).ToString() : "NULL");

            ws.Cell(intRow, 15).Value = (drSummary.CopyToDataTable().Compute("max([Hydroxychloroquine])", "[Hydroxychloroquine] IS NOT NULL") != DBNull.Value ? Convert.ToBoolean(drSummary.CopyToDataTable().Compute("max([Hydroxychloroquine])", string.Empty)).ToString() : "NULL");
            ws.Cell(intRow, 16).Value = (drSummary.CopyToDataTable().Compute("max([Azithromycin])", "[Azithromycin] IS NOT NULL") != DBNull.Value ? Convert.ToBoolean(drSummary.CopyToDataTable().Compute("max([Azithromycin])", string.Empty)).ToString() : "NULL");
            ws.Cell(intRow, 17).Value = (drSummary.CopyToDataTable().Compute("max([Azithro_Hydroxychlor])", "[Azithro_Hydroxychlor] IS NOT NULL") != DBNull.Value ? Convert.ToBoolean(drSummary.CopyToDataTable().Compute("max([Azithro_Hydroxychlor])", string.Empty)).ToString() : "NULL");
            ws.Cell(intRow, 18).Value = (drSummary.CopyToDataTable().Compute("max([Azithro_Hydroxychl_Zinc])", "[Azithro_Hydroxychl_Zinc] IS NOT NULL") != DBNull.Value ? Convert.ToBoolean(drSummary.CopyToDataTable().Compute("max([Azithro_Hydroxychl_Zinc])", string.Empty)).ToString() : "NULL");
            ws.Cell(intRow, 19).Value = (drSummary.CopyToDataTable().Compute("max([Steroid_Use])", "[Steroid_Use] IS NOT NULL") != DBNull.Value ? Convert.ToBoolean(drSummary.CopyToDataTable().Compute("max([Steroid_Use])", string.Empty)).ToString() : "NULL");
            ws.Cell(intRow, 20).Value = (drSummary.CopyToDataTable().Compute("max([Remdesivir])", "[Remdesivir] IS NOT NULL") != DBNull.Value ? Convert.ToBoolean(drSummary.CopyToDataTable().Compute("max([Remdesivir])", string.Empty)).ToString() : "NULL");
            ws.Cell(intRow, 21).Value = (drSummary.CopyToDataTable().Compute("max([EIDD_2801])", "[EIDD_2801] IS NOT NULL") != DBNull.Value ? Convert.ToBoolean(drSummary.CopyToDataTable().Compute("max([EIDD_2801])", string.Empty)).ToString() : "NULL");
            ws.Cell(intRow, 22).Value = (drSummary.CopyToDataTable().Compute("max([Ceftriax_Rocephin])", "[Ceftriax_Rocephin] IS NOT NULL") != DBNull.Value ? Convert.ToBoolean(drSummary.CopyToDataTable().Compute("max([Ceftriax_Rocephin])", string.Empty)).ToString() : "NULL");
            ws.Cell(intRow, 23).Value = (drSummary.CopyToDataTable().Compute("max([Other_Antibiotics])", "[Other_Antibiotics] IS NOT NULL") != DBNull.Value ? Convert.ToBoolean(drSummary.CopyToDataTable().Compute("max([Other_Antibiotics])", string.Empty)).ToString() : "NULL");
            ws.Cell(intRow, 24).Value = (drSummary.CopyToDataTable().Compute("max([Zinc_Suppl])", "[Zinc_Suppl] IS NOT NULL") != DBNull.Value ? Convert.ToBoolean(drSummary.CopyToDataTable().Compute("max([Zinc_Suppl])", string.Empty)).ToString() : "NULL");
            ws.Cell(intRow, 25).Value = (drSummary.CopyToDataTable().Compute("max([Plasma_Use])", "[Plasma_Use] IS NOT NULL") != DBNull.Value ? Convert.ToBoolean(drSummary.CopyToDataTable().Compute("max([Plasma_Use])", string.Empty)).ToString() : "NULL");
            ws.Cell(intRow, 26).Value = (drSummary.CopyToDataTable().Compute("max([Hyperbaric_O2])", "[Hyperbaric_O2] IS NOT NULL") != DBNull.Value ? Convert.ToBoolean(drSummary.CopyToDataTable().Compute("max([Hyperbaric_O2])", string.Empty)).ToString() : "NULL");
            ws.Cell(intRow, 27).Value = (drSummary.CopyToDataTable().Compute("max([Avigan_Favipiravir])", "[Avigan_Favipiravir] IS NOT NULL") != DBNull.Value ? Convert.ToBoolean(drSummary.CopyToDataTable().Compute("max([Avigan_Favipiravir])", string.Empty)).ToString() : "NULL");
            ws.Cell(intRow, 28).Value = (drSummary.CopyToDataTable().Compute("max([Actemra_Tociliz])", "[Actemra_Tociliz] IS NOT NULL") != DBNull.Value ? Convert.ToBoolean(drSummary.CopyToDataTable().Compute("max([Actemra_Tociliz])", string.Empty)).ToString() : "NULL");
            ws.Cell(intRow, 29).Value = (drSummary.CopyToDataTable().Compute("max([Kevzara_Sarilumb])", "[Kevzara_Sarilumb] IS NOT NULL") != DBNull.Value ? Convert.ToBoolean(drSummary.CopyToDataTable().Compute("max([Kevzara_Sarilumb])", string.Empty)).ToString() : "NULL");
            ws.Cell(intRow, 30).Value = (drSummary.CopyToDataTable().Compute("max([Monteluk_Singulair])", "[Monteluk_Singulair] IS NOT NULL") != DBNull.Value ? Convert.ToBoolean(drSummary.CopyToDataTable().Compute("max([Monteluk_Singulair])", string.Empty)).ToString() : "NULL");
            ws.Cell(intRow, 31).Value = (drSummary.CopyToDataTable().Compute("max([Vit_C])", "[Vit_C] IS NOT NULL") != DBNull.Value ? Convert.ToBoolean(drSummary.CopyToDataTable().Compute("max([Vit_C])", string.Empty)).ToString() : "NULL");
            ws.Cell(intRow, 32).Value = (drSummary.CopyToDataTable().Compute("max([Vit_D])", "[Vit_D] IS NOT NULL") != DBNull.Value ? Convert.ToBoolean(drSummary.CopyToDataTable().Compute("max([Vit_D])", string.Empty)).ToString() : "NULL");
            ws.Cell(intRow, 33).Value = (drSummary.CopyToDataTable().Compute("max([Magnesium])", "[Magnesium] IS NOT NULL") != DBNull.Value ? Convert.ToBoolean(drSummary.CopyToDataTable().Compute("max([Magnesium])", string.Empty)).ToString() : "NULL");
            ws.Cell(intRow, 34).Value = (drSummary.CopyToDataTable().Compute("max([Anticoagulant])", "[Anticoagulant] IS NOT NULL") != DBNull.Value ? Convert.ToBoolean(drSummary.CopyToDataTable().Compute("max([Anticoagulant])", string.Empty)).ToString() : "NULL");
            ws.Cell(intRow, 35).Value = (drSummary.CopyToDataTable().Compute("max([Aspirin])", "[Aspirin] IS NOT NULL") != DBNull.Value ? Convert.ToBoolean(drSummary.CopyToDataTable().Compute("max([Aspirin])", string.Empty)).ToString() : "NULL");
            ws.Cell(intRow, 36).Value = (drSummary.CopyToDataTable().Compute("max([Atazanavir])", "[Atazanavir] IS NOT NULL") != DBNull.Value ? Convert.ToBoolean(drSummary.CopyToDataTable().Compute("max([Atazanavir])", string.Empty)).ToString() : "NULL");
            ws.Cell(intRow, 37).Value = (drSummary.CopyToDataTable().Compute("max([Tenofov_Lam_Riton])", "[Tenofov_Lam_Riton] IS NOT NULL") != DBNull.Value ? Convert.ToBoolean(drSummary.CopyToDataTable().Compute("max([Tenofov_Lam_Riton])", string.Empty)).ToString() : "NULL");
        }

        private static String BytesToString(long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
            if (byteCount == 0)
                return "0" + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + suf[place];
        }


    }



}
