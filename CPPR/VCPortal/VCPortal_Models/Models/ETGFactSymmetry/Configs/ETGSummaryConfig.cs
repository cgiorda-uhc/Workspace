using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCPortal_Models.Models.ETGFactSymmetry;
public class ETGSummaryConfig
{

    public string ETG_Base_Class { get; set; }
    public string ETG_Description { get; set; }
    public string Premium_Specialty { get; set; }

    public char Never_Map { get; set; }

    public char Never_Map_Previous { get; set; }

    public string Previous_Rx_NRx { get; set; }
    public string Current_Rx_NRx { get; set; }
    public string Previous_LOB { get; set; }
    public string Current_LOB { get; set; }
    public string PC_Previous_Treatment_Indicator { get; set; }
    public string PC_Current_Treatment_Indicator { get; set; }
    //public double? PC_Episode_Cnt { get; set; }
    //public double? PC_Tot_Cost { get; set; }
    //public double? PC_Avg_Cost { get; set; }
    //public double? PC_CV { get; set; }
    public double? PC_Spec_Episode_Cnt { get; set; }
    public double? PC_Spec_Episode_Distribution { get; set; }
    public double? PC_Spec_Perc_of_Episodes { get; set; }
    public double? PC_Spec_Tot_Cost { get; set; }
    public double? PC_Spec_Avg_Cost { get; set; }

    public double? PC_Spec_Normalized_Pricing { get; set; }


    public double? PC_Spec_CV { get; set; }
    public string PC_Prev_Attribution { get; set; }

    public string PC_Current_Attribution { get; set; }
    public string PC_Change_Comments { get; set; }
    public string EC_Previous_Treatment_Indicator { get; set; }
    public string EC_Current_Treatment_Indicator { get; set; }
    //public double? EC_Episode_Count { get; set; }
    //public double? EC_Tot_Cost { get; set; }
    //public double? EC_Avg_Cost { get; set; }
    //public double? EC_CV { get; set; }
    public double? EC_Spec_Episode_Cnt { get; set; }
    public double? EC_Spec_Episode_Distribution { get; set; }
    public double? EC_Spec_Perc_of_Episodes { get; set; }
    public double? EC_Spec_Tot_Cost { get; set; }
    public double? EC_Spec_Avg_Cost { get; set; }
    public double? EC_Spec_Normalized_Pricing { get; set; }
    public double? EC_Spec_CV { get; set; }
    public string EC_Previous_Mapping { get; set; }
    public string EC_Current_Mapping { get; set; }
    public string EC_Change_Comments { get; set; }
    //public string Previous_Pt_Centric_Mapping { get; set; }
    //public string Current_Pt_Centric_Mapping { get; set; }
    //public string Pt_Centric_Change_Comments { get; set; }
    public string Measure_Status { get; set; }
}
