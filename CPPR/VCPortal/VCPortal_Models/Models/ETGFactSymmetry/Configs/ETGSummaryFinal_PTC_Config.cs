using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCPortal_Models.Models.ETGFactSymmetry.Configs;
public class ETGSummaryFinal_PTC_Config
{
    public string ETG_Base_Class { get; set; }
    public string ETG_Description { get; set; }
    public string Premium_Specialty { get; set; }

    public string Never_Map { get; set; }

    public string Never_Map_Previous { get; set; }

    public string Previous_Rx_NRx { get; set; }
    public string Current_Rx_NRx { get; set; }
    public string Previous_LOB { get; set; }
    public string Current_LOB { get; set; }

    public string LOB_UGAP { get; set; }

    public string PC_Previous_Treatment_Indicator { get; set; }
    public string PC_Current_Treatment_Indicator { get; set; }

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

    public string Measure_Status { get; set; }

    public string UGAP_Changes { get; set; }


    public string Is_Flagged { get; set; }



}
