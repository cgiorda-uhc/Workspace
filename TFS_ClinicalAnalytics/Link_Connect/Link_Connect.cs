using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;


//   PM    5,812 


namespace Link_Connect
{
    class Link_Connect
    {

    //MIKE REPORT 6192018
    //SELECT Distinct a.MPIN, b.taxid, b.P_LastName, b.P_FirstName, p.MPIN as practice_id, CorpOwnerId as CorpMPIN FROM dbo.PBP_Outl_Ph13 as a inner join dbo.PBP_Outl_Ph13_models as m on m.mpin= a.mpin inner join dbo.PBP_outl_demogr_Ph13 as b on a.MPIN= b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph13 as p on p.mpin= PTIGroupID_upd inner join dbo.PBP_dim_RCMO as r on r.Region= b.RGN_NM WHERE a.Exclude in(0,5) AND r.phase_id=2 and a.MPIN in (SELECT mpin from qa_link_request_tracker where letter_type = 'practice')

    //SELECT Distinct ad.TaxID, ad.MPIN as practice_id, ad.Practice_Name, CorpOwnerId as CorpMPIN FROM dbo.PBP_Outl_ph13 as a inner join dbo.PBP_outl_demogr_ph13 as b on a.MPIN= b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph13 as ad on ad.mpin= b.PTIGroupID_upd inner join dbo.PBP_outl_Ph13_models as m on m.MPIN= a.MPIN WHERE a.Exclude in(0,5) AND b.PTIGroupID>0 and ad.MPIN in (SELECT mpin from qa_link_request_tracker where letter_type = 'practice')


        static string fileReadTest = ConfigurationManager.AppSettings["pcr_location"];
        static string fileDownloadTest = ConfigurationManager.AppSettings["pcr_download_results_location"];

        static string strEnvironment = ConfigurationManager.AppSettings["environment"];


        static string strLetterType = ConfigurationManager.AppSettings["LetterType"];
        static string strProjectName = ConfigurationManager.AppSettings["ProjectName"];


        static string external_id;
        static string space_id;

        static string clientId;
        static string clientSecret;
        static string tokenUrl;
        static string documentUrl;
        static string attachmentUrl;
       

        static string accessToken = null;


        static MetaData md = null;
        static RequestContainer mdC = null;


        static string strSQLInsert = "INSERT INTO qa_link_request_tracker (request_string,response_string,external_id,space_id,status,dateCreated,attachment_id,file_name,file_size,environment, mpin,letter_type, project_name,document_id) VALUES ('{$request_string}','{$response_string}','{$external_id}','{$space_id}','{$status}','{$dateCreated}','{$attachment_id}','{$file_name}','{$file_size}','{$environment}',{$mpin},'{$letter_type}','{$project_name}','{$document_id}');";

        static string strSQL = "";

        static string strConnectionString = ConfigurationManager.AppSettings["DatabaseConnectionString"];

        static string strMPin = "";
        static DataTable dt;

        //GLOBAL VARIABLES
        static ResponseContainer responseContainer_GLOBAL = null;
        static string strRequest_GLOBAL = null;
        static string strResponse_GLOBAL = null;
        static string strError_GLOBAL = null;


        static string strSASSQL = null;

        static string strMessageGlobal = null;

        static bool blSASIT = true;

