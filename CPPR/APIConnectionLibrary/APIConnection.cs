using Nancy.Json;
using SharedFunctionsLibrary;
using System.Net;


namespace APIConnectionLibrary
{
    public class APIConnection
    {
        public static string getToken(string strUrl, string strUsername, string strPassword, bool isExternal = false)
        {

            string strEncodedPassword = SharedFunctions.Base64Encode(strPassword);

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(strUrl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new System.IO.StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = "{\"external\":" + isExternal.ToString().ToLower() + ",\"password\":\"" + strEncodedPassword + "\", \"username\":\"" + strUsername + "\"}";

                streamWriter.Write(json);
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            string strToken;
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                strToken = streamReader.ReadToEnd();
            }
            return strToken;

        }


        public static string refreshToken(string strUrl, string strToken)
        {

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(strUrl);

            httpWebRequest.Accept = "application/json";
            httpWebRequest.Method = "GET";
            httpWebRequest.Headers["Authorization"] = "Bearer " + strToken;
            httpWebRequest.Headers["Refresh-Token"] = "true";

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            string result;
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }

            return result;

        }


        public static string getJWTToken(string strJSON)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            JWTToken jwt = js.Deserialize<JWTToken>(strJSON);
            return jwt.jwttoken;
        }


        public static void uploadFile(string strUrl, string strToken, string strFilePath, string strDisplayName)
        {

            //curl -X POST "https://ecgqcpift.healthtechnologygroup.com:9443/qcgatewayservice/api/uploadfile?recipient-display-name=NicholasJennings&recipient-type=PARTNER" -H "accept: application/json" -H "Authorization: Bearer eyJhbGciOiJIUzI1NiJ9.eyJzdWIiOiJjZ2lvcmRhIiwiZXhwIjoxNjM2MzkxODg2LCJpYXQiOjE2MzYzODQ2ODZ9.lQmrazUesu3An0ohE0dlwic6qCWUHuw7UT-vM-1nEWg" -H "Content-Type: multipart/form-data" -F "file=@NickECGTest.txt;type=text/plain"
            //https://ecgqcpift.healthtechnologygroup.com:9443/qcgatewayservice/api/uploadfile?recipient-display-name=NicholasJennings&recipient-type=PARTNER

            //NameValueCollection nvc = new NameValueCollection();
            //nvc.Add("Authorization", "Bearer " + strToken);
            HttpUploadFile(strUrl + "?recipient-display-name=" + strDisplayName + "&recipient-type=PARTNER", strFilePath, "file", "text/plain", strToken);

        }


        private static void HttpUploadFile(string url, string file, string paramName, string contentType, string strToken)
        {

            Console.WriteLine(string.Format("Uploading {0} to {1}", file, url));
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.Method = "POST";
            wr.KeepAlive = true;
            wr.Credentials = System.Net.CredentialCache.DefaultCredentials;
            wr.Headers["Authorization"] = "Bearer " + strToken;

            Stream rs = wr.GetRequestStream();

            //string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
            //foreach (string key in nvc.Keys)
            //{
            //    rs.Write(boundarybytes, 0, boundarybytes.Length);
            //    string formitem = string.Format(formdataTemplate, key, nvc[key]);
            //    byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
            //    rs.Write(formitembytes, 0, formitembytes.Length);
            //}
            rs.Write(boundarybytes, 0, boundarybytes.Length);

            string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
            string header = string.Format(headerTemplate, paramName, file, contentType);
            byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
            rs.Write(headerbytes, 0, headerbytes.Length);

            FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[4096];
            int bytesRead = 0;
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                rs.Write(buffer, 0, bytesRead);
            }
            fileStream.Close();

            byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            rs.Write(trailer, 0, trailer.Length);
            rs.Close();

            WebResponse wresp = null;
            try
            {
                wresp = wr.GetResponse();
                Stream stream2 = wresp.GetResponseStream();
                StreamReader reader2 = new StreamReader(stream2);
                Console.WriteLine(string.Format("File uploaded, server response is: {0}", reader2.ReadToEnd()));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error uploading file", ex);
                if (wresp != null)
                {
                    wresp.Close();
                    wresp = null;
                }
            }
            finally
            {
                wr = null;
            }
        }



    }

    public class JWTToken
    {
        public string jwttoken { get; set; }
        public string message { get; set; }
    }
}
