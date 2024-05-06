using DBConnectionLibrary;
using System.Collections;
using System.Data;
using System.Data.OleDb;
using System.Text;


namespace SASConnectionLibrary
{
    public class SASConnection
    {
        public static string SASHost = null;// "sasgrid.uhc.com";
        public static int SASPort = 0; // 8564;
        public static string SASClassIdentifier = null;// "0217E202-B560-11DB-AD91-001083FF6836";
        public static string SASUserName = null;// "cgiorda";
        public static string SASPassword = null; //xxxxxxxxxxxxxx
        public static string SASUserNameUnix = null;// "cgiorda";
        public static string SASPasswordUnix = null; //xxxxxxxxxxxxxx
        public static string SASUserNameOracle = null;// "UHG_000556521";
        public static string SASPasswordOracle = null; //xxxxxxxxxxxxxx


        public static string strSASConnectionLog = null;
        public static string strProcSQLResults = null; //xxxxxxxxxxxxxx
        public static string strSASLog = null; //xxxxxxxxxxxxxx

        public static string strSASConnectionString = null;
        public static ArrayList alLiveServerLibs = null;

        public static SAS.LanguageService objLangServ = null;
        private static SASObjectManager.ObjectKeeper objKeeper = null;
        private static SASObjectManager.ObjectFactoryMulti2 objFactory = null;
        private static SASObjectManager.ServerDef objServerDef = null;
        public static SAS.Workspace objSAS = null;
        private static SAS.Libref objLibRef = null;
        public static Array arrLibnames = null;


        public static StringBuilder sbSASLog = new StringBuilder();


        public static void create_SAS_instance(string strAlias, string strPath)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Alias", typeof(string));
            dt.Columns.Add("Path", typeof(string));

            DataRow dr = dt.NewRow();
            dr["Alias"] = strAlias;
            dr["Path"] = strPath;
            dt.Rows.Add(dr);


            create_SAS_instance(dt);
        }


        private static void populateLiveLibServers()
        {
            alLiveServerLibs = new ArrayList();
            alLiveServerLibs.Add("Libname UHN SQLSVR Datasrc='UHN_Reporting_IWA' User = '{$un}' Password = '{$pw}' Schema = DBO Connection=Global;");
            alLiveServerLibs.Add("Libname IL_UCA SQLSVR Datasrc='IL_UCA_IWA' User = '{$un}' Password = '{$pw}' Schema = DBO;");
            alLiveServerLibs.Add("Libname PDPRODC SQLSVR Datasrc='UHPD_Analytics662' User = '{$un}' Password = '{$pw}' Schema = PD;");
            alLiveServerLibs.Add("Libname PDSTAGE SQLSVR Datasrc='UHPD_Analytics2' User = '{$un}' Password = '{$pw}' Schema = PD;");
            alLiveServerLibs.Add("Libname PDSTAGG SQLSVR Datasrc='UHPD_Analytics2' User = '{$un}' Password = '{$pw}' Schema = GRP;");
            alLiveServerLibs.Add("Libname glxyPROD DB2 dsn='GLXYPROD' uid='{$un}' pwd='{$pw}' schema='GALAXY';");
            alLiveServerLibs.Add("libname UDWP_FIN Teradata Server = 'UDWPROD' user='{$un}' Password ='{$pw}' Schema =UHCDM001 Connection=Global;");
            alLiveServerLibs.Add("libname UDWP_CLI Teradata Server = 'UDWPROD' user='{$un}' Password ='{$pw}' Schema =CLODM001 Connection=Global;");
            alLiveServerLibs.Add("LIBNAME CDR ORACLE USER='{$unO}' PASSWORD='{$pwO}' PATH='CDRPR03' Schema=STG_HSR Connection=Global;");
            //alLiveServerLibs.Add("libname CDR ORACLE PATH='CDR_PROD' user='{$unO}' orapw ='{$pwO}' Schema=STG_HSR Connection=Global;");
            //alLiveServerLibs.Add("CONNECT TO teradata AS tera(user = '{$un}' password = '{$pw}' Server = 'UDWPROD');");
            ///alLiveServerLibs.Add("CONNECT TO DB2 AS DB2(user = '{$un}' password = '{$pw}'database = 'GLXYPROD');");
        }



