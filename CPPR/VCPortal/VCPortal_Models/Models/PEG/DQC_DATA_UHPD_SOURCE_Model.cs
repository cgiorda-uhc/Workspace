using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCPortal_Models.Models.PEG;
public class DQC_DATA_UHPD_SOURCE_Model
{
    public Int64 Id { get; set; }
    public string PEG_ANCH_CATGY { get; set; }
    public string PEG_ANCH_SBCATGY { get; set; }
    public string PREM_SPCL_CD { get; set; }
    public char SVRTY_LVL_CD { get; set; }
    public int APR_DRG_RLLP_NBR { get; set; }
    public string QLTY_MSR_NM { get; set; }
    public int CNFG_POP_SYS_ID { get; set; }
    public string MKT_NBR { get; set; }
    public int UNET_MKT_NBR { get; set; }
    public string UNET_MKT_DESC { get; set; }
    public string Current_Version { get; set; }
    public Int64 Current_Market_Compliant { get; set; }
    public Int64 Current_Market_Opportunity { get; set; }
    public int Current_National_Compliant { get; set; }
    public int Current_National_Opportunity { get; set; }
    public string Previous_Version { get; set; }
    public Int64 Previous_Market_Compliant { get; set; }
    public Int64 Previous_Market_Opportunity { get; set; }
    public int Previous_National_Compliant { get; set; }
    public int Previous_National_Opportunity { get; set; }
    public string DTLocation { get; set; }
    public DateTime Data_Extract_Dt { get; set; }

}