        static void Main(string[] args)
        {


            IR_SAS_Connect.strSASHost = ConfigurationManager.AppSettings["SAS_Host"];
            IR_SAS_Connect.intSASPort = int.Parse(ConfigurationManager.AppSettings["SAS_Port"]);
            IR_SAS_Connect.strSASClassIdentifier = ConfigurationManager.AppSettings["SAS_ClassIdentifier"];
            IR_SAS_Connect.strSASUserName = ConfigurationManager.AppSettings["SAS_UN"];
            IR_SAS_Connect.strSASPassword = ConfigurationManager.AppSettings["SAS_PW"];
            IR_SAS_Connect.strSASUserNameUnix = ConfigurationManager.AppSettings["SAS_UN_Unix"];
            IR_SAS_Connect.strSASPasswordUnix = ConfigurationManager.AppSettings["SAS_PW_Unix"];



            if (strEnvironment == "dev")
            {
                clientId = ConfigurationManager.AppSettings["clientId_dev"];
                clientSecret = ConfigurationManager.AppSettings["clientSecret_dev"];
                tokenUrl = ConfigurationManager.AppSettings["tokenURL_dev"];
                documentUrl = ConfigurationManager.AppSettings["documentURL_dev"];
                external_id = ConfigurationManager.AppSettings["external_id_dev"];
                space_id = ConfigurationManager.AppSettings["space_id_dev"];
                attachmentUrl = ConfigurationManager.AppSettings["attachmentURL_dev"];

            }
            if (strEnvironment == "stg")
            {
                clientId = ConfigurationManager.AppSettings["clientId_stg"];
                clientSecret = ConfigurationManager.AppSettings["clientSecret_stg"];
                tokenUrl = ConfigurationManager.AppSettings["tokenURL_stg"];
                documentUrl = ConfigurationManager.AppSettings["documentURL_stg"];
                external_id = ConfigurationManager.AppSettings["external_id_stg"];
                space_id = ConfigurationManager.AppSettings["space_id_stg"];
                attachmentUrl = ConfigurationManager.AppSettings["attachmentURL_stg"];

            }
            else if (strEnvironment == "prod")
            {
                clientId = ConfigurationManager.AppSettings["clientId_prod"];
                clientSecret = ConfigurationManager.AppSettings["clientSecret_prod"];
                tokenUrl = ConfigurationManager.AppSettings["tokenURL_prod"];
                documentUrl = ConfigurationManager.AppSettings["documentURL_prod"];
                external_id = ConfigurationManager.AppSettings["external_id_prod"];
                space_id = ConfigurationManager.AppSettings["space_id_prod"];
                attachmentUrl = ConfigurationManager.AppSettings["attachmentURL_prod"];

            }
            dt = new DataTable();


            //getCount().Wait();


            //deleteMainRequestMPINS(strLetterType, strProjectName, strEnvironment, "SELECT mpin from qa_link_request_tracker WHERE  letter_type = 'practice' AND environment = 'prod' and project_name = 'SPEC_CH4_PR_PM_Profile'").Wait();

            //return;





            //TEST 2020
            //TestMainRequest().Wait();
            //return;

            TestMainRequest("SELECT attachment_id FROM[IL_UCA].[dbo].[qa_link_request_tracker] WHERE project_name = 'PCP_CH6_PR_PM_Profiles' and mpin in (13268,27423,39894)").Wait();
            return;



            Console.WriteLine("Connecting to SAS Server...");
            IR_SAS_Connect.create_SAS_instance(IR_SAS_Connect.getLib());


            if (strLetterType == "physician")
            {

                if(blSASIT)
                {
 
                    //strSASSQL = "select a.MPIN, b.taxid, b.P_LastName, b.P_FirstName, p.MPIN as practice_id, CorpOwnerId as CorpMPIN from PH34.outliers as a inner join PH34.outl_models as m on m.mpin=a.mpin inner join PH34.UHN_May6_demog as b on a.MPIN=b.MPIN inner join PH34.UHN_May6_pti_demog as p on p.mpin=PTIGroupID_upd inner join Ph34.spec_handling as h on h.mpin=a.mpin WHERE h.Folder_Name = '' ;";

                    //strSASSQL = "select a.MPIN, b.taxid, b.P_LastName, b.P_FirstName, p.MPIN as practice_id, CorpOwnerId as CorpMPIN from Ph15.outliers6 as a inner join Ph15.UHN_JAN14_DEMOG as b on a.MPIN=b.MPIN inner join Ph15.UHN_JAN14_PTI_DEMOG as p on p.mpin=PTIGroupID_upd inner join Ph15.outl_models6 as m on m.mpin=a.mpin inner join IL_UCA.PBP_dim_RCMO as r on r.Region=b.RGN_NM where a.Exclude in(0,5) and r.phase_id=2;";

                    //strSASSQL = "select a.MPIN, b.taxid, b.P_LastName, b.P_FirstName, p.MPIN as practice_id, CorpOwnerId as CorpMPIN from Ph35.outliers8 as a inner join Ph35.UHN_Jun1_DEMOG as b on a.MPIN=b.MPIN inner join Ph35.UHN_Jun1_PTI_DEMOG as p on p.mpin=PTIGroupID_upd inner join Ph35.outl_models8 as m on m.mpin=a.mpin inner join IL_UCA.PBP_dim_RCMO as r on r.Region=b.RGN_NM where a.Exclude in(0,5) and r.phase_id=2;";

                    strSASSQL = "select a.MPIN, b.taxid, b.P_LastName, b.P_FirstName, p.MPIN as practice_id, CorpOwnerId as CorpMPIN from ph16.outliers6 as a inner join ph16.UHN_FEB2_DEMOG as b on a.MPIN=b.MPIN inner join ph16.UHN_FEB2_PTI_DEMOG as p on p.mpin=PTIGroupID_upd inner join ph16.outl_models6 as m on m.mpin=a.mpin inner join IL_UCA.PBP_dim_RCMO as r on r.Region=b.RGN_NM where a.Exclude in(0,5) and r.phase_id=2";


                    DataTable dtMain = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSASSQL);
                    //HADLE BULK INSERTS FOR CONSOLE FEEDBACK
                    dtMain.TableName = "qa_link_request_phy_sas_cache";
                    DBConnection32.ExecuteMSSQL(strConnectionString, "TRUNCATE TABLE " + dtMain.TableName + ";");
                    DBConnection32.SQLServerBulkImportDT(dtMain, strConnectionString);
                    DBConnection32.getOleDbDataTableGlobalClose();
                    IR_SAS_Connect.destroy_SAS_instance();
                }







                //strSQL = "select a.MPIN,b.taxid,b.P_LastName,b.P_FirstName, p.MPIN as practice_id, CorpOwnerId as CorpMPIN from dbo.PBP_Outl_Ph32 as a inner join dbo.PBP_outl_demogr_ph32 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_ph32 as p on p.mpin=PTIGroupID_upd inner join dbo.PBP_spec_handl_ph32 as h on h.MPIN=a.mpin inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM where a.Exclude in(0,5) and r.phase_id=2 and a.MPIN = {?mpin} and a.MPIN not in (SELECT isnull(mpin,0) as mpin from qa_link_request_tracker WHERE error_message is null AND letter_type = '"+strLetterType+"' AND  project_name = '"+ strProjectName +"' AND environment = '"+strEnvironment+"')";

                //LAST USED OFFIFICALLY
                //LAST USED OFFIFICALLY
                //LAST USED OFFIFICALLY
                //LAST USED OFFIFICALLY
                //LAST USED OFFIFICALLY
                //strSQL = "SELECT Distinct a.MPIN, b.taxid, b.P_LastName, b.P_FirstName, p.MPIN as practice_id, CorpOwnerId as CorpMPIN FROM dbo.PBP_Outl_Ph13 as a inner join dbo.PBP_Outl_Ph13_models as m on m.mpin=a.mpin inner join dbo.PBP_outl_demogr_Ph13 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph13 as p on p.mpin=PTIGroupID_upd inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM WHERE a.Exclude in(0,5) AND r.phase_id=2 and a.MPIN = {?mpin} and a.MPIN not in (SELECT isnull(mpin,0) as mpin from qa_link_request_tracker WHERE error_message is null AND letter_type = '" + strLetterType + "' AND  project_name = '" + strProjectName + "' AND environment = '" + strEnvironment + "')";


                //strSQL  = "SELECT Distinct a.MPIN, b.taxid, b.P_LastName, b.P_FirstName, p.MPIN as practice_id, CorpOwnerId as CorpMPIN FROM dbo.PBP_Outl_Ph33 as a inner join dbo.PBP_Outl_Ph33_models as m on m.mpin=a.mpin inner join dbo.PBP_outl_demogr_Ph33 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph33 as p on p.mpin=PTIGroupID_upd inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM WHERE a.Exclude in(0,5) AND r.phase_id=2 and a.MPIN = {?mpin} and a.MPIN not in (SELECT isnull(mpin,0) as mpin from qa_link_request_tracker WHERE error_message is null AND letter_type = '" + strLetterType + "' AND  project_name = '" + strProjectName + "' AND environment = '" + strEnvironment + "') ";


                strSQL = "select distinct  ipc.MPIN, ipc.taxid, ipc.P_LastName, ipc.P_FirstName, ipc.practice_id, ipc.CorpMPIN FROM qa_link_request_phy_sas_cache ipc WHERE ipc.MPIN = {?mpin} and ipc.MPIN not in (SELECT isnull(mpin,0) as mpin from qa_link_request_tracker WHERE error_message is null AND letter_type = '" + strLetterType + "' AND  project_name = '" + strProjectName + "' AND environment = '" + strEnvironment + "') ";




                //strSQL = "select a.MPIN,b.taxid,b.P_LastName,b.P_FirstName, p.MPIN as practice_id, ISNULL(q.MPIN,p.MPIN) as CorpMPIN from dbo.PBP_Outl_Ph32 as a inner join dbo.PBP_outl_demogr_ph32 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_ph32 as p on p.mpin=PTIGroupID_upd inner join dbo.PBP_spec_handl_ph32 as h on h.MPIN=a.mpin inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM LEFT OUTER JOIN qa_link_request_tests_tracker q on a.MPIN = q.mpin where a.Exclude in(0,5) and r.phase_id=2 and a.MPIN = {?mpin}";


                //strSQL = "SELECT * FROM (select 788530 as mpin, 941156581 as taxid,110728 as practice_id, 'HARRIS' as P_LastName union select 971294 as mpin, 941156581 as taxid,110728 as practice_id, 'VASDEV' as P_LastName union select 2708992 as mpin,841457488 as taxid,1341830 as practice_id, 'SCHWARTZ' as P_LastName union select 2792475 as mpin, 841457488 as taxid,1341830 as practice_id, 'MORSE' as P_LastName) tmp WHERE mpin = {?mpin}";



                //strSQL = "SELECT * FROM (select 788530 as mpin, 841457488 as taxid,1341830 as practice_id, 'HARRIS' as P_LastName union select 971294 as mpin, 841457488 as taxid,1341830 as practice_id, 'VASDEV' as P_LastName union select 1460056 as mpin, 841457488 as taxid,1341830 as practice_id, 'CORVALLIS' as P_LastName) tmp WHERE mpin = {?mpin}";




                // strSQL = "SELECT * FROM (select 1460056 as mpin, 841457488 as taxid,1341830 as practice_id, 'CORVALLIS' as P_LastName) tmp WHERE mpin = {?mpin}";


                // strSQL = "SELECT * FROM (select 1460056 as mpin, 941156581 as taxid,110728 as practice_id, 'TestName1_110728' as P_LastName UNION select 788530 as mpin, 941156581 as taxid,110728 as practice_id, 'TestName2_110728' as P_LastName union select 3050054 as mpin, 841457488 as taxid,1341830 as practice_id, 'TestName1_1341830' as P_LastName  union select 971294 as mpin, 841457488 as taxid,1341830 as practice_id, 'TestName2_1341830' as P_LastName  union select 2708992 as mpin, 841457488 as taxid,1341830 as practice_id, 'TestName3_1341830' as P_LastName ) tmp WHERE mpin = {?mpin}";


                //strSQL = "SELECT * FROM (select 1460056 as mpin, 911830142 as taxid,1460056 as practice_id, 'Miketest1' as P_LastName UNION select 788530 as mpin, 911830142 as taxid,1460056 as practice_id, 'Miketest2' as P_LastName union select 3050054 as mpin, 911830142 as taxid,1460056 as practice_id, 'Miketest3' as P_LastName  ) tmp WHERE mpin = {?mpin}";


                //strSQL = "SELECT * FROM (select 1460056 as mpin, 113297654 as taxid,1534045 as practice_id, 'Alextest1' as P_LastName UNION select 788530 as mpin, 113297654 as taxid,1534045 as practice_id, 'Alextest2' as P_LastName union select 3050054 as mpin,113297654 as taxid,1534045 as practice_id, 'Alextest3' as P_LastName  union select 971294 as mpin, 200157091 as taxid,3489103 as practice_id, 'Berktest1' as P_LastName  union select 2708992 as mpin, 200157091 as taxid,3489103  as practice_id, 'Berktest2' as P_LastName  union select 2792475 as mpin, 200157091 as taxid,3489103  as practice_id, 'Berktest3' as P_LastName) tmp WHERE mpin = {?mpin}";




                //strSQL = "SELECT * FROM (select 1460056 as mpin, 841457488 as taxid,1341830 as practice_id, 1341830 as CorpMPIN,'FellerTest3' as P_LastName UNION select 788530 as mpin, 841457488 as taxid,1341830 as practice_id, 1341830 as CorpMPIN, 'FellerTest4' as P_LastName ) tmp WHERE mpin = {?mpin}";

            }
            else if (strLetterType == "practice")
            {


                if (blSASIT)
                {

                    //strSASSQL = "SELECT  distinct ad.TaxID, ad.MPIN as practice_id, ad.Practice_Name, CorpOwnerId as CorpMPIN  FROM ph34.outliers as a inner join ph34.UHN_MAY6_DEMOG as b on a.MPIN=b.MPIN inner join Ph34.UHN_MAY6_PTI_DEMOG as ad on ad.mpin=b.PTIGroupID_upd inner join Ph34.spec_handling as h on h.mpin=a.mpin inner join Ph34.OUTL_MODELS as m on m.mpin=a.mpin WHERE a.Exclude in(0,5) AND b.PTIGroupID>0 AND (h.Folder_Name = '' OR h.Folder_Name IS NULL) ;";

                    //strSASSQL = "SELECT  distinct ad.TaxID, ad.MPIN as practice_id, ad.Practice_Name, CorpOwnerId as CorpMPIN  FROM ph15.outliers6 as a inner join ph15.UHN_JAN14_DEMOG as b on a.MPIN=b.MPIN inner join ph15.UHN_JAN14_PTI_DEMOG as ad on ad.mpin=b.PTIGroupID_upd inner join ph15.OUTL_MODELS6 as m on m.mpin=a.mpin WHERE a.Exclude in(0,5) AND b.PTIGroupID>0;";


                    //strSASSQL = "SELECT distinct ad.TaxID, ad.MPIN as practice_id, ad.Practice_Name, CorpOwnerId as CorpMPIN  FROM ph35.outliers8 as a inner join ph35.UHN_JUN1_DEMOG as b on a.MPIN=b.MPIN inner join ph35.UHN_JUN1_PTI_DEMOG as ad on ad.mpin=b.PTIGroupID_upd inner join ph35.OUTL_MODELS8 as m on m.mpin=a.mpin WHERE a.Exclude in(0,5) AND b.PTIGroupID>0";


                    strSASSQL = "SELECT distinct ad.TaxID, ad.MPIN as practice_id, ad.Practice_Name, CorpOwnerId as CorpMPIN  FROM ph16.outliers6 as a inner join ph16.UHN_FEB2_DEMOG as b on a.MPIN=b.MPIN inner join ph16.UHN_FEB2_PTI_DEMOG as ad on ad.mpin=b.PTIGroupID_upd inner join ph16.OUTL_MODELS6 as m on m.mpin=a.mpin WHERE a.Exclude in(0,5) AND b.PTIGroupID>0";

                    DataTable dtMain = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSASSQL);
                    //HADLE BULK INSERTS FOR CONSOLE FEEDBACK
                    dtMain.TableName = "qa_link_request_pm_sas_cache";
                    DBConnection32.ExecuteMSSQL(strConnectionString, "TRUNCATE TABLE " + dtMain.TableName + ";");
                    DBConnection32.SQLServerBulkImportDT(dtMain, strConnectionString);
                    DBConnection32.getOleDbDataTableGlobalClose();
                    IR_SAS_Connect.destroy_SAS_instance();
                }




