using iTextSharp.text.pdf;
using iTextSharp.text.xml.xmp;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Web;

namespace Link_Optum_Services
{
    public class DataObject
    {
        public string Name { get; set; }
    }


    class Link_Optum_Services
    {

        //API DEFINITIONS URL
        //http://10.87.13.38/federation/2



        //DEV
        static string clientId = "76af0ded169147cf84999b178956ee72";
        static string clientSecret = "9d8052a42b5c4fc1a951f632e00c00d9";

        //PROD
        //clientId = "4fb7ee001b774a66b648bc2a3b7b018e";
        //clientSecret = "7f1abad570c847d2b5ab549203334cf5";

        //DEV
        static string tokenUrl = "https://fs2-stagedmz-ose3.optum.com/v2/oauth2/token.json";
        static string attachmentUrl = "https://fs2-stagedmz-ose3.optum.com/v2/attachments?space_id=8919";
        //static string documentUrl = "https://fs2-stagedmz-ose3.optum.com/v2/documents/";
        static string documentUrl = "https://fs2-stagedmz-ose3.optum.com/v2/documents"; /// <summary>
                                                                                         /// ?space_id=8919
                                                                                         /// </summary>
        //static string documentUrl = "https://fs2-stagedmz-ose3.optum.com/v2/attachments?space_id=8919";

        //PROD
        //static string tokenUrl = "https://federateddata.optum.com/v2/oauth2/token.json";
        //static string documentUrl = "https://federateddata.optum.com/v2/attachments?space_id=8918";
        //static string attachmentUrl = "https://federateddata.optum.com/v2/documents/";


        static string accessToken = null;
        static HttpClient httpClient;
        static ByteArrayContent byteArrayContent;
        static MultipartFormDataContent multipartFormDataContent;
        static byte[] bytes;
        static HttpResponseMessage response;

        static string strFilePath;
        static string strFileName;



        static string strConnectionString = "data source = IL_UCA; server=wn000005325;Persist Security Info=True;database=IL_UCA;Integrated Security = SSPI; connect timeout = 300000;";

        static string strSQL = "select a.MPIN,a.attr_clients as clients,LastName,FirstName,P_LastName,P_FirstName,ProvDegree, a.Spec_display as NDB_Specialty, b.Street,b.City,b.[State],b.zipcd,b.taxid, p.MPIN as practice_id,p.Practice_Name,Tot_Util_meas,Tot_PX_meas, RCMO,RCMO_title,RCMO_title1,Special_Handling,Folder_Name from dbo.PBP_Outl_Ph32 as a inner join dbo.PBP_outl_demogr_ph32 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_ph32 as p on p.mpin=PTIGroupID_upd inner join dbo.PBP_spec_handl_ph32 as h on h.MPIN=a.mpin inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM where a.Exclude in(0,5) and r.phase_id=2 and a.MPIN = {?mpin}";
        static string strMPin = "";
        static DataTable dtMetaData;

