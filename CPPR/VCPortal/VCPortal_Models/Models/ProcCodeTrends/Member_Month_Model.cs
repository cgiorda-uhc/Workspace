using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCPortal_Models.Models.ProcCodeTrends;
public class Member_Month_Model
{
    public string Metric { get; set; }

    public int? Y1Q1_Mbr_Month { get; set; }

    public int? Y1Q2_Mbr_Month { get; set; }


    public int? Y1Q3_Mbr_Month { get; set; }

    public int? Y1Q4_Mbr_Month { get; set; }

    public int? Y2Q1_Mbr_Month { get; set; }




    public int? Y2Q2_Mbr_Month { get; set; }



    public int? Y2Q3_Mbr_Month { get; set; }



    public int? Y2Q4_Mbr_Month { get; set; }

    public double? Y1Q1_Y2Q1_trend { get; set; }

    public double? Y1Q2_Y2Q2_trend { get; set; }

    public double? Y1Q3_Y2Q3_trend { get; set; }

    public double? Y1Q4_Y2Q4_trend { get; set; }

}