                //strSQL = "SELECT distinct ad.TaxID, ad.MPIN as practice_id, ad.Practice_Name, CorpOwnerId as CorpMPIN  FROM dbo.PBP_Outl_ph32 as a inner join dbo.PBP_outl_demogr_ph32 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_ph32 as ad on ad.mpin=b.PTIGroupID_upd inner join dbo.PBP_spec_handl_ph32 as h on h.MPIN=a.mpin inner join dbo.PBP_dim_RCMO as r on ad.RGN_NM=r.Region WHERE a.Exclude in(0,5) AND b.PTIGroupID>0 AND r.phase_id=2  and ad.MPIN = {?mpin} and ad.MPIN not in (SELECT isnull(mpin,0) as mpin from qa_link_request_tracker WHERE error_message is null AND letter_type = '"+strLetterType+"' AND  project_name = '"+ strProjectName +"' AND environment = '"+strEnvironment+"')";

                //strSQL = "SELECT Distinct ad.TaxID, ad.MPIN as practice_id, ad.Practice_Name, CorpOwnerId as CorpMPIN FROM dbo.PBP_Outl_ph13 as a inner join dbo.PBP_outl_demogr_ph13 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph13 as ad on ad.mpin=b.PTIGroupID_upd inner join dbo.PBP_outl_Ph13_models as m on m.MPIN=a.MPIN WHERE a.Exclude in(0,5) AND b.PTIGroupID>0 and ad.MPIN = {?mpin} and ad.MPIN not in (SELECT isnull(mpin,0) as mpin from qa_link_request_tracker WHERE error_message is null AND letter_type = '" + strLetterType + "' AND  project_name = '" + strProjectName + "' AND environment = '" + strEnvironment + "')";



