using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtensionMethods
{
    public static class StringExt
    {
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        public static bool IsNumeric(this string s)
        {
            float output;
            return float.TryParse(s, out output);
        }

        public static bool IsDate(this string input)
        {
            DateTime result;
            bool valid = DateTime.TryParse(input, out result);
            return valid;
        }
    }


    public static class DataTableExt
    {
        public static DataTable Delete(this DataTable table, string filter)
        {
            table.Select(filter).Delete();
            table.AcceptChanges();
            return table;
        }
        public static void Delete(this IEnumerable<DataRow> rows)
        {
            foreach (var row in rows)
                row.Delete();
        }
    }



}
