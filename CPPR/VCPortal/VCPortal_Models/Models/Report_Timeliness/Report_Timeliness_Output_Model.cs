using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCPortal_Models.Models.Report_Timeliness;
public class Report_Timeliness_Output_Model
{
    public Int16? ertf_id { get; set; }
    public string file_location_wild { get; set; }

    public string file_name { get; set; }
    public string file_name_wild { get; set; }

    public string file_date { get; set; }

    public Int16 file_month { get; set; }

    public Int16 file_year { get; set; }

    public string drop_date { get; set; }
}