                //strSQL = "SELECT distinct ad.TaxID, ad.MPIN as practice_id, ad.Practice_Name, CorpOwnerId as CorpMPIN  FROM dbo.PBP_Outl_ph33 as a inner join dbo.PBP_outl_demogr_ph33 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_ph33 as ad on ad.mpin=b.PTIGroupID_upd inner join dbo.PBP_spec_handl_ph33 as h on h.MPIN=a.mpin inner join dbo.PBP_dim_RCMO as r on ad.RGN_NM=r.Region WHERE a.Exclude in(0,5) AND b.PTIGroupID>0 AND r.phase_id=2  and ad.MPIN = {?mpin} and ad.MPIN not in (SELECT isnull(mpin,0) as mpin from qa_link_request_tracker WHERE error_message is null AND letter_type = '" + strLetterType + "' AND  project_name = '" + strProjectName + "' AND environment = '" + strEnvironment + "')";


                strSQL = "SELECT distinct pmc.TaxID, pmc.practice_id, pmc.Practice_Name, pmc.CorpMPIN  FROM qa_link_request_pm_sas_cache pmc WHERE pmc.practice_id = {?mpin} and pmc.practice_id not in (SELECT isnull(mpin,0) as mpin from qa_link_request_tracker WHERE error_message is null AND letter_type = '" + strLetterType + "' AND  project_name = '" + strProjectName + "' AND environment = '" + strEnvironment + "')";


                //strSQL = "SELECT * FROM (select 1460056 as mpin, 841457488 as taxid,1341830 as practice_id, 'CORVALLISGASTROENTEROLOGY' as Practice_Name, 123456 as CorpMPIN) tmp WHERE mpin = {?mpin}";

            }



            //getCount().Wait();



            //deleteMainRequest(strLetterType, strProjectName, strEnvironment).Wait();

            //RUN 2020
            MainRequest().Wait();




