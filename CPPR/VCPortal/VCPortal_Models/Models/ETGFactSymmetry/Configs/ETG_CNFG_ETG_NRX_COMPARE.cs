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
    public float Prior_RX_Rate { get; set; }
    public string Prior_RX_NRX { get; set; }
    public float Current_RX_Rate { get; set; }
    public string Current_RX_NRX { get; set; }
    public string Change { get; set; }


}