        static void Main(string[] args)
        {

            //readWritePDFMetaData();
            dtMetaData = new DataTable();

            //API CALL
            //string[] files = Directory.GetFiles(fileReadFolder, "*.pdf", SearchOption.AllDirectories);
            //string[] files = Directory.GetFiles(fileReadTest, "*.txt", SearchOption.AllDirectories);
            string[] files = Directory.GetFiles(fileReadTest, "*.pdf", SearchOption.AllDirectories);
            foreach (string s in files)
            {
                strFilePath = s;
                strFileName = Path.GetFileName(s);
                strMPin = strFileName.Split('_')[0];
                dtMetaData.Clear();
                dtMetaData = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL.Replace("{?mpin}", strMPin));

                MainAsync().Wait();




               



                MultipartForm form = new MultipartForm(documentUrl);
                form.AccessToken = accessToken;
                //form.FileContentType = "application/json";
                //form.FileContentType = "application/x-www-form-urlencoded";
                //form.SetField("fileName", strFileName);
                //form.SetField("fileDescription", strFileName + "_TestDescription");
                ////form.SetField("createdDate", DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss.SSSZ"));
                //form.SetField("createdDate", DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss.fffZ"));
                //form.SetField("corporateMpin", dtMetaData.Rows[0]["practice_id"].ToString());
                //form.SetField("tin", dtMetaData.Rows[0]["taxid"].ToString());
                //form.SetField("mpin", dtMetaData.Rows[0]["MPIN"].ToString());




                string phyName = (dtMetaData.Rows[0]["P_LastName"] != DBNull.Value ? (dtMetaData.Rows[0]["P_FirstName"].ToString().Trim() + " " + dtMetaData.Rows[0]["P_LastName"].ToString().Trim()) : "NAME MISSING");


                form.SetField("fileName", strFileName);
                form.SetField("fileDescription", strFileName + "_TestDescription"); //?????
                //form.SetField("createdDateOLD", DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss.fffZ"));
                //form.SetField("createdDateOLD2", DateTime.Now.ToString("yyyy-MM-dd”T”HH:mm:ss.SSSXXX"));




                form.SetField("createdDate", DateTime.Now.ToString("yyyy -MM-dd'T'HH:mm:sszzz", CultureInfo.InvariantCulture));


                form.SetField("createApplication", "Informatics and Analytical Solutions");
                //form.SetField("expiryDateOLD", DateTime.Now.AddYears(2).ToString("yyyy-MM-ddThh:mm:ss.fffZ"));
                //form.SetField("expiryDateOLD2", DateTime.Now.ToString("yyyy-MM-dd”T”HH:mm:ss.SSSXXX"));
                form.SetField("expiryDate", DateTime.Now.ToString("yyyy -MM-dd'T'HH:mm:sszzz", CultureInfo.InvariantCulture));

                form.SetField("filePath", "//?/?/?");
                form.SetField("tenant", "Link");
                form.SetField("claimNumber", "NULL");
                form.SetField("fileType", "Letter");
                form.SetField("category", "Claims");
                form.SetField("subCategory", "Claims/Information");
                form.SetField("privilege", "RPRT"); //17?????
                form.SetField("organization","UHC");
                form.SetField("corporateMpin", dtMetaData.Rows[0]["practice_id"].ToString());
                form.SetField("tin", dtMetaData.Rows[0]["taxid"].ToString());
                form.SetField("mpin", dtMetaData.Rows[0]["MPIN"].ToString());
                form.SetField("memberId", "NULL");
                form.SetField("notificationReq", "NULL");
                form.SetField("emailAlertReq", "NULL");
                form.SetField("physicianName", phyName);
                form.SetField("policyNumber", "NULL");
                form.SetField("employeeName", "NULL");
                form.SetField("dateOfService", "NULL");
                form.SetField("memberName", "NULL");
                form.SetField("providerEmailId", "NULL");
                form.SetField("frequency", "NULL");
                form.SetField("weekDay", "NULL");


                form.SendFile(strFilePath);
                Console.WriteLine(form.ResponseText.ToString() + Environment.NewLine + Environment.NewLine + Environment.NewLine);

                //Console.WriteLine(response.RequestMessage.ToString() + Environment.NewLine + response.ToString() + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine);
            }

  
            //Console.ReadLine(); //Pause


        }


        private static async Task MainAsync()
        {


            accessToken = await GetAccessToken();



            //documentUrl = "https://fs2-stagedmz-ose3.optum.com/v2/documents";
            //documentUrl = "https://fs2-stagedmz-ose3.optum.com/v2/attachments?space_id=8919";


            string boundary = "------------------------" + DateTime.Now.Ticks.ToString("x");


            var webClient = new WebClient();
            webClient.Headers[HttpRequestHeader.Authorization] = Convert.ToString("Bearer ") + accessToken;
            webClient.Headers[HttpRequestHeader.Accept] = "application/json";
            webClient.Headers.Add("Content-Type", "multipart/form-data; boundary=" + boundary);

            //////////////////////////////FILEUPLOAD START
            bytes = System.IO.File.ReadAllBytes(strFilePath);
            var fileData = webClient.Encoding.GetString(bytes);



            var package = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"file\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n{3}\r\n--{0}--\r\n", boundary, strFileName, "multipart/form-data", fileData);
            //var package = "\"{ 'external_id':123456, 'space_id':8919, 'store_input_files': false}\" -F \"" + strFileName + "\"  \"" + documentUrl + "\" --header \"Content-Type:multipart/form-data\" --header \"Authorization: Bearer " + accessToken + "\"";


            //string packageInner = "{\"external_id\":\"CSG1234\",\"space_id\":8919,\"store_input_files\":\"true\"}";

            //var package = string.Format("--{0}\r\nContent-Disposition:form-data;name=\"file\";filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n{3}\r\n--{0}--\r\n", boundary, strFileName, "multipart/form-data", fileData);


            //var package = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"file\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n{3}\r\n--{0}--\r\n", boundary, strFileName, "multipart/form-data", fileData);



            //curl - k - v - F 'documents="{'space_id':8888,'external_id':'11hello1234','metadata':{'fileName' ': 'Ganesh Test Doc Vault Functionality.pdf',  'fileDescription' : 'Test Doc Vault','createApplication' : 'FDS Test',  'expiryDate' : '2018-08-09T00:00:00.000-05:00',  'filePath' : 'EOC/FEB2017',  'tenant' : 'LINK',  'fileType' : 'Report',  'category' : 'Episodes of Care',  'subCategory' : 'Provider Report',  'privilege' : 'Patient Eligibility and Benefits',  'organization' : 'Optum Technologies',  'corporateMpin' : 'ALL',  'tin' : 'ALL',  'notificationReq' : 'Y',  'emailAlertReq' : 'N',  'testHarness' : 'true'},'workflow':{'process':{'process_key':'transformMergeProcess', 'output_file_name':'outputMergedFile.pdf'}}}" -F test01=@test1.doc  -F test02=@"test2.txt" --header "Authorization: bearer $token" $url/v2/documents


