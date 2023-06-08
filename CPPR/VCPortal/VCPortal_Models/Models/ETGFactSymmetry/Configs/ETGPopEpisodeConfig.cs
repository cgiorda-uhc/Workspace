using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCPortal_Models.Models.ETGFactSymmetry;
public class ETGPopEpisodeConfig
{
    public string Base_ETG { get; set; }
    public string ETG_Base_Class_Description { get; set; }
    public string Premium_Specialty { get; set; }

    public string TRT_CD { get; set; }

    public string Always_Attributed { get; set; }

    public string If_Attributed { get; set; }

    public string Rx { get; set; }
    public string NRx { get; set; }

    public string Risk_Model { get; set; }

    public string Current_EC_Treatment_Indicator { get; set; }

}
