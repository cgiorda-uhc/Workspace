using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCS_PWFile
{
    class UCS_PWFile
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter PW:  "); // or Console.Write("Type any number:  "); to enter number in the same line
            string strPW= Base64Encode(Console.ReadLine());
            string strPWPath = ConfigurationManager.AppSettings["PWPath"];

            File.WriteAllText(strPWPath, strPW);

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
    }
}
