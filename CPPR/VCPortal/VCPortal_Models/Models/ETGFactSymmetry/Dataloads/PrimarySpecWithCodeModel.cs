using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCPortal_Models.Models.ETGFactSymmetry.Dataloads;
public class PrimarySpecWithCodeModel
{
    public string PREM_SPCL_CD { get; set; }
    public Int64 MPIN { get; set; }
    public string ProvType { get; set; }
    public string NDB_SPCL_CD { get; set; }
    public string SpecTypeCd { get; set; }
    public string PrimaryInd { get; set; }
    public string ShortDesc { get; set; }

    public string Secondary_Spec { get; set; }

    public float PD_Version { get; set; }
}
