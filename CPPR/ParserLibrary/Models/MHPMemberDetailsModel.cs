using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagerLibrary.Models;

public class MHPMemberDetailsModel
{
    public Int64 mhp_uni_id { get; set; }
    public string SearchMethod { get; set; }
    public string PLN_VAR_SUBDIV_CD { get; set; }
    public DateTime mnth_eff_dt { get; set; }
    public string LEG_ENTY_NBR { get; set; }
    public string LEG_ENTY_FULL_NM { get; set; }
    public string HCE_LEG_ENTY_ROLLUP_DESC { get; set; }
    public string MKT_TYP_DESC { get; set; }
    public string CUST_SEG_NBR { get; set; }
    public string CUST_SEG_NM { get; set; }
    public string PRDCT_CD { get; set; }
    public string PRDCT_CD_DESC { get; set; }
    public string MKT_SEG_DESC { get; set; }
    public string MKT_SEG_RLLP_DESC { get; set; }
    public string MKT_SEG_CD { get; set; }

    public string FINC_ARNG_CD { get; set; }
    public string FINC_ARNG_DESC { get; set; }
    public string MBR_FST_NM { get; set; }
    public string MBR_LST_NM { get; set; }
    public DateTime BTH_DT { get; set; }
    public string MBR_ALT_ID { get; set; }
    public string MBR_ID { get; set; }
    public string PRDCT_SYS_ID { get; set; }
    public string CS_PRDCT_CD_SYS_ID { get; set; }

    public string CS_CO_CD { get; set; }

    public string CS_CO_CD_ST { get; set; }

    public string SBSCR_MEDCD_RCIP_NBR { get; set; }

}
