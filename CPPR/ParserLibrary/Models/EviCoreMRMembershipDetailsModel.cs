using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagerLibrary.Models;

public class EviCoreMRMembershipDetailsModel
{
    public string Program { get; set; }

    public DateTime IncurredDt { get; set; }

    public int MemberCount { get; set; }

    public string report_type { get; set; }
    public int file_month { get; set; }
    public int file_year { get; set; }
    public DateTime file_date { get; set; }
    public string sheet_name { get; set; }
    public string file_name { get; set; }
    public string file_path { get; set; }
}
