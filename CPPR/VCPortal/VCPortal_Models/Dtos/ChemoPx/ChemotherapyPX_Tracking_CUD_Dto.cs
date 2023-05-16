using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCPortal_Models.Dtos.ChemoPx;
public class ChemotherapyPX_Tracking_CUD_Dto
{
    public int? Id { get; set; }

    public int? ChemoPX_Id { get; set; }
    public string CODE { get; set; }

    [MaxLength(100, ErrorMessage = "The field {0}'s length must not be greater than {1} chars.")]
    public string GENERIC_NAME { get; set; }

    [MaxLength(50, ErrorMessage = "The field {0}'s length must not be greater than {1} chars.")]
    public string TRADE_NAME { get; set; }


    [Required]
    public bool? CKPT_INHIB_IND { get; set; }

    [Required]
    public bool? ANTI_EMETIC_IND { get; set; }
    public DateTime? CODE_EFF_DT { get; set; }

    [Required]
    public bool? NHNR_CANCER_THERAPY { get; set; }
    [Required]
    [Range(1, 50, ErrorMessage = "The field {0} is required.")]
    public Int16? CODE_CATEGORY_ID { get; set; }
    public Int16? ASP_CATEGORY_ID { get; set; }

    [Required]
    [Range(1, 50, ErrorMessage = "The field {0} is required.")]
    public Int16? DRUG_ADM_MODE_ID { get; set; }
    [Required]
    [Range(1, 50, ErrorMessage = "The field {0} is required.")]
    public Int16? PA_DRUGS_ID { get; set; }
    [Required]
    public DateTime? PA_EFF_DT { get; set; }
    [Required]
    public DateTime? PA_END_DT { get; set; }
    [Required]
    [Range(1, 50, ErrorMessage = "The field {0} is required.")]
    public Int16? CEP_PAY_CD_ID { get; set; }
    [Required]
    [Range(1, 50, ErrorMessage = "The field {0} is required.")]
    public Int16? CEP_ENROLL_CD_ID { get; set; }
    [MaxLength(12)]
    public string CEP_ENROLL_EXCL_DESC { get; set; }
    public bool? NOVEL_STATUS_IND { get; set; }
    public int? FIRST_NOVEL_MNTH { get; set; }
    [Required]
    public string SOURCE { get; set; }


    public DateTime? UPDATE_DT { get; set; }

    public string UPDATE_USER { get; set; }

    public string UPDATE_ACTION { get; set; }


    public bool IsValid { get; set; }
}