        public static DataTable getLib()
        {
            DataTable dtLib = new DataTable();
            dtLib.Columns.Add("Alias", typeof(string));
            dtLib.Columns.Add("Path", typeof(string));

            DataRow drLib = dtLib.NewRow();
            drLib["Alias"] = "Ph34";
            drLib["Path"] = "/hpsasfin/int/projects/uhccpm/PHI/projects/analytics/pbp/Ph34";
            dtLib.Rows.Add(drLib);

            drLib = dtLib.NewRow();
            drLib["Alias"] = "Ph35";
            drLib["Path"] = "/hpsasfin/int/projects/uhccpm/PHI/projects/analytics/pbp/Ph35";
            dtLib.Rows.Add(drLib);

            drLib = dtLib.NewRow();
            drLib["Alias"] = "Ph14";
            drLib["Path"] = "/hpsasfin/int/projects/uhccpm/PHI/projects/analytics/pbp/Ph14";
            dtLib.Rows.Add(drLib);

            drLib = dtLib.NewRow();
            drLib["Alias"] = "Ph15";
            drLib["Path"] = "/hpsasfin/int/projects/uhccpm/PHI/projects/analytics/pbp/Ph15";
            dtLib.Rows.Add(drLib);

            drLib = dtLib.NewRow();
            drLib["Alias"] = "Ph16";
            drLib["Path"] = "/hpsasfin/int/projects/uhccpm/PHI/projects/analytics/pbp/Ph16";
            dtLib.Rows.Add(drLib);

            drLib = dtLib.NewRow();
            drLib["Alias"] = "CARD";
            drLib["Path"] = "/hpsasfin/int/projects/uhccpm/PHI/projects/analytics/pbp/Card/Cath/Data_Spec_2019";
            dtLib.Rows.Add(drLib);

            drLib = dtLib.NewRow();
            drLib["Alias"] = "ERD";
            drLib["Path"] = "/hpsasfin/int/nas/bi_out/PCR/Vendor_Pilots/ER_Discharge";
            dtLib.Rows.Add(drLib);

            drLib = dtLib.NewRow();
            drLib["Alias"] = "wdrive";
            drLib["Path"] = "/hpsasfin/int/winfiles7/Analytics/Infrastructure/CMS_OPPS_APC_SI_Source_Files";
            dtLib.Rows.Add(drLib);

            drLib = dtLib.NewRow();
            drLib["Alias"] = "NiK";
            drLib["Path"] = "/hpsasfin/int/projects/acad/CategoryAnalytics/Common/code/code_sets";
            dtLib.Rows.Add(drLib);


            drLib = dtLib.NewRow();
            drLib["Alias"] = "sq";
            drLib["Path"] = "/hpsasfin/int/winfiles7/Program/UEP/ICUE_ADT/EI_ER_report/SAS_file/SDR";
            dtLib.Rows.Add(drLib);




            //drLib = dtLib.NewRow();
            //drLib["Alias"] = "base_mn";
            //drLib["Path"] = "/hpsasfin/int/winfiles7/Analytics/Infrastructure/Code Maintenance/ACIS MedN/data";
            //dtLib.Rows.Add(drLib);



            //drLib = dtLib.NewRow();
            //drLib["Alias"] = "SF";
            //drLib["Path"] = "/hpsasfin/int/projects/uhccpm/PHI/projects/analytics/pbp/Ph34/SpineFusion";
            //dtLib.Rows.Add(drLib);

            //drLib = dtLib.NewRow();
            //drLib["Alias"] = "postopms";
            //drLib["Path"] = "/hpsasfin/int/projects/uhccpm/PHI/projects/analytics/pbp/PBC/May2019/postopms";
            //dtLib.Rows.Add(drLib);


            //drLib = dtLib.NewRow();
            //drLib["Alias"] = "postopms";
            //drLib["Path"] = "/hpsasfin/int/projects/uhccpm/PHI/projects/analytics/pbp/PBC/Mar2021/postopms";
            //dtLib.Rows.Add(drLib);

            //drLib = dtLib.NewRow();
            //drLib["Alias"] = "tymp";
            //drLib["Path"] = "/hpsasfin/int/projects/uhccpm/PHI/projects/analytics/pbp/Ph34/ENT/Tympanostomy";
            //dtLib.Rows.Add(drLib);


            //drLib = dtLib.NewRow();
            //drLib["Alias"] = "sin";
            //drLib["Path"] = "/hpsasfin/int/projects/uhccpm/PHI/projects/analytics/pbp/Px/Sinusitis/2019_Q2/Output";
            //dtLib.Rows.Add(drLib);


            //drLib = dtLib.NewRow();
            //drLib["Alias"] = "RX";
            //drLib["Path"] = "/hpsasfin/int/projects/uhccpm/PHI/projects/analytics/pbp/RX_Scorecard/Spec/Data_2019";
            //dtLib.Rows.Add(drLib);


            //drLib = dtLib.NewRow();
            //drLib["Alias"] = "SOS";
            //drLib["Path"] = "/hpsasfin/int/projects/uhccpm/PHI/projects/analytics/pbp/SOS/Data/Spec_2019";
            //dtLib.Rows.Add(drLib);


            //drLib = dtLib.NewRow();
            //drLib["Alias"] = "astsur";
            //drLib["Path"] = "/hpsasfin/int/projects/uhccpm/PHI/projects/analytics/pbp/AsstSurg/Data/Spec_2019";
            //dtLib.Rows.Add(drLib);


            //drLib = dtLib.NewRow();
            //drLib["Alias"] = "OONAS";
            //drLib["Path"] = "/hpsasfin/int/projects/uhccpm/PHI/projects/analytics/pbp/Ph34/OONAS/Data/Spec_2019";
            //dtLib.Rows.Add(drLib);


            //drLib = dtLib.NewRow();
            //drLib["Alias"] = "slsd";
            //drLib["Path"] = "/hpsasfin/int/projects/uhccpm/PHI/projects/analytics/pbp/Ph34/SleepStd";
            //dtLib.Rows.Add(drLib);

            //drLib = dtLib.NewRow();
            //drLib["Alias"] = "SLEEPSTD";
            //drLib["Path"] = "/hpsasfin/int/projects/uhccpm/PHI/projects/analytics/pbp/Ph34/SleepStd";
            //dtLib.Rows.Add(drLib);


            //drLib = dtLib.NewRow();
            //drLib["Alias"] = "onc";
            //drLib["Path"] = "/optum/uhs/01datafs/phi/onc/opchemo/rpt";
            //dtLib.Rows.Add(drLib);




            //drLib = dtLib.NewRow();
            //drLib["Alias"] = "COVID";
            //drLib["Path"] = "/hpsasfin/int/projects/uhccpm/PHI/projects/analytics/pbp/COVID/Data";
            //dtLib.Rows.Add(drLib);




            return dtLib;
        }


