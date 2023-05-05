using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagerLibrary.Configuration.HeaderInterfaces.Concrete;

public class ExcelConfig
{
    public string SheetName { get; set; }

    public string ColumnRange { get; set; }

    public string SheetIdentifier { get; set; }

    public int StartingDataRow { get; set; }

    public string ColumnToValidate { get; set; }


    public List<string> SheetsToIgnore { get; set; }
}
