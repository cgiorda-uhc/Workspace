using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCPortal_Models.Dtos.ChemoPx;
public class ChemotherapyPX_Tracking_CUD_Dto
{
    public int? Id { get; set; }

    public int? ChemoPX_Id { get; set; }
    public string CODE { get; set; }
    public string GENERIC_NAME { get; set; }
    public string TRADE_NAME { get; set; }
    public bool? CKPT_INHIB_IND { get; set; }
    public bool? ANTI_EMETIC_IND { get; set; }
    public DateTime? CODE_EFF_DT { get; set; }
    public bool? NHNR_CANCER_THERAPY { get; set; }
    public Int16? CODE_CATEGORY_ID { get; set; }
    public Int16? ASP_CATEGORY_ID { get; set; }

    public Int16? DRUG_ADM_MODE_ID { get; set; }

    public Int16? PA_DRUGS_ID { get; set; }
    public DateTime? PA_EFF_DT { get; set; }
    public DateTime? PA_END_DT { get; set; }

    public Int16? CEP_PAY_CD_ID { get; set; }
 
    public Int16? CEP_ENROLL_CD_ID { get; set; }
    public string CEP_ENROLL_EXCL_DESC { get; set; }
    public bool? NOVEL_STATUS_IND { get; set; }
    public int? FIRST_NOVEL_MNTH { get; set; }
    public string SOURCE { get; set; }


    public DateTime? UPDATE_DT { get; set; }

    public string UPDATE_USER { get; set; }

    public string UPDATE_ACTION { get; set; }
}