        public static void create_SAS_instance(DataTable dtAliasPath = null)
        {

            populateLiveLibServers();

            objServerDef = new SASObjectManager.ServerDef();
            objServerDef.MachineDNSName = SASHost;
            objServerDef.Port = SASPort;
            objServerDef.Protocol = SASObjectManager.Protocols.ProtocolBridge;
            objServerDef.BridgeEncryptionAlgorithm = "SASProprietary";
            objServerDef.BridgeSecurityPackage = "Negotiate";
            objServerDef.ClassIdentifier = SASClassIdentifier;

            objFactory = new SASObjectManager.ObjectFactoryMulti2();
            objKeeper = new SASObjectManager.ObjectKeeper();
            dynamic omi = objFactory.CreateObjectByServer(SASHost, true, objServerDef, SASUserName, SASPassword);
            objFactory.SetRepository(omi);
            IEnumerable<SASObjectManager.ServerDef> serverDefs = objFactory.ServerDefs.Cast<SASObjectManager.ServerDef>();
            SASObjectManager.IServerDef workSpaceServerDef = default(SASObjectManager.IServerDef);

            foreach (SASObjectManager.ServerDef serverDef in serverDefs)
            {
                //if (serverDef.Name == "SASEG - Workspace Server")
                //if (serverDef.Name == "SASMeta - Metadata Server")
                //if (serverDef.Name == "FIN360Prod - Workspace Server")

                //if (serverDef.Name.Contains("Workspace Server"))
                //{
                //    var s = serverDef.Name;
                //}

                Console.WriteLine(serverDef.Name);


                if (serverDef.Name == "FIN360Int - Workspace Server")
                {
                    workSpaceServerDef = serverDef;
                    break;
                }
            }

            //objSAS = (SAS.Workspace)objFactory.CreateObjectByServer(strSASHost, true, (SASObjectManager.ServerDef)workSpaceServerDef, strSASUserName, strSASPassword);
            objSAS = (SAS.Workspace)objFactory.CreateObjectByServer(SASHost, true, (SASObjectManager.ServerDef)workSpaceServerDef, SASUserName, SASPassword);
            objKeeper.AddObject(1, "WorkspaceObject", objSAS);
            objLangServ = objSAS.LanguageService;


            //ADD SAS FILE DATA LIBS
            if (dtAliasPath != null)
            {
                foreach (DataRow dr in dtAliasPath.Rows)
                    objLibRef = objSAS.DataService.AssignLibref(dr["Alias"].ToString(), string.Empty, dr["Path"].ToString(), string.Empty);
            }

            //ADD REMOTE SERVER DATA LIBS
            foreach (string s in alLiveServerLibs)
                objLangServ.Submit(s.Replace("{$un}", SASUserName).Replace("{$pw}", SASPassword).Replace("{$unX}", SASUserNameUnix).Replace("{$pwX}", SASPasswordUnix).Replace("{$unO}", SASUserNameOracle).Replace("{$pwO}", SASPasswordOracle));

            objSAS.DataService.ListLibrefs(out arrLibnames);


            strSASConnectionLog = flushAndGetLogs();
            sbSASLog.Append("---------------------------------------Connections---------------------------------" + Environment.NewLine);
            sbSASLog.Append(strSASConnectionLog + Environment.NewLine);
            Console.Write(sbSASLog.ToString());


            objLangServ.Async = true;
            objLangServ.SuspendOnError = true;
            objLangServ.StepError += new SAS.CILanguageEvents_StepErrorEventHandler(SuspendOnError);
            objLangServ.SubmitComplete += new SAS.CILanguageEvents_SubmitCompleteEventHandler(SubmitComplete);


            objLangServ.DatastepStart +=
                new SAS.CILanguageEvents_DatastepStartEventHandler(sasDataStepStartHandler);
            objLangServ.DatastepComplete +=
                new SAS.CILanguageEvents_DatastepCompleteEventHandler(sasDataStepCompleteHandler);
            objLangServ.ProcStart +=
                new SAS.CILanguageEvents_ProcStartEventHandler(sasProcStartHandler);
            objLangServ.ProcComplete +=
                new SAS.CILanguageEvents_ProcCompleteEventHandler(sasProcCompleteHandler);



            //runProcSQLCommands("proc sql;");
            //flushLanguageServices();
            strProcSQLResults = null;
            strSASLog = null;

            //PROC SQL SOLUTION ?????????????
            //objLangServ.Submit("")?????????????;

            //PASSTHROUGH SOLUTION
            //USED FOR EXTERNAL REF WITHOUT 'public' FULL SAS INSTANCE
            strSASConnectionString = "provider=sas.iomprovider.1; SAS Workspace ID=" + objSAS.UniqueIdentifier + "; ";
        }



