using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCPortal_Models.Models.ProcCodeTrends;
public class CLM_PHYS_Model
{
    public string px { get; set; }
    public string px_desc { get; set; }
    public float year { get; set; }
    public float quarter { get; set; }
    public string LOB { get; set; }
    public string HLTH_PLN_FUND_DESC { get; set; }
    public string mapping_state { get; set; }
    public string region { get; set; }
    public string PRDCT_LVL_1_NM { get; set; }
    public string CS_TADM_PRDCT_MAP { get; set; }
    public string CS_PRDCT_CD_DESC { get; set; }
    public string TADM_LOB { get; set; }
    public string HCE_LEG_ENTY_ROLLUP_DESC { get; set; }
    public string SRC_SYS_GRP_DESC { get; set; }
    public string CS_DUAL_IND { get; set; }
    public string MR_DUAL_IND { get; set; }
    public string op_phys_bucket { get; set; }
    public string HP_PROV_STS_RLLP_DESC { get; set; }
    public float indv { get; set; }
    public float evnts { get; set; }
    public float claims { get; set; }
    public float fac_clms { get; set; }
    public float phy_clms { get; set; }
    public float oth_clms { get; set; }
    public float px_cnt { get; set; }
    public float adj_srv_uni { get; set; }
    public float allw_amt { get; set; }
    public float paid_amt { get; set; }

}
