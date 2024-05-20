using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Reflection;

namespace SharedFunctionsLibrary
{
    public class SharedFunctions
    {

        public static dynamic ConvertToType(dynamic source, Type dest)
        {
            return Convert.ChangeType(source, dest);
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }


        public static void runPythonCmd(string cmd = null, string args = null, string pythonPath = @"C:\Python36\python.exe", string pyExecutable = @"C:\Users\cgiorda\PycharmProjects\SteveCrawler\GetCMSFile_20220111.py")
        {


            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = pythonPath;
            //start.WorkingDirectory = @"C:\Users\cgiorda\PycharmProjects\SteveCrawler\venv\Lib\site-packages";
            //start.Arguments = string.Format("D:\\script\\test.py -a {0} -b {1} ", "some param", "some other param");
            start.Arguments = pyExecutable;
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    Console.Write(result);
                }
            }

        }


        public static object CastPropertyValue(PropertyInfo property, string value)
        {
            if (property == null || String.IsNullOrEmpty(value))
                return null;
            if (property.PropertyType.IsEnum)
            {
                Type enumType = property.PropertyType;
                if (Enum.IsDefined(enumType, value))
                    return Enum.Parse(enumType, value);
            }
            if (property.PropertyType == typeof(bool))
                return value == "1" || value == "true" || value == "on" || value == "checked";
            else if (property.PropertyType == typeof(Uri))
                return new Uri(Convert.ToString(value));
            else
                return Convert.ChangeType(value, property.PropertyType);
        }


        public static bool IsNumericType(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        public static Task EmailAsync(string strTo, string strFrom, string strSubect, string strBody, string? strCC = null, string? strAttachmentPaths = null, MailPriority mp = MailPriority.Normal)
        {
            /*function was terminated. The asynchronous programs required that you execute the threads and tasks to their end. Async sends the code on a separate thread, in most cases you can use the Task.Run function and run it asynchronously, like this, */
            //try                                         //
            //{
                Task t = Task.Run(async () =>
                {
                    MailMessage message = new MailMessage();
                    using (SmtpClient smtp = new SmtpClient())
                    {
                        message.From = new MailAddress(strFrom);

                        //LOOP ; separated??
                        string[] strRecipientArr = strTo.Split(';');
                        foreach (string s in strRecipientArr)
                            message.To.Add(new MailAddress(s));

                        if (strCC != null)
                        {
                            string[] strCCArr = strCC.Split(';');
                            foreach (string s in strCCArr)
                                message.CC.Add(new MailAddress(s));
                        }


                        if (strAttachmentPaths != null)
                        {
                            string[] strAttachmentPathArr = strAttachmentPaths.Split(';');
                            foreach (string s in strAttachmentPathArr)
                                message.Attachments.Add(new Attachment(s));
                        }

                        message.Priority = mp;

                        message.Subject = strSubect;
                        message.IsBodyHtml = true; //to make message body as html  
                        message.Body = strBody;
                        smtp.Port = 25;
                        smtp.Host = "mailo2.uhc.com"; //for gmail host  
                        smtp.EnableSsl = false;
                        smtp.UseDefaultCredentials = true;
                        //smtp.Credentials = new NetworkCredential("ms/peisaid", "BooWooDooFoo2023!!");
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        //await smtp.SendMailAsync(message).ConfigureAwait(false);

                        await Retry.DoWithRetryAsync(async () => await smtp.SendMailAsync(message).ConfigureAwait(false), TimeSpan.FromSeconds(30), tryCount: 1000);



                    }
                });
                t.Wait(); // Wait until the above task is complete, email is sent
                return Task.CompletedTask;
            //}                                           //
            //catch (Exception ex)                        //
            //{                                           //
            //    throw;   //
            //}
        }






    }
}