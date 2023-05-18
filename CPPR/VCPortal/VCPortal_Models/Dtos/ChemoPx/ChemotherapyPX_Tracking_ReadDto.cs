using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCPortal_Models.Dtos.ChemoPx;
public class ChemotherapyPX_Tracking_ReadDto
{

    public int? Tracking_Id { get; set; }

    public string CODE { get; set; }

    public string CODE_DESC { get; set; }

    public DateTime CODE_END_DT { get; set; }

    public DateTime CODE_END_DT_PREVIOUS { get; set; }
    public string CODE_TYPE { get; set; }

    public string CODE_TYPE_PREVIOUS { get; set; }

    public string GENERIC_NAME { get; set; }
    public string GENERIC_NAME_PREVIOUS { get; set; }
    public string TRADE_NAME { get; set; }
    public string TRADE_NAME_PREVIOUS { get; set; }
    public bool? CKPT_INHIB_IND { get; set; }

    public bool? CKPT_INHIB_IND_PREVIOUS { get; set; }
    public bool? ANTI_EMETIC_IND { get; set; }
    public bool? ANTI_EMETIC_IND_PREVIOUS { get; set; }
    public DateTime? CODE_EFF_DT { get; set; }
    public DateTime? CODE_EFF_DT_PREVIOUS { get; set; }
    public bool? NHNR_CANCER_THERAPY { get; set; }

    public bool? NHNR_CANCER_THERAPY_PREVIOUS { get; set; }
    public string CODE_CATEGORY { get; set; }
    public string CODE_CATEGORY_PREVIOUS { get; set; }
    public string ASP_CATEGORY { get; set; }
    public string ASP_CATEGORY_PREVIOUS { get; set; }

    public string DRUG_ADM_MODE { get; set; }

    public string DRUG_ADM_MODE_PREVIOUS { get; set; }


    public string PA_DRUGS { get; set; }
    public string PA_DRUGS_PREVIOUS { get; set; }
    public DateTime? PA_EFF_DT { get; set; }
    public DateTime? PA_EFF_DT_PREVIOUS { get; set; }
    public DateTime? PA_END_DT { get; set; }
    public DateTime? PA_END_DT_PREVIOUS { get; set; }
    public string CEP_PAY_CD { get; set; }
    public string CEP_PAY_CD_PREVIOUS { get; set; }
    public string CEP_ENROLL_CD { get; set; }
    public string CEP_ENROLL_CD_PREVIOUS { get; set; }
    public string CEP_ENROLL_EXCL_DESC { get; set; }
    public string CEP_ENROLL_EXCL_DESC_PREVIOUS { get; set; }
    public bool? NOVEL_STATUS_IND { get; set; }
    public bool? NOVEL_STATUS_IND_PREVIOUS { get; set; }
    public int? FIRST_NOVEL_MNTH { get; set; }
    public int? FIRST_NOVEL_MNTH_PREVIOUS { get; set; }
    public string SOURCE { get; set; }

    public string SOURCE_PREVIOUS { get; set; }

    public DateTime? UPDATE_DT { get; set; }

    public string UPDATE_USER { get; set; }

    public string UPDATE_ACTION { get; set; }
}
