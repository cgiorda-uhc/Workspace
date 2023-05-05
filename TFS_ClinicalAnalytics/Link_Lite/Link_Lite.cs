using Newtonsoft.Json; //NUGET: newtonsoft.json
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Runtime.Serialization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Link_Lite
{
    class Link_Lite
    {
        //ADD TO App.config file
        //<add key = "clientId_dev" value="????????"/>
        //<add key = "clientSecret_dev" value="????????"/>
        //<add key = "tokenURL_dev" value="https://fs2-stagedmz-ose3.optum.com:443/v2/oauth2/token.json"/>
        //<add key = "documentURL_dev" value="https://fs2-stagedmz-ose3.optum.com:443/v2/documents/"/>
        //<add key = "attachmentURL_dev" value="https://fs2-stagedmz-ose3.optum.com/v2/attachments/{$attachment_id}"/>
        //<add key = "external_id_dev" value="????????"/>
        //<add key = "space_id_dev" value="????????"/>


        static string clientId = ConfigurationManager.AppSettings["clientId_dev"];
        static string clientSecret = ConfigurationManager.AppSettings["clientSecret_dev"];
        static string tokenUrl = ConfigurationManager.AppSettings["tokenURL_dev"]; //https://fs2-stagedmz-ose3.optum.com:443/v2/documents/
        static string documentUrl = ConfigurationManager.AppSettings["documentURL_dev"]; //https://fs2-stagedmz-ose3.optum.com:443/v2/oauth2/token.json
        static string external_id = ConfigurationManager.AppSettings["external_id_dev"];
        static Int16 space_id = Int16.Parse(ConfigurationManager.AppSettings["space_id_dev"]);
        static string attachmentUrl = ConfigurationManager.AppSettings["attachmentURL_dev"]; //https://fs2-stagedmz-ose3.optum.com/v2/attachments/{$attachment_id}

        static void Main(string[] args)
        {
            TestRequest().Wait();
        }

        private static async Task TestRequest()
        {

            string strFileToUpload = @"C:\TestUpload\Test.pdf";

            //GET ALL METADATA FOR THIS FILE
            MetaData md = new MetaData
            {
                fileName = "Test.pdf",
                fileDescription = "Reports",
                createdDate = DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ss.fffzzz"), //FDS UPLOAD
                createApplication = "Reports",
                expiryDate = DateTime.Now.AddYears(1).ToString("yyyy-MM-dd'T'HH:mm:ss.fffzzz"),
                filePath = "Reports",
                tenant = "Link",
                fileType = "Reports",
                category = "Reports",
                subCategory = "Reports",
                privilege = "RPRT",
                corporateMpin = "MPIN", //PAD WITH LEADING ZEROS TOTAL 9 CHARACTERS
                tin = "TIN"
            };

            //WRAP METADATA INTO CONTAINER
            RequestContainer mdContainer = new RequestContainer
            {
                space_id = space_id,
                external_id = external_id,
                metadata = md

            };


            //MULTIPART
            string accessToken = await FDSGetAccessToken();
            ResponseContainer responseContainer = await FDSMultiPartRequest(documentUrl, accessToken, mdContainer, strFileToUpload);

            //TEST RESULTS
            string strNewFileNameAndPath = @"C:\TestDownload\Test.pdf";
            string strAttachmentId = responseContainer.attachments[0].id; //GENERATED FROM PREVIOUS REQUEST
            await FDSGetAttachmentFile(attachmentUrl, accessToken, strAttachmentId, strNewFileNameAndPath);

        }





        //FDS FRIENDLY FUNCTIONS START
        //FDS FRIENDLY FUNCTIONS START
        //FDS FRIENDLY FUNCTIONS START
        private static async Task<string> FDSGetAccessToken()
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
                //HANDLE
            }

            return await Task.FromResult(((dynamic)responseData).access_token);
        }


        private static Task<ResponseContainer> FDSMultiPartRequest(string strUrl, string accessToken, RequestContainer metadataObject, string strFileToUploadPath)
        {
            string strResponse = null;
            string strRequest = null;
            ResponseContainer responseContainer = null;
            string strFileName = Path.GetFileName(strFileToUploadPath);
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
                buffer = Encoding.UTF8.GetBytes(string.Format("\r\n\r\n--" + boundary + "\r\n" + "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n\r\n", "file_01", strFileName));
                memStream.Write(buffer, 0, buffer.Length);
                string fileExt = strFileName.Substring(strFileName.Length - 3, 3);
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
                    strRequest = System.Text.Encoding.Default.GetString(tempBuffer);

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
                    strResponse = streamReader.ReadToEnd();
                    //var objResponse = new JavaScriptSerializer().DeserializeObject(strResponse); //RESONSE CONTAINER IS NOT REQUIRED
                    responseContainer = new JavaScriptSerializer().Deserialize<ResponseContainer>(strResponse);
                }
            }
            catch (Exception ex)
            {
                //HANDLE
                Task.FromResult<ResponseContainer>(null);
            }

            return Task.FromResult<ResponseContainer>(responseContainer);
        }

        private static Task FDSGetAttachmentFile(string strUrl, string accessToken, string attachmentId, string strNewFileAndPath)
        {
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
            catch (Exception ex)
            {
                //HANDLE
            }

            return Task.FromResult<object>(null);
        }



        private static Task FDSDeleteFile(string strUrl, string accessToken, string documentId)
        {
            try
            {

                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(strUrl + documentId);
                //NEW REQUEST HEADER
                req.Headers.Add("Authorization", "Bearer " + accessToken);
                string boundary = "------------------------" + DateTime.Now.Ticks.ToString("x");
                req.ContentType = "multipart/form-data; boundary=" + boundary;
                req.Accept = "application/json";  // GET
                req.Method = "DELETE";

                var response = req.GetResponse();

                response.Close();
            }
            catch (Exception ex)
            {
                //HANDLE
            }


            return Task.FromResult<object>(null);

        }





    }







    //FDS FRIENDLY FUNCTIONS END
    //FDS FRIENDLY FUNCTIONS END
    //FDS FRIENDLY FUNCTIONS END


    //INNER CLASS FOR MULTIPART REQUEST
    public class MetaData
    {
        [DataMember(Name = "fileName")]
        public string fileName { get; set; }

        [DataMember(Name = "fileDescription")]
        public string fileDescription { get; set; }

        [DataMember(Name = "createdDate")]
        public string createdDate { get; set; }

        [DataMember(Name = "createApplication")]
        public string createApplication { get; set; }

        [DataMember(Name = "expiryDate")]
        public string expiryDate { get; set; }

        [DataMember(Name = "filePath")]
        public string filePath { get; set; }

        [DataMember(Name = "tenant")]
        public string tenant { get; set; }

        [DataMember(Name = "fileType")]
        public string fileType { get; set; }

        [DataMember(Name = "category")]
        public string category { get; set; }

        [DataMember(Name = "subCategory")]
        public string subCategory { get; set; }

        [DataMember(Name = "privilege")]
        public string privilege { get; set; }

        [DataMember(Name = "corporateMpin")]
        public string corporateMpin { get; set; }

        [DataMember(Name = "tin")]
        public string tin { get; set; }

    }

    //OUTER CLASS FOR MULTIPART REQUEST
    public class RequestContainer
    {
        [DataMember(Name = "space_id")]
        public int space_id { get; set; }

        [DataMember(Name = "external_id")]
        public string external_id { get; set; }

        [DataMember(Name = "metadata")]
        public MetaData metadata { get; set; }

    }



    //INNER CLASS FOR DOCUMENTS RESPONSE
    public class ResponseContainer
    {
        [JsonProperty("id")]
        public string id { get; set; }


        [JsonProperty("external_id")]
        public string external_id { get; set; }


        [JsonProperty("space_id")]
        public string space_id { get; set; }


        [JsonProperty("status")]
        public string status { get; set; }

        [JsonProperty("dateCreated")]
        public string dateCreated { get; set; }

        [JsonProperty("attachments")]
        public Attachments[] attachments { get; set; }

        [JsonProperty("metadata")]
        public MetaData metadata { get; set; }

    }

    //OUTER CLASS FOR DOCUMENTS RESPONSE
    public class Attachments
    {
        [JsonProperty("id")]
        public string id { get; set; }

        [JsonProperty("index")]
        public string index { get; set; }

        [JsonProperty("status")]
        public string status { get; set; }

        [JsonProperty("space_id")]
        public string space_id { get; set; }

        [JsonProperty("file_name")]
        public string file_name { get; set; }

        [JsonProperty("file_size")]
        public string file_size { get; set; }

        [JsonProperty("content_type")]
        public string content_type { get; set; }

        [JsonProperty("date_created")]
        public string date_created { get; set; }
    }







}