        public static DataTable runPassthroughSQLCommandsDT(string strSQL)
        {

            DataTable dt = null;
            try
            {
                //dt = DBConnection64.getOleDbDataTable(strSASConnectionString, strSQL);
                dt = DBConnection.getOleDbDataTable(strSASConnectionString, strSQL);
            }
            catch (Exception ex)
            {
                objLangServ.Reset();
                throw ex;
            }


            return dt;

        }

        public static OleDbDataReader runPassthroughSQLCommandsDR(string strSQL)
        {

            OleDbDataReader oleDr = null;
            try
            {
                //oleDr = DBConnection64.getOleDbDataReader(strSASConnectionString, strSQL);
                oleDr = DBConnection.getOleDbDataReader(strSASConnectionString, strSQL);
            }
            catch (Exception ex)
            {
                objLangServ.Reset();
                throw ex;
            }


            return oleDr;
        }


        public static void runProcSQLCommands(string strSQL, bool blClean = true)
        {
            //EVENT HANDLING SAMPLES!!!!!!
            //https://support.sas.com/rnd/itech/doc9/dev_guide/dist-obj/winclnt/windotnet.html

            blHitProcSQLError = false;
            strProcSQLResults = null;
            strSASLog = null;

            //SURELY A BETTER WAY FOR ONE LINE -- LATER ;-)
            //strSQL = Regex.Replace(strSQL, "proc sql;", "", RegexOptions.IgnoreCase);
            if (!strSQL.ToLower().Replace(" ", "").Contains("procsql") && blClean) //ADD "PROC SQL;" IF NEEDED
                strSQL = "proc sql;" + strSQL;

            objLangServ.Submit(strSQL);
            //strProcSQLResults = objLangServ.FlushList(Int32.MaxValue).Replace("ƒ", "_");

            //THREAD WAITING FOR SubmitComplete(int sasrc) TO FINISH
            SpinWait.SpinUntil(() => !string.IsNullOrEmpty(strProcSQLResults), 100000);

            //2020 OR:
            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();
            //while (!myPredicate() && stopwatch.ElapsedMilliseconds < timeOut)
            //{
            //    Thread.Sleep(50)
            //}





            //objLangServ.Cancel();
            //objLangServ.Reset();
        }


