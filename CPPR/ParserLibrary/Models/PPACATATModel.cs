using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagerLibrary.Models;

public class PPACATATModel : IFileDetailsModel
{
    public string Summary_of_Lob { get; set; }
    public string Carrier_State { get; set; }
    public string Line_of_Business { get; set; }
    public string Modality { get; set; }
    public int Total_Authorizations_Notifications { get; set; }
    public int LessEqual_2_BUS_Days { get; set; }
    public float PerLessEqual_2_BUS_Days { get; set; }
    public float Less_State_TAT_Requirements { get; set; }
    public float PerLess_State_TAT_Requirements { get; set; }
    public float Average_Business_Days { get; set; }
    public float Average_BUS_Days_Receipt_Clinical { get; set; }
    public float Avg_CAL_Days_Case_Creation { get; set; }
    public float Average_BUS_Days_Case_Creation { get; set; }
    public float Avg_Business_Days_Denial_Letter_Sent { get; set; }

    public string report_type { get; set; }
    public int file_month { get; set; }
    public int file_year { get; set; }
    public DateTime file_date { get; set; }
    public string sheet_name { get; set; }
    public string file_name { get; set; }
    public string file_path { get; set; }
    public DateTime delivery_date { get; set; }

}
