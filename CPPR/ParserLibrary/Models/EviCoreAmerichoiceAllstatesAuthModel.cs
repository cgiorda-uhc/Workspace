using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagerLibrary.Models;

public class EviCoreAmerichoiceAllstatesAuthModel : IFileDetailsModel
{
    public string State { get; set; }
    public string Modality { get; set; }

    public string Month { get; set; }

    public int Member_Lives { get; set; }

    public int Total_Requests { get; set; }

    public int Approved { get; set; }
    public int Denied { get; set; }
    public int Withdrawn { get; set; }
    public int Expired { get; set; }
    public int Non_Cert { get; set; }
    public int Pending { get; set; }
    public string report_type { get; set; }
    public int file_month { get; set; }
    public int file_year { get; set; }
    public DateTime file_date { get; set; }
    public string sheet_name { get; set; }
    public string file_name { get; set; }
    public string file_path { get; set; }

}