        //public static SAS.LanguageService objLangServ = null;
        //private static SASObjectManager.ObjectKeeper objKeeper = null;
        //private static SASObjectManager.ObjectFactoryMulti2 objFactory = null;
        //private static SASObjectManager.ServerDef objServerDef = null;
        //public static SAS.Workspace objSAS = null;
        //private static SAS.Libref objLibRef = null;
        //public static Array arrLibnames = null;


        public static void runStoredProcess(string strSASFile, string strSASPath)
        {
            strProcSQLResults = null;
            objLangServ.SuspendOnError = false;
            objLangServ.Async = false;


            SAS.StoredProcessService SASproc;
            SASproc = objSAS.LanguageService.StoredProcessService;
            SASproc.Repository = "file:" + strSASPath;
            SASproc.Execute(strSASFile, "ds=Sasuser.Export_output");


            objLangServ.Async = true;
            objLangServ.SuspendOnError = true;



            //SASproc.Repository = "file:/hpsasfin/int/nas/bi_out/PCR/Vendor_Pilots/ER_Discharge";
            //SASproc.Execute("Test_ED_ADT_Report.sas", "ds=Sasuser.Export_output");

            //strProcSQLResults = objLangServ.FlushList(Int32.MaxValue).Replace("ƒ", "_");

            //THREAD WAITING FOR SubmitComplete(int sasrc) TO FINISH
            //SpinWait.SpinUntil(() => !string.IsNullOrEmpty(strProcSQLResults), 1000000000);
            //Console.Write("Done!");
            //Console.Write(strProcSQLResults);
            //2020 OR:
            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();
            //while (!myPredicate() && stopwatch.ElapsedMilliseconds < timeOut)
            //{
            //    Thread.Sleep(50)
            //}





            //objLangServ.Cancel();
            //objLangServ.Reset();
        }



        static bool blDestroyGlobal = false;
        public static void destroy_SAS_instance()
        {
            if (objSAS != null)
            {
                objKeeper.RemoveObject(objSAS);
                blDestroyGlobal = true;
                objSAS.Close();
                objSAS = null;

            }
            objFactory = null;
            objKeeper = null;
        }

        private static void SubmitComplete(int sasrc)
        {

            if (blHitProcSQLError)
            {
                strProcSQLResults = flushAndGetLogs();
                objLangServ.Cancel();
                objLangServ.Reset();

                objLangServ.FlushList(1000000);
                objLangServ.FlushLog(1000000);

                blHitProcSQLError = false;
            }
            else
            {
                strProcSQLResults = flushAndGetLists();
                if (string.IsNullOrEmpty(strProcSQLResults))
                    strProcSQLResults = flushAndGetLogs();
            }


        }

