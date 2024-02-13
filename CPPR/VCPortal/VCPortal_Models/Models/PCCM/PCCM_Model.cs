using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCPortal_Models.Models.PCCM;
public class PCCM_Model
{
    public Int64? MBR_ID { get; set; }
    public string INDV_ID { get; set; }
    public Int64? MBR_PGM_ID { get; set; }
    public string PGM_CATGY_TYP_DESC { get; set; }
    public string PGM_TYP_DESC { get; set; }
    public string CALC_PGM_TYP { get; set; }
    public string NOM_DEPT_TYP_DESC { get; set; }
    public string NOM_RSN_TYP_DESC { get; set; }
    public string MBR_PGM_STS_TYP_DESC { get; set; }
    public string MBR_PGM_STS_RSN_TYP_DESC { get; set; }
    public DateTime? CREAT_DT { get; set; }
    public DateTime? PRE_ENRL_DT { get; set; }
    public DateTime? OPS_ENROLLED_DT { get; set; }
    public DateTime? OPS_ENGAGED_DT { get; set; }
    public DateTime? END_DT { get; set; }
    public bool? OPS_IDENTIFIED { get; set; }
    public bool? OPS_QUALIFIED { get; set; }
    public bool? OPS_ATTEMPTED { get; set; }
    public bool? OPS_CONTACTED { get; set; }
    public bool? OPS_MBR_CONTACTED { get; set; }
    public bool? OPS_ENROLLED { get; set; }
    public bool? OPS_ENGAGED { get; set; }
    public bool? PSU_IND { get; set; }
    public string PSU_NEW_ORIG { get; set; }

    public string PSU_NEW { get; set; }

    public string RPT_MTH_YR_DISPLAY { get; set; }

    public string RPT_MTH { get; set; }

    public string RPT_YR { get; set; }

    public int RPT_DAYS { get; set; }


    public DateTime? RPT_DATE { get; set; }

    public string QUAL_CATEG { get; set; }

    public string ENRL_CATEG { get; set; }
}
