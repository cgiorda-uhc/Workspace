using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCPortal_Models.Models.ETGFactSymmetry;
public class ETGEpisodeCostConfig
{
    public string ETG_Base_Class { get; set; }

    public string ETG_Description { get; set; }
    public string Premium_Specialty { get; set; }

    public string Current_Rx_NRx { get; set; }
    public string Current_LOB { get; set; }
    public string EC_Current_Treatment_Indicator { get; set; }
    public double? EC_Episode_Count { get; set; }

    public double? EC_Tot_Cost { get; set; }

    public double? EC_Avg_Cost { get; set; }

    public double? EC_CV { get; set; }
    public double? EC_Spec_Episode_Cnt { get; set; }
    public double? EC_Spec_Episode_Distribution { get; set; }
    public double? EC_Spec_Perc_of_Episodes { get; set; }

    public double? EC_Spec_Tot_Cost { get; set; }

    public double? EC_Spec_Avg_Cost { get; set; }

    public double? EC_Spec_CV { get; set; }

    public string EC_Current_Mapping { get; set; }

    public string EC_Change_Comments { get; set; }
}
