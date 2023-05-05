using FileParsingLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCPortal_Models.Configuration.HeaderInterfaces.Concrete;
public class ExcelExportConfig
{
    public string FilePath { get; set; }

    public string FileName { get; set; }

    public List<ExcelSheetConfig> Sheets { get; set; }
}
