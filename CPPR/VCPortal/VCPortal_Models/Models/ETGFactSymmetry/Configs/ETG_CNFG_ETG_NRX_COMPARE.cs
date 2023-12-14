using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCPortal_Models.Models.ETGFactSymmetry.Configs;
public class ETG_CNFG_ETG_NRX_COMPARE
{

    public string ETG_BAS_CLSS_NBR { get; set; }
    public string ETG_BASE_CLS_TRT_RPT_DESC { get; set; }

    public char CNCR_IND { get; set; }


    public Int64? Current_MEMBER_COUNT { get; set; }
    public Int64? Current_EPSD_COUNT { get; set; }
    public Int64? Current_ETGD_TOT_ALLW_AMT { get; set; }
    public Int64? Current_ETGD_RX_ALLW_AMT { get; set; }



    public float Prior_RX_Rate { get; set; }
    public string Prior_RX_NRX { get; set; }
    public float Current_RX_Rate { get; set; }
    public string Current_RX_NRX { get; set; }
    public string Change { get; set; }


}
