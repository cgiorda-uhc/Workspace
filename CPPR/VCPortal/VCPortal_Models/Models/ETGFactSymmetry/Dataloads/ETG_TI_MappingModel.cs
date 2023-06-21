using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCPortal_Models.Models.ETGFactSymmetry.Dataloads;
public class ETG_TI_MappingModel
{
    public string MPC { get; set; }
    public int Base_ETG { get; set; }

    public Int16 Treatment_Indicator { get; set; }
    public string Treatment_Indicator_Description { get; set; }
}
