using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileParsingLibrary.Models
{
    public class ExcelExport
    {
        public List<object> ExportList { get; set; }

        //public IEnumerable<T> ExportList { get; set; }

        public string SheetName { get; set; }

    }
}