            //deleteMainRequestMPINS(strLetterType, strProjectName, strEnvironment, "265536, 2681001, 3575246, 140202, 523391, 1456435, 1458283, 2149530, 3336488, 3373220, 3561514,265536, 1836271, 2637265, 3535674").Wait();

        }









        private static async Task getCount()
        {



            if (accessToken == null) //FIRST TIME ONLY!!!!
                accessToken = await GetAccessToken();


            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(documentUrl.Replace("documents/", "documents?count=1"));
            //NEW REQUEST HEADER
            req.Headers.Add("Authorization", "Bearer " + accessToken);
            string boundary = "------------------------" + DateTime.Now.Ticks.ToString("x");
            req.ContentType = "multipart/form-data; boundary=" + boundary;
            req.Accept = "application/json";  // GET
            req.Method = "GET";
            req.KeepAlive = true;


            using (var response = req.GetResponse())
            {
                Stream streamTmp = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(streamTmp);
                strResponse_GLOBAL = streamReader.ReadToEnd();
                //var objResponse = new JavaScriptSerializer().DeserializeObject(strResponse);
                responseContainer_GLOBAL = new JavaScriptSerializer().Deserialize<ResponseContainer>(strResponse_GLOBAL);
                response.Close();
            }

            //response.Close();


            //return Task.FromResult<object>(null);




            ////POST REQUEST TO UPLOAD METADATA AND FILE
            //using (Stream requestStream = req.GetRequestStream())
            //{
            //    memStream.Position = 0;
            //    byte[] tempBuffer = new byte[memStream.Length];
            //    memStream.Read(tempBuffer, 0, tempBuffer.Length);


            //    //ADDED TO CAPTURE AND LOG REQUEST
            //    strRequest_GLOBAL = System.Text.Encoding.Default.GetString(tempBuffer);

            //    memStream.Close();
            //    requestStream.Write(tempBuffer, 0, tempBuffer.Length);

            //}

            ////GET RESPONSE
            ////GET RESPONSE
            ////GET RESPONSE
            //using (var response = req.GetResponse())
            //{
            //    Stream streamTmp = response.GetResponseStream();
            //    StreamReader streamReader = new StreamReader(streamTmp);
            //    strResponse_GLOBAL = streamReader.ReadToEnd();
            //    //var objResponse = new JavaScriptSerializer().DeserializeObject(strResponse);
            //    responseContainer_GLOBAL = new JavaScriptSerializer().Deserialize<ResponseContainer>(strResponse_GLOBAL);
            //    response.Close();
            //}




        }



        private static async Task deleteMainRequest(string strLetterType, string strProjectName, string strEnvironment)
        {

            //string sql = "SELECT response_string from qa_link_request_tracker WHERE error_message is null AND letter_type = '" + strLetterType + "' AND  project_name = '" + strProjectName + "' AND environment = '" + strEnvironment + "'";
            string sql = "SELECT qa_link_request_tracker_id, response_string from qa_link_request_tracker WHERE error_message is null AND letter_type = '" + strLetterType + "' AND environment = '" + strEnvironment + "' and project_name = '"+strProjectName+"'";// and deleted = 0";
            dt = DBConnection32.getMSSQLDataTable(strConnectionString, sql);
            string strDocumentId = null;
            int intCnt = 1;
            foreach (DataRow dr in dt.Rows)
            {
                responseContainer_GLOBAL = new JavaScriptSerializer().Deserialize<ResponseContainer>(dr["response_string"].ToString());
                strDocumentId = responseContainer_GLOBAL.id;

                try
                {
                    if (accessToken == null) //FIRST TIME ONLY!!!!
                        accessToken = await GetAccessToken();
                    //accessToken = "e7888313-a238-4d63-9bb8-791febab3508";
                    await deleteFiles(documentUrl, accessToken, strDocumentId);
                    if (strError_GLOBAL != null)//FAILED, TRY TO GET NEW TOKEN
                    {
                        accessToken = await GetAccessToken();
                        await deleteFiles(documentUrl, accessToken, strDocumentId);
                    }



                    DBConnection32.ExecuteMSSQL(strConnectionString, "UPDATE qa_link_request_tracker SET deleted = 1 WHERE qa_link_request_tracker_id =" + dr["qa_link_request_tracker_id"].ToString());


                    Console.WriteLine(intCnt + " GOOD = " + strDocumentId  +  Environment.NewLine);

                }
                catch(Exception ex)
                {
                    Console.WriteLine(intCnt + " ERROR = " + strDocumentId + " " + ex.Message +  Environment.NewLine);
                }

                intCnt++;
            }

        }

        private static async Task deleteMainRequestMPINS(string strLetterType, string strProjectName, string strEnvironment, string strMPINS)
        {

            //string sql = "SELECT response_string from qa_link_request_tracker WHERE error_message is null AND letter_type = '" + strLetterType + "' AND  project_name = '" + strProjectName + "' AND environment = '" + strEnvironment + "'";
            string sql = "SELECT qa_link_request_tracker_id, response_string from qa_link_request_tracker WHERE error_message is null AND letter_type = '" + strLetterType + "' AND environment = '" + strEnvironment + "' and project_name = '" + strProjectName + "' and mpin in ("+ strMPINS +")";// and deleted = 0";
            dt = DBConnection32.getMSSQLDataTable(strConnectionString, sql);
            string strDocumentId = null;
            int intCnt = 1;
            foreach (DataRow dr in dt.Rows)
            {
                responseContainer_GLOBAL = new JavaScriptSerializer().Deserialize<ResponseContainer>(dr["response_string"].ToString());
                strDocumentId = responseContainer_GLOBAL.id;

                try
                {
                    if (accessToken == null) //FIRST TIME ONLY!!!!
                        accessToken = await GetAccessToken();
                    //accessToken = "e7888313-a238-4d63-9bb8-791febab3508";
                    await deleteFiles(documentUrl, accessToken, strDocumentId);
                    if (strError_GLOBAL != null)//FAILED, TRY TO GET NEW TOKEN
                    {
                        accessToken = await GetAccessToken();
                        await deleteFiles(documentUrl, accessToken, strDocumentId);
                    }



                    DBConnection32.ExecuteMSSQL(strConnectionString, "UPDATE qa_link_request_tracker SET deleted = 1 WHERE qa_link_request_tracker_id =" + dr["qa_link_request_tracker_id"].ToString());


                    Console.WriteLine(intCnt + " GOOD = " + strDocumentId + Environment.NewLine);

                }
                catch (Exception ex)
                {
                    Console.WriteLine(intCnt + " ERROR = " + strDocumentId + " " + ex.Message + Environment.NewLine);
                }

                intCnt++;
            }

        }

        //private static async Task TestMainRequest(string attachmentIdCSV = "select attachment_id from qa_link_request_tracker WHERE project_name = 'PCP_CH4_PR_Profiles_Physician'")
        private static async Task TestMainRequest(string attachmentIdCSV = "SELECT attachment_id FROM [IL_UCA].[dbo].[qa_link_request_tracker] WHERE project_name = 'SPEC_CH5_PR_PM_Profiles' and mpin in (101337,1261303,2737445 )")
        {
            string sql = "SELECT attachment_id, file_name from dbo.qa_link_request_tracker WHERE attachment_id in ("+ attachmentIdCSV + ")";
            dt = DBConnection32.getMSSQLDataTable(strConnectionString, sql);

            foreach(DataRow dr in dt.Rows)
            {
                if (accessToken == null) //FIRST TIME ONLY!!!!
                    accessToken = await GetAccessToken();
                //accessToken = "e7888313-a238-4d63-9bb8-791febab3508";
                await getFiles(attachmentUrl, accessToken, dr["attachment_id"].ToString(), fileDownloadTest  + dr["file_name"].ToString());
                if (strError_GLOBAL != null)//FAILED, TRY TO GET NEW TOKEN
                {
                    accessToken = await GetAccessToken();
                    await getFiles(attachmentUrl, accessToken, dr["attachment_id"].ToString(), fileDownloadTest  + dr["file_name"].ToString());
                }
            }

        }



       static  StringBuilder sbMissingMpins = new StringBuilder();
        private static async Task MainRequest()
        {
            string strNewFilename = null;
            int intCnt = 1;
            //GET FILES TO UPLOAD
            string[] files = Directory.GetFiles(fileReadTest, "*.pdf", SearchOption.AllDirectories);
            //string[] files = Directory.GetFiles(fileReadTest, "*.txt", SearchOption.AllDirectories);
            foreach (string sFile in files)
            {
               
                var strFileName = Path.GetFileName(sFile);
                strMPin = strFileName.Split('_')[0];
                dt.Clear();

                //if (strMPin != "2387742" && strMPin != "2388150" && strMPin != "238845" && strMPin != "2388818")
                //{
                //    Console.WriteLine(intCnt + " = skipping...." + Environment.NewLine);
                //    intCnt++;
                //    continue;
                //}
          
                //LOAD DATA FROM MPIN IN FILENAME TO DATATABLE
                dt = DBConnection32.getMSSQLDataTable(strConnectionString, strSQL.Replace("{?mpin}", strMPin));

                if (dt.Rows.Count == 0)
                {


                    //File.Move(sFile, @"C:\~ProjectProfiles\PCR_Specialty_ch4_SAS\2019 Done\Final\Garbage\" + strFileName);
                   // File.Delete(sFile);


                    Console.WriteLine(intCnt + " = skipping...." + Environment.NewLine);
                    intCnt++;
                    continue;
                }
                //else
                //    continue;

                if (strLetterType == "physician")
                    strNewFilename = dt.Rows[0]["P_LastName"].ToString().Replace(" ", "").Replace("\\", "").Replace("/", "").Replace("'", "").Replace("*", "").Replace("&", "_") + "_" + dt.Rows[0]["MPIN"].ToString() + "_PCR_Report_" + DateTime.Now.ToString("MMMM") + "_" + DateTime.Now.Year.ToString() + ".pdf";
                else if (strLetterType == "practice")
                    strNewFilename = dt.Rows[0]["Practice_Name"].ToString().Replace(" ", "").Replace("\\", "").Replace("/", "").Replace("'", "").Replace("*", "").Replace("&", "_") + "_PracticeManager_" + dt.Rows[0]["practice_id"].ToString() + "_PCR_Report_" + DateTime.Now.ToString("MMMM") + "_" + DateTime.Now.Year.ToString() + ".pdf";

                //GET ALL METADATA FOR THIS FILE
                md = new MetaData
                {
                    //fileName = strFileName,
                    fileName = strNewFilename,
                    fileDescription = "Peer Comparison Reports " + DateTime.Now.ToString("MMMM") + " " + DateTime.Now.Year.ToString(),
                    createdDate = DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ss.fffzzz"), //FDS UPLOAD
                    createApplication = "PCR",
                    expiryDate = DateTime.Now.AddYears(1).ToString("yyyy-MM-dd'T'HH:mm:ss.fffzzz"),
                    filePath = "Peer Comparison Reports",
                    tenant = "Link",
                    fileType = "Letter",
                    category = "PCR",
                    subCategory = "PCR",
                    privilege = "Patient Eligibility and Benefits",
                    corporateMpin = dt.Rows[0]["CorpMPIN"].ToString().PadLeft(9, '0'), //PAD WITH LEADING ZEROS TOTAL 9 CHARACTERS
                    //corporateMpin = dt.Rows[0]["MPIN"].ToString().PadLeft(9, '0'), //COMMENT AFTER TESTS!!!!
                    tin = dt.Rows[0]["taxid"].ToString()
                };

                //WRAP METADATA INTO CONTAINER
                mdC = new RequestContainer
                {
                    space_id = space_id,
                    external_id = external_id,
                    metadata = md

                };



                if (accessToken == null) //FIRST TIME ONLY!!!!
                    accessToken = await GetAccessToken();
                    //accessToken = "e7888313-a238-4d63-9bb8-791febab3508";

                await makeRequest(documentUrl, accessToken, mdC, sFile, strNewFilename);
                if(responseContainer_GLOBAL == null)//FAILED, TRY TO GET NEW TOKEN
                {
                    accessToken = await GetAccessToken();
                    await makeRequest(documentUrl, accessToken, mdC, sFile, strNewFilename);
                }


                string sql = null; 
                if (responseContainer_GLOBAL == null)//FAILED YET AGAIN, LOG AND NEXT
                {

                    sql = "INSERT INTO qa_link_request_tracker (request_string,external_id,space_id,environment, error_message, mpin,letter_type,project_name) VALUES ('" + strRequest_GLOBAL.Replace("'", "''") + "','" + external_id + "','" + space_id + "','" + strEnvironment + "','" + strError_GLOBAL.Replace("'", "''") + "', "+strMPin+ ", '" + strLetterType + "', '" + strProjectName + "')";

                    Console.WriteLine(strError_GLOBAL + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine);

                }
                else
                {
                    sql = strSQLInsert.Replace("{$request_string}", strRequest_GLOBAL.Replace("'", "''")).Replace("{$response_string}", strResponse_GLOBAL.Replace("'", "''")).Replace("{$external_id}", responseContainer_GLOBAL.external_id.Replace("'", "''")).Replace("{$space_id}", responseContainer_GLOBAL.attachments[0].space_id.Replace("'", "''")).Replace("{$status}", responseContainer_GLOBAL.attachments[0].status.Replace("'", "''")).Replace("{$dateCreated}", responseContainer_GLOBAL.attachments[0].date_created.Replace("'", "''")).Replace("{$attachment_id}", responseContainer_GLOBAL.attachments[0].id.Replace("'", "''")).Replace("{$file_name}", responseContainer_GLOBAL.attachments[0].file_name.Replace("'", "''")).Replace("{$file_size}", responseContainer_GLOBAL.attachments[0].file_size.Replace("'", "''")).Replace("{$environment}", strEnvironment).Replace("{$mpin}", strMPin).Replace("{$letter_type}", strLetterType).Replace("{$project_name}", strProjectName).Replace("{$document_id}", responseContainer_GLOBAL.id.Replace("'", "''"));
                }

                ///////////////REMOVED

                //STORE REQUEST/RESPONSE
                DBConnection32.ExecuteMSSQL(strConnectionString, sql);


                Console.WriteLine(intCnt + " = " + strResponse_GLOBAL + Environment.NewLine + Environment.NewLine );

                intCnt++;

                //if (intCnt > 12)
                //    break;

            }

            Console.ReadLine(); //Pause

        }

        private static Task makeRequest(string strUrl, string accessToken, RequestContainer metadataObject, string strFileToUploadPath, string strNewFileName)
        {
            responseContainer_GLOBAL = null;
            strError_GLOBAL = null;
            //string strFileName = Path.GetFileName(strFileToUploadPath);
            try
            {
                //CONVERT CONTAINER TO JSON STRING
                string jsonRequest = new JavaScriptSerializer().Serialize(metadataObject).Replace("\"", "'");

                //NEW REQUEST
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(strUrl);
                //NEW REQUEST HEADER
                req.Headers.Add("Authorization", "Bearer " + accessToken);
                string boundary = "------------------------" + DateTime.Now.Ticks.ToString("x");
                req.ContentType = "multipart/form-data; boundary=" + boundary;
                req.Accept = "application/json";  // GET
                req.Method = "POST";
                req.KeepAlive = true;

                //CREATE MEMORY STREAM TO STORE FULL REQUEST
                Stream memStream = new System.IO.MemoryStream();
                //ADD METADATA TO MEMORY STREAM
                var buffer = Encoding.UTF8.GetBytes(string.Format("\r\n--" + boundary + "\r\n" + "Content-Disposition: form-data; name=\"{0}\";\r\n\r\n{1}", "documents", jsonRequest));
                memStream.Write(buffer, 0, buffer.Length);


                //ADD FILE INFO TO MEMORY STREAM
                buffer = Encoding.UTF8.GetBytes(string.Format("\r\n\r\n--" + boundary + "\r\n" + "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n\r\n", "file_01", strNewFileName));
                memStream.Write(buffer, 0, buffer.Length);
                string fileExt = strNewFileName.Substring(strNewFileName.Length - 3, 3);
                switch (fileExt.ToUpper())
                {
                    case "PDF":
                        buffer = Encoding.UTF8.GetBytes(string.Format("Content-Type: {0}{1}{1}", "application/octet-stream", "\r\n"));
                        memStream.Write(buffer, 0, buffer.Length);
                        break;
                    case "TXT":
                        buffer = Encoding.UTF8.GetBytes(string.Format("Content-Type: {0}{1}{1}", "text/plain", "\r\n"));
                        memStream.Write(buffer, 0, buffer.Length);
                        break;
                        //OTHER FILE TYPES HERE.....
                }

                //ADD FILE CONTENTS TO MEMORY STREAM
                byte[] fileBytesArr = null;
                // Open file for reading
                System.IO.FileStream _FileStream = new System.IO.FileStream(strFileToUploadPath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                // attach filestream to binary reader
                System.IO.BinaryReader _BinaryReader = new System.IO.BinaryReader(_FileStream);
                // get total byte length of the file
                long _TotalBytes = new System.IO.FileInfo(strFileToUploadPath).Length;
                // read entire file into buffer
                fileBytesArr = _BinaryReader.ReadBytes((Int32)_TotalBytes);
                // close file reader
                _FileStream.Close();
                _FileStream.Dispose();
                _BinaryReader.Close();

                memStream.Write(fileBytesArr, 0, fileBytesArr.Length);
                buffer = Encoding.ASCII.GetBytes("\r\n");
                memStream.Write(buffer, 0, buffer.Length);


                //ADD CLOSING BOUNDS TO MEMORY STREAM
                var boundaryBuffer = Encoding.UTF8.GetBytes(boundary + "--");
                memStream.Write(boundaryBuffer, 0, boundaryBuffer.Length);

                //POST REQUEST TO UPLOAD METADATA AND FILE
                using (Stream requestStream = req.GetRequestStream())
                {
                    memStream.Position = 0;
                    byte[] tempBuffer = new byte[memStream.Length];
                    memStream.Read(tempBuffer, 0, tempBuffer.Length);


                    //ADDED TO CAPTURE AND LOG REQUEST
                    strRequest_GLOBAL = System.Text.Encoding.Default.GetString(tempBuffer);

                    memStream.Close();
                    requestStream.Write(tempBuffer, 0, tempBuffer.Length);

                }

                //GET RESPONSE
                //GET RESPONSE
                //GET RESPONSE
                using (var response = req.GetResponse())
                {
                    Stream streamTmp = response.GetResponseStream();
                    StreamReader streamReader = new StreamReader(streamTmp);
                    strResponse_GLOBAL = streamReader.ReadToEnd();
                    //var objResponse = new JavaScriptSerializer().DeserializeObject(strResponse);
                    responseContainer_GLOBAL = new JavaScriptSerializer().Deserialize<ResponseContainer>(strResponse_GLOBAL);
                    response.Close();
                }
            }
            catch(Exception ex)
            {
                strError_GLOBAL = ex.ToString();
                responseContainer_GLOBAL = null;
            }

            return Task.FromResult<object>(null);
        }



        private static async Task<string> GetAccessToken()
        {
            HttpResponseMessage response = null;
            object responseData = null;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(tokenUrl);

                    // We want the response to be JSON.
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // Build up the data to POST.


                    Dictionary<string, string> postDict = new Dictionary<string, string>();
                    postDict.Add("client_id", clientId);
                    postDict.Add("client_secret", clientSecret);
                    postDict.Add("grant_type", "authorization_code");
                    postDict.Add("Content-Type", "application/x-www-form-urlencoded");

                    FormUrlEncodedContent content = new FormUrlEncodedContent(postDict);

                    response = await client.PostAsync("", content);
                    string jsonString = await response.Content.ReadAsStringAsync();
                    responseData = JsonConvert.DeserializeObject(jsonString);

                }
            }
            catch (Exception ex)
            {
                strError_GLOBAL = ex.ToString();
                return null;
            }

            return await Task.FromResult(((dynamic)responseData).access_token);
        }



        private static Task getFiles(string strUrl, string accessToken, string attachmentId, string strNewFileAndPath)
        {
            strError_GLOBAL = null;
            string boundary = "------------------------" + DateTime.Now.Ticks.ToString("x");

            //GET ATTACHMENT ID FROM RESPONSE TO TEST ATTACHMENT
            //GET ATTACHMENT ID FROM RESPONSE TO TEST ATTACHMENT
            //GET ATTACHMENT ID FROM RESPONSE TO TEST ATTACHMENT
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.Headers[HttpRequestHeader.Authorization] = Convert.ToString("Bearer ") + accessToken;
                    webClient.Headers[HttpRequestHeader.Accept] = "application/json";
                    webClient.Headers.Add("Content-Type", "multipart/form-data; boundary=" + boundary);
                    webClient.DownloadFile(strUrl.Replace("{$attachment_id}", attachmentId), strNewFileAndPath);

                }
            }
            catch(Exception ex)
            {
                strError_GLOBAL = ex.ToString();
            }

            return Task.FromResult<object>(null);
        }



      




        private static Task deleteFiles(string strUrl, string accessToken, string documentId)
        {


            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(strUrl + documentId);
            //NEW REQUEST HEADER
            req.Headers.Add("Authorization", "Bearer " + accessToken);
            string boundary = "------------------------" + DateTime.Now.Ticks.ToString("x");
            req.ContentType = "multipart/form-data; boundary=" + boundary;
            req.Accept = "application/json";  // GET
            req.Method = "DELETE";
            req.KeepAlive = true;


            var response = req.GetResponse();

            response.Close();


            return Task.FromResult<object>(null);

        }




        ////CONVERT CONTAINER TO JSON STRING
        //string jsonRequest = new JavaScriptSerializer().Serialize(mdC).Replace("\"", "'");




        ////NEW REQUEST
        //HttpWebRequest req = (HttpWebRequest)WebRequest.Create(documentUrl);
        ////NEW REQUEST HEADER
        //req.Headers.Add("Authorization", "Bearer " + accessToken);
        //string boundary = "------------------------" + DateTime.Now.Ticks.ToString("x");
        //req.ContentType = "multipart/form-data; boundary=" + boundary;
        //req.Accept = "application/json";  // GET
        //req.Method = "POST";
        //req.KeepAlive = true;

        ////CREATE MEMORY STREAM TO STORE FULL REQUEST
        //Stream memStream = new System.IO.MemoryStream();
        ////ADD METADATA TO MEMORY STREAM
        //var buffer = Encoding.UTF8.GetBytes(string.Format("\r\n--" + boundary + "\r\n" + "Content-Disposition: form-data; name=\"{0}\";\r\n\r\n{1}", "documents", jsonRequest));
        //memStream.Write(buffer, 0, buffer.Length);


        ////ADD FILE INFO TO MEMORY STREAM
        //buffer = Encoding.UTF8.GetBytes(string.Format("\r\n\r\n--" + boundary + "\r\n" + "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n\r\n", "file_01", strFileName));
        //memStream.Write(buffer, 0, buffer.Length);
        //string fileExt = strFileName.Substring(strFileName.Length - 3, 3);
        //switch (fileExt.ToUpper())
        //{
        //    case "PDF":
        //        buffer = Encoding.UTF8.GetBytes(string.Format("Content-Type: {0}{1}{1}", "application/octet-stream", "\r\n"));
        //        memStream.Write(buffer, 0, buffer.Length);
        //        break;
        //    case "TXT":
        //        buffer = Encoding.UTF8.GetBytes(string.Format("Content-Type: {0}{1}{1}", "text/plain", "\r\n"));
        //        memStream.Write(buffer, 0, buffer.Length);
        //        break;
        //    //OTHER FILE TYPES HERE.....
        //}

        ////ADD FILE CONTENTS TO MEMORY STREAM
        //byte[] fileBytesArr = null;
        //// Open file for reading
        //System.IO.FileStream _FileStream = new System.IO.FileStream(strFilePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
        //// attach filestream to binary reader
        //System.IO.BinaryReader _BinaryReader = new System.IO.BinaryReader(_FileStream);
        //// get total byte length of the file
        //long _TotalBytes = new System.IO.FileInfo(strFilePath).Length;
        //// read entire file into buffer
        //fileBytesArr = _BinaryReader.ReadBytes((Int32)_TotalBytes);
        //// close file reader
        //_FileStream.Close();
        //_FileStream.Dispose();
        //_BinaryReader.Close();

        //memStream.Write(fileBytesArr, 0, fileBytesArr.Length);
        //buffer = Encoding.ASCII.GetBytes("\r\n");
        //memStream.Write(buffer, 0, buffer.Length);


        ////ADD CLOSING BOUNDS TO MEMORY STREAM
        //var boundaryBuffer = Encoding.UTF8.GetBytes(boundary + "--");
        //memStream.Write(boundaryBuffer, 0, boundaryBuffer.Length);

        ////POST REQUEST TO UPLOAD METADATA AND FILE
        //string strRequest = null;
        //using (Stream requestStream = req.GetRequestStream())
        //{
        //    memStream.Position = 0;
        //    byte[] tempBuffer = new byte[memStream.Length];
        //    memStream.Read(tempBuffer, 0, tempBuffer.Length);


        //    //ADDED TO CAPTURE AND LOG REQUEST
        //    strRequest = System.Text.Encoding.Default.GetString(tempBuffer);


        //    memStream.Close();
        //    requestStream.Write(tempBuffer, 0, tempBuffer.Length);

        //}

        ////GET RESPONSE
        ////GET RESPONSE
        ////GET RESPONSE
        //ResponseContainer ResponseContainer = null;
        //string strResponse = null;
        //using (var response = req.GetResponse())
        //{
        //    Stream streamTmp = response.GetResponseStream();
        //    StreamReader streamReader = new StreamReader(streamTmp);
        //    strResponse = streamReader.ReadToEnd();

        //    //var objResponse = new JavaScriptSerializer().DeserializeObject(strResponse);
        //    //ResponseContainer = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseContainer>(strResponse);
        //    ResponseContainer = new JavaScriptSerializer().Deserialize<ResponseContainer>(strResponse);
        //}























    }

}
