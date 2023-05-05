using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysicianFeedbackTracker
{
    static class ExtensionMethods
    {

        public static string ToFullTextSearch(this string str, Int16 intType = 1 )
        {

            if (str.IsNumeric())
                return str;

            StringBuilder sbFinal = new StringBuilder();
            string[] strArr = str.Trim().Split(' ');
            string strFinal;

            foreach(string s in  strArr)
            {
                if (s.Trim().Equals(""))
                    continue;
                else
                {
                    if(intType == 1)
                    {
                        sbFinal.Append("\"" + s + "*\" AND ");
                    }
                    else if (intType == 2)
                    {
                        sbFinal.Append("\"" + s + "*\" OR ");
                    }
                    else
                    {
                        sbFinal.Append("\"" + s + "*\"  ");
                    }
                }

            }

            if (intType == 1)
            {
                strFinal = sbFinal.ToString().TrimEnd(' ', 'A', 'N', 'D').Trim();
            }
            else if (intType == 2)
            {
                strFinal = sbFinal.ToString().TrimEnd(' ', 'O', 'R').Trim();
            }
            else
            {
                strFinal = sbFinal.ToString().Trim();
            }


            return strFinal;

        }



        public static bool IsNumeric(this string theValue)
        {
            long retNum;
            return long.TryParse(theValue, System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
        }

    }
}
