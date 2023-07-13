using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace VCPortal_Models.Models.EBM;
public class DQC_DATA_EBM_UHPD_SOURCE_Model
{
    public Int64 Id { get; set; }
    public string REPORT_CASE_ID { get; set; }

    public string REPORT_RULE_ID { get; set; }

    public string COND_NM { get; set; }

    public string RULE_DESC { get; set; }

    public string PREM_SPCL_CD { get; set; }

    public int CNFG_POP_SYS_ID { get; set; }

    public string LOB { get; set; }
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
