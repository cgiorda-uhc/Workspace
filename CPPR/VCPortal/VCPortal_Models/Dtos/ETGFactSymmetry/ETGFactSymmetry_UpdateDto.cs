using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCPortal_Models.Dtos.ETGFactSymmetry;
public class ETGFactSymmetry_UpdateDto
{

    public long ETG_Fact_Symmetry_id { get; set; }
    public string Current_Patient_Centric_Mapping { get; set; }
public string Previous_Patient_Centric_Mapping { get; set; }
    public string Current_Mapping { get; set; }
    public string Previous_Mapping { get; set; }
    public string Current_Mapping_Orginal { get; set; }
    public string Previous_Mapping_Orginal { get; set; }
    public string Current_Episode_Cost_Treatment_Indicator { get; set; }
    public string Previous_Episode_Cost_Treatment_Indicator { get; set; }
    public string Current_Attribution { get; set; }
    public string Previous_Attribution { get; set; }
    public string Pop_Cost_Current_Treatment_Indicator { get; set; }
    public string Pop_Cost_Previous_Treatment_Indicator { get; set; }
    public string LOBCurrentString { get; set; }
    public string LOBPreviousString { get; set; }
    public bool Has_Commercial { get; set; }
    public bool Has_Medicare { get; set; }
    public string Has_Medicaid { get; set; }
    public string Pop_Cost_Change_Comments { get; set; }
    public string Episode_Cost_Change_Comments { get; set; }
    public string Patient_Centric_Change_Comments { get; set; }
    public string User { get; set; }
}