            var values = new[]
            {
                new KeyValuePair<string, string>("Content-Disposition", "form-data"),
                new KeyValuePair<string, string>("name", "file"),
                new KeyValuePair<string, string>("filename", strFileName),
                new KeyValuePair<string, string>("Content-Type", "multipart/form-data"),
                new KeyValuePair<string, string>("documents", fileData),
                new KeyValuePair<string, string>("external_id", "123456"),
                new KeyValuePair<string, string>("space_id", "8919"),
                new KeyValuePair<string, string>("store_input_files", "true"),
            };

            //foreach (var keyValuePair in values)
            //{
            //    content.Add(new StringContent(keyValuePair.Value), keyValuePair.Key);
            //}



            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(documentUrl);

                // We want the response to be JSON.
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + accessToken);



                // Build up the data to POST.


                Dictionary<string, string> postDict = new Dictionary<string, string>();
                postDict.Add("Content-Disposition", "form-data");
                postDict.Add("name", "file");
                postDict.Add("filename", strFileName);
                postDict.Add("Content-Type", "multipart/form-data");
                //postDict.Add("Content-Type", "application/x-www-form-urlencoded");

                postDict.Add("documents", fileData);
                postDict.Add("external_id", "123456");
                postDict.Add("space_id", "8919");
                postDict.Add("store_input_files", "true");



                FormUrlEncodedContent content = new FormUrlEncodedContent(postDict);

                // Post to the Server and parse the response.
                //HttpResponseMessage response = await client.PostAsync("Token", content);
                HttpResponseMessage response = await client.PostAsync(documentUrl, content);
                string jsonString = await response.Content.ReadAsStringAsync();
                object responseData = JsonConvert.DeserializeObject(jsonString);

                // return the Access Token.
               // return ((dynamic)responseData).access_token;
            }

































            var documents = new Documents();
            documents.documents = new Document
            {

                space_id = 8919,
                external_id = "123456",
                metadata = new Metadata
                {
                    fileName = strFileName,
                    fileDescription = strFileName + "_TestDescription"
                }
            };

            var json = JsonConvert.SerializeObject(documents);
           // var json = JsonConvert.SerializeObject(values);
            var nfile = webClient.Encoding.GetBytes(json);
            //var nfile = webClient.Encoding.GetBytes(package);
            //webClient.Encoding = Encoding.UTF8;
            //var nfile = webClient.Encoding.GetBytes(package);


            //byte[] resp = null;
            string resp = null;
            try
            {
                //resp = webClient.UploadData(documentUrl, "POST", nfile);
                resp = webClient.UploadString(documentUrl, "POST", json );
            }
            catch (Exception ex)
            {
                string exc = ex.ToString();
            }






            // byte[] resp = webClient.UploadData(documentUrl, "POST", package);



            //string Content = System.Text.Encoding.ASCII.GetString(resp);



            //System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
            //object RawData = jss.DeserializeObject(Content);


            ////////////////////////////////METADATA START
            ////string json = JsonConvert.SerializeObject(new MetaData { fileName = "TestName", fileDescription = "TestDescription" });
            ////string strResponse =  webClient.UploadString(documentUrl, "POST", json);


            //RawData = RawData;


















            //var client = new RestClient(documentUrl);

            //client.AddDefaultHeader("Content-Type", "application/json"); //I expect this to be enough

            // var request = new RestRequest();

            // //request.RequestFormat = DataFormat.Json;
            //// request.JsonSerializer.ContentType = "application/json; charset=utf-8";
            // request.Method = Method.POST;

            // //request.AddHeader("Accept", "application/json");
            // request.AddHeader("Content-Type", "application/json");
            // request.AddHeader("Authorization", "Bearer " + accessToken);
            // request.Parameters.Clear();

            // //request.AddHeader("Content-Type", "application/json");

            // //request.AddFile("filename", strFilePath, "text/plain");



            // //        //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("multipart/form-data"));
            // //        client.DefaultRequestHeaders.Add("Authorization", "bearer " + accessToken);


            // //request.AddParameter("Content-Type", "application/json");
            // //request.AddParameter("application/json", "test", ParameterType.RequestBody);

            // //request.AddParameter("external_id", "123456");
            // //request.AddParameter("space_id", "8919");
            // //request.AddParameter("store_input_files", "false");
            // request.RequestFormat = DataFormat.Json;
            // request.AddBody("test:t");

            // //        client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
            // //        client.DefaultRequestHeaders.Add("MIME-Version", "1.0");


            // IRestResponse response = client.Execute(request);




            // string s = "";




            //client.AddDefaultHeader("Content-Type", "application/json"); //I expect this to be enough
            //var req = new RestRequest("api/values");
            //req.AddHeader("Content-Type", "application/json"); //but its not so i tried adding it here
            //req.AddParameter("application/json", "test", ParameterType.RequestBody); //which didnt work so i tried adding it here
            //req.Method = Method.POST;

            //var resp = client.Execute(req);








            //using (var client = new HttpClient())
            //{
            //    using (var content =
            //        new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture)))
            //    {

            //        bytes = System.IO.File.ReadAllBytes(strFilePath);


            //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //        //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("multipart/form-data"));
            //        client.DefaultRequestHeaders.Add("Authorization", "bearer " + accessToken);


            //        client.DefaultRequestHeaders.Add("external_id", "123456");
            //        client.DefaultRequestHeaders.Add("space_id", "8919");
            //        client.DefaultRequestHeaders.Add("store_input_files", "false");


            //        client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
            //        client.DefaultRequestHeaders.Add("MIME-Version", "1.0");





            //        content.Add(new StreamContent(new MemoryStream(bytes)), "documents", strFileName);


            //        HttpResponseMessage response = await client.PostAsync(documentUrl, content);
            //        string jsonString = await response.Content.ReadAsStringAsync();
            //        object responseData = JsonConvert.DeserializeObject(jsonString);


            //    }
            //}


            //using (var client = new HttpClient())
            //{


            //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //    //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("multipart/form-data"));
            //    client.DefaultRequestHeaders.Add("Authorization", "bearer " + accessToken);
            //    client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
            //    client.DefaultRequestHeaders.Add("MIME-Version", "1.0");



            //    using (var content = new MultipartFormDataContent())
            //    {
            //        var values = new[]
            //        {
            //            new KeyValuePair<string, string>("external_id", "123456"),
            //            new KeyValuePair<string, string>("space_id", "8919"),
            //            new KeyValuePair<string, string>("store_input_files", "true"),
            //        };

            //        foreach (var keyValuePair in values)
            //        {
            //            content.Add(new StringContent(keyValuePair.Value), keyValuePair.Key);
            //        }

            //        var fileContent = new ByteArrayContent(System.IO.File.ReadAllBytes(strFilePath));
            //        fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("documents")
            //        {
            //            FileName = strFileName
            //        };
            //        content.Add(fileContent);


            //        var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(content));


            //        var fileDatad = Encoding.UTF8.GetString(bytes);
            //        //var nfile = webClient.Encoding.GetBytes(package);

            //        var result = client.PostAsJsonAsync(documentUrl, fileDatad).Result;


            //        //var result = client.PostAsJsonAsync(documentUrl, content).Result;

            //        //request.Content.ReadAsMultipartAsync().Result.Contents,


            //        //var result = client.PostAsync(documentUrl, content).Result;
            //    }
            //}



            //return;





            //var package = string.Format("--{0}\r\nContent-Disposition: form-data; space_id=\"8919\"; name=\"file\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n{3}\r\n--{0}--\r\n", boundary, strFileName, "multipart/form-data", fileData);
            // ?space_id = 8919



            //var package = "\"documents ={ 'external_id':123456, 'space_id':8919, store_input_files: false}\" -F \""+strFileName+"\"  \""+ documentUrl + "\" --header \"Content-Type:multipart/form-data\" --header \"Authorization: Bearer " + accessToken + "\"";






            //var package = "\"documents ={ 'external_id':123456, 'space_id':8919, 'store_input_files':false}\" -F \"" + strFileName + "\"  \"" + documentUrl + "\" --header \"Content-Type:multipart/form-data\" --header \"Authorization: Bearer " + accessToken + "\"";



            //var package = "\"documents ={ 'external_id':123456, 'space_id':8919, 'store_input_files':false}\" -F \"" + strFileName + "\"  \"" + documentUrl + "\" --header \"Content-Type:multipart/form-data\" --header \"Authorization: Bearer " + accessToken + "\"";


            //var package = "\"documents ={ 'external_id':123456, 'space_id':8919, 'store_input_files':false}\" -F \"file_01=@" + strFileName + "\"  \"" + documentUrl + "\" --header \"Content-Type:multipart/form-data\" \r\n\r\n12345\r\n --header \"Authorization: Bearer " + accessToken + "\"";





            //string  package = "'documents=\"{'external_id':'CSG1234',  'space_id':8919,   'store_input_files':'true'}\"'";

            //string package = "'documents = {'external_id':'CSG1234',  'space_id':8919, 'store_input_files':true}'";


            //string package = "'documents=\"{'external_id':'CSG1234',  'space_id':8919, 'store_input_files':true}\"'";


            // { "external_id":"1anesh1234", "space_id":8919, "store_input_files":"true"}




            //{ "external_id":"Ganesh1234",  "space_id":"8919",   "store_input_files":"true"}

            // curl - k - v - F 'documents="{'space_id':8888,'external_id':'11hello1234','metadata':{'fileName' ': 'Ganesh Test Doc Vault Functionality.pdf',  'fileDescription' : 'Test Doc Vault','createApplication' : 'FDS Test',  'expiryDate' : '2018-08-09T00:00:00.000-05:00',  'filePath' : 'EOC/FEB2017',  'tenant' : 'LINK',  'fileType' : 'Report',  'category' : 'Episodes of Care',  'subCategory' : 'Provider Report',  'privilege' : 'Patient Eligibility and Benefits',  'organization' : 'Optum Technologies',  'corporateMpin' : 'ALL',  'tin' : 'ALL',  'notificationReq' : 'Y',  'emailAlertReq' : 'N',  'testHarness' : 'true'},'workflow':{'process':{'process_key':'transformMergeProcess', 'output_file_name':'outputMergedFile.pdf'}}}" -F test01=@test1.doc  -F test02=@"test2.txt" --header "Authorization: bearer $token" $url/v2/documents 2:36 PM 
            //{ "external_id":"Ganesh1234",  "space_id":"8919",   "store_input_files":"true"}





            //curl - k - v - F 'documents="{'space_id':8888,'external_id':'11hello1234','metadata':{'fileName' ': 'Ganesh Test Doc Vault Functionality.pdf',  'fileDescription' : 'Test Doc Vault','createApplication' : 'FDS Test',  'expiryDate' : '2018-08-09T00:00:00.000-05:00',  'filePath' : 'EOC/FEB2017',  'tenant' : 'LINK',  'fileType' : 'Report',  'category' : 'Episodes of Care',  'subCategory' : 'Provider Report',  'privilege' : 'Patient Eligibility and Benefits',  'organization' : 'Optum Technologies',  'corporateMpin' : 'ALL',  'tin' : 'ALL',  'notificationReq' : 'Y',  'emailAlertReq' : 'N',  'testHarness' : 'true'},'workflow':{'process':{'process_key':'transformMergeProcess', 'output_file_name':'outputMergedFile.pdf'}}}" -F test01=@test1.doc  -F test02=@"test2.txt" --header "Authorization: bearer $token" $url/v2/documents







            //                curl - k - v - F documents = "{'space_id':'8888','external_id':'11hello1234','metadata':{'fileName' : 'Ganesh Test Doc Vault Functionality.pdf',  'fileDescription' : 'Test Doc Vault','createApplication' : 'FDS Test',  'expiryDate' : '2018-08-09T00:00:00.000-05:00',  'filePath' : 'EOC/FEB2017',  'tenant' : 'LINK',  'fileType' : 'Report',  'category' : 'Episodes of Care',  'subCategory' : 'Provider Report',  'privilege' : 'Patient Eligibility and Benefits',  'organization' : 'Optum Technologies',  'corporateMpin' : 'ALL',  'tin' : 'ALL',  'notificationReq' : 'Y',  'emailAlertReq' : 'N',  'testHarness' : 'true'},'workflow':{'process':{'process_key':'transformMergeProcess', 'output_file_name':'outputMergedFile.pdf'}}}" - F test01 = @test1.doc - F test02 = @"test2.txt"--header "Authorization: bearer $token" $url / v2 / documents
            //#curl -k -v -F documents="{'space_id':'8888','external_id':'Testing123','metadata' : {'fileDescription' : 'Claim status update','employeeName' : 'Testing Files', 'fileName' : '132988004_1111111.pdf',  'subCategory' : 'C2','filePath' : 'Claim Letters/Acknowledgements','policyNumber' : '905973','memberName' : 'GANESH TEST','privilege' : 'Claim Status','expiryDate' : '2018-01-22T17:13:01.346Z', 'createdDate' : '2017-01-15T18:06:00.000-06:00', 'physicianName' : 'CORA HEALTH SERVICES INC', 'providerId' : 'ELGS', 'createApplication' : 'Link', 'tin' : '341853567','dateOfService' : '2016-01-27T18:10:00.000-06:00','category' : 'Claim', 'claimNumber' : '6215156482', 'tenant' : 'Link','fileType' : 'Letter', 'memberId' : '593644951','corporateMpin' : '001913953'},'workflow':{'process':{'process_key':'transformMergeProcess', 'output_file_name':'132988004_1111111.pdf'}}}" -F test01=@test1.doc  -F test02=@"test2.txt" --header "Authorization: bearer $token" $url/v2/documents
            ////Dictionary <string, string> DictObjData = jss.ConvertToType(new Dictionary<String, String>)(RawData["virusscan"][0]);



            //curl - k - verbose - F "documents={'space_id':<space id value>,  'metadata' : { 'providerId': 'sample_provider', 'fileName': 'birds3.jpg', 'fileDescription': 'very simple file', 'createdDate': '2016-12-21T12:42:36.247+05:30', 'createApplication': 'sample application', 'expiryDate': '2050-12-21T12:42:36.247+05:30', 'filePath': 'reports/FEB2017', 'tenant': 'uhcbo', 'claimNumber': '12345678', 'fileType': 'Letter', 'category': 'Benefits', 'subCategory': 'Transactions', 'privilege': 'Electronic Payments and Statements', 'organization': 'Optum Technologies', 'corporateMpin': '123456789', 'tin': '234567891', 'mpin': '12345', 'memberId': '345678912', 'physicianName': 'Smith,Ann', 'policyNumber': '4567891', 'employeeName': 'Smith, John', 'dateOfService': '2016-12-21T12:42:36.247+05:30', 'memberName': 'Brown, Sue', 'notificationReq': 'N', 'emailAlertReq': 'Y' }}" "$HOST_URL/v2/documents" - F "file_01=@close-active.png" - F "file_02=@test22.doc"--header "Authorization: Bearer <AUTH_TOKEN>"


        }



        private static async Task<string> GetAccessToken()
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

                // Post to the Server and parse the response.
                //HttpResponseMessage response = await client.PostAsync("Token", content);
                HttpResponseMessage response = await client.PostAsync("", content);
                string jsonString = await response.Content.ReadAsStringAsync();
                object responseData = JsonConvert.DeserializeObject(jsonString);

                // return the Access Token.
                return ((dynamic)responseData).access_token;
            }
        }


        private static async Task OLDAttempts()
        {

            accessToken = await GetAccessToken();

            string boundary = "------------------------" + DateTime.Now.Ticks.ToString("x");
            bytes = System.IO.File.ReadAllBytes(strFilePath);

            var webClient = new WebClient();
            webClient.Headers[HttpRequestHeader.Authorization] = Convert.ToString("Bearer ") + accessToken;
            webClient.Headers[HttpRequestHeader.Accept] = "application/json";
            webClient.Headers.Add("Content-Type", "multipart/form-data; boundary=" + boundary);
            var fileData = webClient.Encoding.GetString(bytes);
            var package = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"file\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n{3}\r\n--{0}--\r\n", boundary, strFileName, "multipart/form-data", fileData);

            var nfile = webClient.Encoding.GetBytes(package);

            byte[] resp = webClient.UploadData(documentUrl, "POST", nfile);

            string Content = System.Text.Encoding.ASCII.GetString(resp);

            System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
            object RawData = jss.DeserializeObject(Content);

            RawData = RawData;
            ////Dictionary <string, string> DictObjData = jss.ConvertToType(new Dictionary<String, String>)(RawData["virusscan"][0]);


            //WORKS!!!!!!!!!!
            //WebClient WC = new WebClient();
            //WC.Headers[HttpRequestHeader.Authorization] = Convert.ToString("Bearer ") + accessToken;
            //WC.Headers[HttpRequestHeader.Accept] = "application/json";

            //Byte[] resp = WC.UploadFile(documentUrl, strFilePath);
            //string Content = System.Text.Encoding.ASCII.GetString(resp);


            //System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
            //object RawData = jss.DeserializeObject(Content);

            //RawData = RawData;
            ////Dictionary <string, string> DictObjData = jss.ConvertToType(new Dictionary<String, String>)(RawData["virusscan"][0]);








            //HttpClient httpClient = null;
            //HttpResponseMessage response = null;


            //try
            //{

            //    httpClient = new HttpClient();



            //    string formDataBoundary = "---------------------------2156852217421";






            //    var httprequestMessage = new HttpRequestMessage(HttpMethod.Post, documentUrl);
            //    httpClient.DefaultRequestHeaders.Add("Authorization", "bearer " + accessToken);
            //    //httpClient.DefaultRequestHeaders.TryAddWithoutValidation("user-agent", ConfigManager.UserAgent);
            //    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/json");
            //    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
            //    httpClient.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
            //    httpClient.DefaultRequestHeaders.Add("MIME-Version", "1.0");


            //    //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //    //    client.DefaultRequestHeaders.Add("Authorization", "bearer " + accessToken);
            //    //    client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip,deflate");
            //    //    client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
            //    //    client.DefaultRequestHeaders.Add("MIME-Version", "1.0");



            //    MultipartFormDataContent form = new MultipartFormDataContent();

            //    bytes = System.IO.File.ReadAllBytes(strFilePath);
            //    form.Add(new StreamContent(BuildMultipartStream("file_01", strFileName, bytes, formDataBoundary)));


            //    form.Add(new StringContent("documents={\"external_id\":\"testid\",\"space_id\":\"8919\",\"store_input_files\":\"true\"}", Encoding.UTF8, "application/json"));//CONTENT-TYPE header


            //    httprequestMessage.Content = form;
            //    httprequestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");
            //    httprequestMessage.Content.Headers.ContentType.Parameters.Add(new NameValueHeaderValue(
            //        "boundary",
            //        formDataBoundary));



            //    //curl - k - verbose - F "documents={'external_id':<external id>, 'space_id':<space id value>, store_input_files:false}" - F "file_01=@msword_lower.doc" - F "file_02=@goat_jpg.jpg" "https://federateddata.optum.com/v2/documents"--header "Content-Type:multipart/form-data"--header "Authorization: Bearer <AUTH_TOKEN>"




            //    //response = await httpClient.SendAsync(httprequestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
            //    response = await httpClient.SendAsync(httprequestMessage, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

            //    response.EnsureSuccessStatusCode();


            //}
            //catch (Exception ex)
            //{

            //}




            using (var client = new HttpClient())
            {

                bytes = System.IO.File.ReadAllBytes(strFilePath);


                client.BaseAddress = new Uri(documentUrl);



                //accessToken = "BOGUS";


                ///client.DefaultRequestHeaders.Add("Accept-Encoding", "multipart/form-data");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("multipart/form-data"));
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + accessToken);
                //client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip,deflate");
                client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
                client.DefaultRequestHeaders.Add("MIME-Version", "1.0");

                //FormUrlEncodedContent content = new FormUrlEncodedContent(postDict);





                //var byteArrayContent = new ByteArrayContent(bytes);
                var byteArrayContent = new ByteArrayContent(bytes, 0, bytes.Length);
                byteArrayContent.Headers.ContentType = MediaTypeHeaderValue.Parse("text/plain");
                //byteArrayContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");


                //byteArrayContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                //{
                //    FileName = strFileName
                //};




                multipartFormDataContent = new MultipartFormDataContent();
                //multipartFormDataContent.Add(content);
                //multipartFormDataContent.Add(new StringContent("external_id"), "testid");
                //multipartFormDataContent.Add(new StringContent("space_id"), "8919");
                //multipartFormDataContent.Add(new StringContent("store_input_files"), "true");

                //multipartFormDataContent.Add(new StringContent("documents={\"external_id\":\"testid\",\"space_id\":\"8919\",\"store_input_files\":\"true\"}", Encoding.UTF8, "application/json"));//CONTENT-TYPE header

                //multipartFormDataContent.Add(byteArrayContent, "\"file_01\"", "\"" + strFileName + "\"");
                multipartFormDataContent.Add(byteArrayContent, "file_01", strFileName);

                // string boundary = "----CustomBoundary" + DateTime.Now.Ticks.ToString("x");
                // multipartFormDataContent.Headers.Remove("Content-Type");
                //multipartFormDataContent.Headers.TryAddWithoutValidation("Content-Type", "multipart/form-data; boundary=" + boundary);
                //multipartFormDataContent.Headers.TryAddWithoutValidation("Content-Type", "multipart/form-data");



                //    //curl - k - verbose - F "documents={'external_id':<external id>, 'space_id':<space id value>, store_input_files:false}" - F "file_01=@msword_lower.doc" - F "file_02=@goat_jpg.jpg" "https://federateddata.optum.com/v2/documents"--header "Content-Type:multipart/form-data"--header "Authorization: Bearer <AUTH_TOKEN>"

                //multipartFormDataContent.Headers.Remove("Content-Type");
                //string boundary = "----CustomBoundary" + DateTime.Now.Ticks.ToString("x");
                //HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "relativeAddress");
                //request.Content = multipartFormDataContent;
                //request.Content.Headers.Add("ContentType", "multipart/form-data; boundary=" + boundary);
                //var response = await client.SendAsync(request);




                //multipartFormDataContent.Add(byteArrayContent);

                var response = await client.PostAsync(documentUrl, multipartFormDataContent);


            }


            //accessToken = await GetAccessToken();

            //string CT = "text/plain";

            //API_Calls.FileParameter f = new API_Calls.FileParameter(File.ReadAllBytes(strFilePath), strFileName, "multipart/form-data");
            //Dictionary<string, object> d = new Dictionary<string, object>();
            //d.Add(CT, f);
            //string ua = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2";
            //API_Calls.MultipartFormDataPost(documentUrl, ua, d, accessToken);








            //curl - k - verbose - F "documents={'space_id':<space id value>,  'metadata' : { 'providerId': 'sample_provider', 'fileName': 'birds3.jpg', 'fileDescription': 'very simple file', 'createdDate': '2016-12-21T12:42:36.247+05:30', 'createApplication': 'sample application', 'expiryDate': '2050-12-21T12:42:36.247+05:30', 'filePath': 'reports/FEB2017', 'tenant': 'uhcbo', 'claimNumber': '12345678', 'fileType': 'Letter', 'category': 'Benefits', 'subCategory': 'Transactions', 'privilege': 'Electronic Payments and Statements', 'organization': 'Optum Technologies', 'corporateMpin': '123456789', 'tin': '234567891', 'mpin': '12345', 'memberId': '345678912', 'physicianName': 'Smith,Ann', 'policyNumber': '4567891', 'employeeName': 'Smith, John', 'dateOfService': '2016-12-21T12:42:36.247+05:30', 'memberName': 'Brown, Sue', 'notificationReq': 'N', 'emailAlertReq': 'Y' }}" "$HOST_URL/v2/documents" - F "file_01=@close-active.png" - F "file_02=@test22.doc"--header "Authorization: Bearer <AUTH_TOKEN>"


            //fileName
            //fileDescription
            //createdDate
            //createApplication
            //expiryDate
            //filePath
            //tenant
            //claimNumber
            //fileType
            //category
            //subCategory
            //privilege
            //organization
            //corporateMpin
            //tin
            //mpin
            //memberId
            //notificationReq
            //emailAlertReq
            //physicianName
            //policyNumber
            //employeeName
            //dateOfService
            //memberName
            //providerEmailId
            //frequency
            //weekDay

            // https://fs2-stagedmz-ose3.optum.com/v2/documents";
            //accessToken = "998bd793-aa32-4c85-b99e-aecc25b2a100ddddddddd";
            //accessToken = await GetAccessToken();
            ////accessToken = "dddddddddddddd";




            //using (var httpClient = new HttpClient())
            //{
            //    //var surveyBytes = ConvertToByteArray(surveyResponse);
            //    bytes = System.IO.File.ReadAllBytes(strFilePath);

            //    //httpClient.DefaultRequestHeaders.Add("X-API-TOKEN", _apiToken);
            //    httpClient.DefaultRequestHeaders.Add("Authorization", "bearer " + accessToken);


            //    httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip,deflate");
            //    httpClient.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
            //    httpClient.DefaultRequestHeaders.Add("MIME-Version", "1.0");
            //    //httpClient.DefaultRequestHeaders.AcceptEncoding.Add( new StringWithQualityHeaderValue("gzip,deflate"));


            //    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //    //httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("multipart/form-data"));


            //    //httpClient.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;


            //    var byteArrayContent = new ByteArrayContent(bytes);
            //    byteArrayContent.Headers.ContentType = MediaTypeHeaderValue.Parse("text/plain");


            //    multipartFormDataContent = new MultipartFormDataContent();
            //    //multipartFormDataContent.Add(new StringContent("external_id"), "testid");
            //    //multipartFormDataContent.Add(new StringContent("space_id"), "8919");
            //    //multipartFormDataContent.Add(new StringContent("store_input_files"), "true");

            //   multipartFormDataContent.Add(new StringContent("{\"external_id\":\"testid\",\"space_id\":\"8919\",\"store_input_files\":\"true\"}", Encoding.UTF8, "application/json"));//CONTENT-TYPE header
            //    //multipartFormDataContent.Add(byteArrayContent, "\"File\"", "\"" + strFileName + "\"");


            //   // HttpUtility.UrlEncode(JsonConvert.SerializeObject(




            //    var response = await httpClient.PostAsync(documentUrl, multipartFormDataContent);



            //    //return response;
            //}




            //return;



            //response = await httpClient.PostAsync(documentUrl, multipartFormDataContent);

            //return ((dynamic)response);



            //            from Ganesh Chandra (privately):
            //POST https://fs2-stagedmz-ose3.optum.com/v2/documents/ HTTP/1.1
            //Accept - Encoding: gzip,deflate
            //Content - Type: multipart / form - data; boundary = "----=_Part_2_9909227.1519157151127"
            //MIME - Version: 1.0
            //Authorization: bearer 17cdf2c7 - 092f - 47e0 - 9cda - 53a9876966d3
            //        accept: application / json
            //Content - Length: 522
            //Host: fs2 - stagedmz - ose3.optum.com
            //Connection: Keep - Alive
            //User - Agent: Apache - HttpClient / 4.1.1(java 1.5)


        }



        static string fileReadTest = @"C:\Users\cgiorda\Desktop\PDF MetaData Test\testAPI";
        static string fileReadFolder = @"C:\Users\cgiorda\Desktop\PDF MetaData Test\Input";
        static string fileWriteFolder = @"C:\Users\cgiorda\Desktop\PDF MetaData Test\Output";

        static void readWritePDFMetaData()
        {

            string[] files = Directory.GetFiles(fileReadFolder, "*.pdf", SearchOption.AllDirectories);
            string strNewfileName = null;
            string strMPINToSearch = null;

            foreach(string strFile in files)
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(strFile);
                strNewfileName = fileWriteFolder + "\\" + fi.Name;
                strMPINToSearch = fi.Name.Split('_')[0];


                //DT = DBCONN.GETData(strMPINToSearch)


                using (var reader = new PdfReader(strFile))
                {
                    using (var stamper = new PdfStamper(reader, new FileStream(strNewfileName, FileMode.Create)))
                    {
                        var info = reader.Info;

                        //DT LOOP
                        info["Author"] = "Chris Giordano";
                        info["Title"] = "Tester";
                        info["MPIN"] = "1234";
                        info["TIN"] = "567890";



                        stamper.MoreInfo = info;

                        using (var ms = new MemoryStream())
                        {
                            var xmp = new XmpWriter(ms, info);
                            stamper.XmpMetadata = ms.ToArray();
                            xmp.Close();
                        }
                    }
                }

            }

        }


    }
}