        private static string flushAndGetLists()
        {
            if (blDestroyGlobal)
                return "SAS Connection Closed...";


            StringBuilder sbResults = new StringBuilder();
            System.Array CCs;
            const int maxLines = int.MaxValue;
            System.Array lineTypes;
            System.Array arrLines;

            bool bMore = true;
            while (bMore)
            {
                objLangServ.FlushListLines(maxLines, out CCs, out lineTypes, out arrLines);

                //For some reason, these two declarations need to be here
                SAS.LanguageServiceCarriageControl CarriageControl = new SAS.LanguageServiceCarriageControl();
                SAS.LanguageServiceLineType LineType = new SAS.LanguageServiceLineType();

                for (int i = 0; i < arrLines.Length; i++)
                {
                    sbResults.Append((arrLines.GetValue(i) + "\r\n"));
                }

                if (arrLines.Length < maxLines)
                    bMore = false;
            }
            return sbResults.ToString();
        }


        public static string flushAndGetLogs()
        {
            if (blDestroyGlobal)
                return "SAS Connection Closed...";


            StringBuilder sbResults = new StringBuilder();
            System.Array CCs;
            const int maxLines = int.MaxValue;
            System.Array lineTypes;
            System.Array arrLines;

            bool bMore = true;
            while (bMore)
            {

                objLangServ.FlushLogLines(maxLines, out CCs, out lineTypes, out arrLines);

                //For some reason, these two declarations need to be here
                SAS.LanguageServiceCarriageControl CarriageControl = new SAS.LanguageServiceCarriageControl();
                SAS.LanguageServiceLineType LineType = new SAS.LanguageServiceLineType();

                for (int i = 0; i < arrLines.Length; i++)
                {
                    sbResults.Append((arrLines.GetValue(i) + "\r\n"));
                }

                if (arrLines.Length < maxLines)
                    bMore = false;

            }

            return sbResults.ToString();
            //objLangServ.Reset();
            //objLangServ.Cancel(); 
        }


        public static bool blHitProcSQLError = false;
        private static void SuspendOnError()
        {
            blHitProcSQLError = true;
            //throw new Exception();
            //objLangServ.Reset();
            objLangServ.Cancel();
            //objLangServ.Continue();
        }

        private static void sasProcStartHandler(string procedureName)
        {

            //if (procedureName == "SQL")
            //strProcSQLResults = null;
            sbSASLog.Append("-------------------------------Proc Handle Start " + procedureName + "-------------------------" + Environment.NewLine);
            sbSASLog.Append(flushAndGetLogs() + Environment.NewLine);
            Console.Write(sbSASLog.ToString());

            //strProcSQLResults = null;
            //strProcSQLResults = flushAndGetLogs();
            //Console.Write(strProcSQLResults);
            //strProcSQLResults = null;
            //if (ProcStart != null) ProcStart(procedureName);
        }

        /// <summary>
        /// Handle SAS procedure complete event
        /// </summary>
        /// <param name="procedureName">procedure name</param>
        private static void sasProcCompleteHandler(string procedureName)
        {
            //if(procedureName == "SQL")
            //strProcSQLResults = flushAndGetLogs();
            //strProcSQLResults = flushAndGetLogs();
            sbSASLog.Append("-----------------------------Proc Handle End " + procedureName + "-----------------------------" + Environment.NewLine);
            sbSASLog.Append(flushAndGetLogs() + Environment.NewLine);
            Console.Write(sbSASLog.ToString());
            // strProcSQLResults = null;
            //if (ProcComplete != null) ProcComplete(procedureName);
        }

        /// <summary>
        /// Handle SAS data step start event
        /// </summary>
        private static void sasDataStepStartHandler()
        {
            sbSASLog.Append("--------------------------------------Data Step Start-----------------------------------------");
            sbSASLog.Append(flushAndGetLogs() + Environment.NewLine);
            Console.Write(sbSASLog.ToString());
            //if (DataStepStart != null) DataStepStart();
        }

        /// <summary>
        /// Handle SAS data step complete event
        /// </summary>
        private static void sasDataStepCompleteHandler()
        {
            sbSASLog.Append("-----------------------------------------Data Step End----------------------------------------");
            sbSASLog.Append(flushAndGetLogs() + Environment.NewLine);
            Console.Write(sbSASLog.ToString());
            //if (DataStepComplete != null) DataStepComplete();
        }

    }
}