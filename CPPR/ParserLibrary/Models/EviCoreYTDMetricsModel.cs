using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagerLibrary.Models;

public class EviCoreYTDMetricsModel
{
    public string Summary_of_Lob { get; set; }
    public string Call_Taker { get; set; }
    public int Total_Calls { get; set; }
    public float? Avg_Speed_Answer { get; set; }
    public float? Abandoned_Calls { get; set; }
    public float? Abandoned_Percent { get; set; }
    public float? Average_Talk_Time { get; set; }
    public float? ASA_in_SL_Perent { get; set; }
    public string report_type { get; set; }
    public int file_month { get; set; }
    public int file_year { get; set; }
    public DateTime file_date { get; set; }
    public string sheet_name { get; set; }
    public string file_name { get; set; }
    public string file_path { get; set; }
}
