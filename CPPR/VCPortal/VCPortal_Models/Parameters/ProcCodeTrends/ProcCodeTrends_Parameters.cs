using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCPortal_Models.Models.ProcCodeTrends;

namespace VCPortal_Models.Parameters.ProcCodeTrends;
public class ProcCodeTrends_Parameters
{
    public string LOB { get; set; }
    public string Region { get; set; }
    public string mapping_state { get; set; }
    public string PRDCT_LVL_1_NM { get; set; }
    public string CS_TADM_PRDCT_MAP { get; set; }
    public string HLTH_PLN_FUND_DESC { get; set; }
    public string HCE_LEG_ENTY_ROLLUP_DESC { get; set; }
    public string SRC_SYS_GRP_DESC { get; set; }
    public string CS_DUAL_IND { get; set; }
    public string MR_DUAL_IND { get; set; }

    public string px { get; set; }

    public int RowCount { get; set; }

    public List<DateSpan_Model> DateSpanList { get; set; }
}
