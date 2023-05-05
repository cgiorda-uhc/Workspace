using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCPortal_Models.Models.ETGFactSymmetry;
public class ETGPatientCentricConfig
{
    public string Base_ETG { get; set; }
    public string ETG_Base_Class_Description { get; set; }
    public string Premium_Specialty { get; set; }
    public string TRT_CD { get; set; }
    public string Current_Rx_NRx { get; set; }
    public string Risk_Model { get; set; }

    public string Current_Pt_Centric_Mapping { get; set; }
    public string Pt_Centric_Change_Comments { get; set; }

}
