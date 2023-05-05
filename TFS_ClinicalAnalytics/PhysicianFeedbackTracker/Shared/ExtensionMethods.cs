using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhysicianFeedbackTracker
{
    static class ExtensionMethods
    {

        public static string ToFullTextSearch(this string str, Int16 intType = 1 )
        {

            if (str.IsNumeric())
                return str;

            StringBuilder sbFinal = new StringBuilder();
            string[] strArr = str.Trim().Replace("\"","").Split(' ');
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


            if (strFinal.Trim() == "")
                strFinal = null;

            return strFinal;

        }



        public static bool IsNumeric(this string theValue)
        {
            long retNum;
            return long.TryParse(theValue, System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
        }

        public static bool IsDate(this string input)
        {
            DateTime result;
            bool valid = DateTime.TryParse(input, out result);
            return valid;
        }

        public static string removeNoise(this string theValue)
        {

            return theValue.Trim().Replace("\n", "").Replace("\r", "");
        }




        static int i = 0;
        public static int[] MultipleIndex(this string StringValue, char chChar)
        {

            var indexs = from rgChar in StringValue
                            where rgChar == chChar && i != StringValue.IndexOf(rgChar, i + 1)
                            select new { Index = StringValue.IndexOf(rgChar, i + 1), Increament = (i = i + StringValue.IndexOf(rgChar)) };
            i = 0;
            return indexs.Select(p => p.Index).ToArray<int>();
        }


        public static int[] FindAllIndexof<T>(this IEnumerable<T> values, T val)
        {
            return values.Select((b, i) => object.Equals(b, val) ? i : -1).Where(i => i != -1).ToArray();
        }




        public static void dataRowsToColumnsInTable(this DataTable dt, DataTable dtSource, string headerColumnName, string typeColumnName)
        {
            string strType;
            string strName;
            Type type = null;
            foreach (DataRow dr in dtSource.Rows)
            {
                strName = (dr[headerColumnName] != DBNull.Value ? dr[headerColumnName].ToString() : null);
                strType = (dr[typeColumnName] != DBNull.Value ? dr[typeColumnName].ToString() : null);
                type = null;

                if (strType == null)
                    continue;

                switch (strType)
                {
                    case "Text":
                        type = typeof(String);
                        break;
                    case "Email":
                        type = typeof(String);
                        break;
                    case "Phone":
                        type = typeof(String);
                        break;
                    case "Date":
                        type = typeof(DateTime);
                        break;
                    case "Int":
                        type = typeof(Int64);
                        break;
                    case "Dec":
                        type = typeof(Decimal);
                        break;
                    case "Bool":
                        type = typeof(Boolean);
                        break;
                    default:
                        type = null;
                        break;
                }

                if (type == null || strName == null)
                    continue;

                dt.Columns.Add(strName, type);
                
            }

        }


        public static void clearOutTable(this DataTable dt)
        {

            dt.Clear();

            foreach (var column in dt.Columns.Cast<DataColumn>().ToArray())
            {
                if (dt.AsEnumerable().All(dr => dr.IsNull(column)))
                    dt.Columns.Remove(column);
            }
        }




        public static bool AllColumnsEmpty(this DataRow dr)
        {
            if (dr == null)
            {
                return true;
            }
            else
            {
                foreach (var value in dr.ItemArray)
                {
                    if (value != null)
                    {
                        return false;
                    }
                }
                return true;
            }
        }


        public static DataTable CopyWithoutEmptyRows(this DataTable dt)
        {
            bool blIsEmpty = true;

            int i = 0;

            DataTable dtNew = dt.Clone();

            foreach(DataRow dr in dt.Rows)
            {
                blIsEmpty = true;

                if (dr != null)
                {
                    foreach (var value in dr.ItemArray)
                    {
                        if (value != DBNull.Value)
                        {
                            blIsEmpty = false;
                            break;
                        }
                    }
                }

                if(i == 197)
                {
                    string s = "";
                }

                if (!blIsEmpty)
                {
                    dtNew.ImportRow(dr);
                }
                    //dt.Rows.Remove(dr);

                i++;
            }

            return dtNew.Copy();

        }

        //RTF FOR HIGHLIGHTED STATUS 3182021
        //RTF FOR HIGHLIGHTED STATUS 3182021
        //RTF FOR HIGHLIGHTED STATUS 3182021
        public static void AppendSelection(this RichTextBox control, string text)
        {
            int len = control.TextLength;

            // Append the text.
            control.AppendText(text);

            // Prepare it for formatting.
            control.SelectionStart = len;
            control.SelectionLength = text.Length;

            // Scroll to it.
            control.ScrollToCaret();
        }

        /// <summary>
        /// Appends the selection.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="text">The text.</param>
        /// <param name="colour">The colour.</param>
        /// <param name="font">The font.</param>
        public static void AppendSelection(this RichTextBox control, string text, Color colour, Font font)
        {
            AppendSelection(control, text);
            control.SelectionColor = colour;
            control.SelectionFont = font;
        }

        /// <summary>
        /// Appends the selection.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="text">The text.</param>
        /// <param name="colour">The colour.</param>
        /// <param name="font">The font.</param>
        public static void AppendLog(this RichTextBox control, string text, Color colour, Font font)
        {
            Action append = () => AppendSelection(control, text, colour, font);
            if (control.InvokeRequired)
                control.Invoke(append);
            else
                append();
        }
  

    }
}
