using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCPortal_Models.Models.ETGFactSymmetry.Configs;
public class ETG_PEC_Summary_Final
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


    
    public string EC_Treatment_Indicator { get; set; }
    
    public string EC_Mapping { get; set; }

    public string EC_Change_Comments { get; set; }
    public string Data_Period { get; set; }
    public string Data_Period_Previous { get; set; }

    public float Symmetry_Version_Previous { get; set; }

    public float Symmetry_Version { get; set; }

}
