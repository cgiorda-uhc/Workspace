using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCPortal_Models.Models.ETGFactSymmetry.Configs;
public class ETGPECSummaryConfig
{

    public string ETG_Base_Class { get; set; }
    public string ETG_Description { get; set; }

    public string Premium_Specialty { get; set; }


    public string LOBPrevious { get; set; }
    public string LOB { get; set; }



    public bool? Never_Mapped_Previous { get; set; }

    public bool? Never_Mapped { get; set; }





    public string RX_NRXPrevious { get; set; }
    public string RX_NRX { get; set; }



    public string EC_Treatment_Indicator_Previous { get; set; }
    public string EC_Treatment_Indicator { get; set; }

    public float EC_Spec_Episode_Distribution { get; set; }
    public float EC_Spec_Percent_of_Episodes { get; set; }
    public float EC_Spec_Total_Cost { get; set; }
    public float EC_Spec_Average_Cost { get; set; }
    public float EC_Coefficients_of_Variation { get; set; }
    public float EC_Episode_Count { get; set; }

    public float EC_Normalized_Pricing_Total_Cost { get; set; }

    public float EC_Spec_Episode_Count_Previous { get; set; }
    public float EC_Spec_Episode_Count { get; set; }

    public float? EC_Spec_Episode_Count_Diff { get; set; }

    public float EC_Total_Cost { get; set; }
    public float EC_Average_Cost { get; set; }
    public float EC_Spec_CV { get; set; }
    public string EC_Changes_Made { get; set; }

    public string EC_Mapping_Previous { get; set; }
    public string EC_Mapping { get; set; }

    public string EC_Change_Comments { get; set; }

    public string Data_Period_Previous { get; set; }
    public string Data_Period { get; set; }

    public float Symmetry_Version_Previous { get; set; }
    public float Symmetry_Version { get; set; }


}
