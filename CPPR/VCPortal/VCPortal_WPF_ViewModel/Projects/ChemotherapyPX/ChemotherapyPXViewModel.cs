
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Office.Interop.Excel;
using NPOI.SS.Formula.PTG;
using SharedFunctionsLibrary;
using System.ComponentModel.DataAnnotations;
using VCPortal_Models.Dtos.ChemoPx;
using VCPortal_Models.Dtos.ETGFactSymmetry;
using VCPortal_WPF_ViewModel.Shared;

namespace VCPortal_WPF_ViewModel.Projects.ChemotherapyPX;
public partial class ChemotherapyPXViewModel : ObservableValidator
{
    
    private ChemotherapyPX_ReadDto _chmpx;


    //[ObservableProperty]
    //private bool is_Valid;

    public int? Id => _chmpx.Id;

    private string _code;
    public string CODE
    {
        get
        {
            return _code;
        }
        set
        {
            if (value == null)
            {
                return;
            }

            if (value.Contains("-"))
            {

                //ONLY NEW ROWS CAN SET A CODE
                if (_chmpx.Id != null)
                {
                    return;
                }

                _code = value.Split('-')[0].Trim();
                var proc = SharedObjects.ProcCodes.Where(x => x.Proc_Cd == _code).FirstOrDefault();

                CODE_DESC = proc.Proc_Desc;
                CODE_END_DT = proc.Proc_Cd_Date;
                CODE_TYPE = proc.Proc_Cd_Type;
                //_code = value;

                trackChanges(_code, "CODE");

                SharedObjects.ProcCodes.Remove(SharedObjects.ProcCodes.Where(x => x.Proc_Cd == _code).FirstOrDefault());
            }
            else
            {
                //DEFAULT Value. 
                _code = value;
            }
        }
    }


    [ObservableProperty]
    private string cODE_DESC;

    public string CODE_DESC_REF { get; set; }

    private string _GENERIC_NAME;
    [MaxLength(100, ErrorMessage = "The field {0}'s length must not be greater than {1} chars.")]
    public string GENERIC_NAME
    {
        get
        {
            return _GENERIC_NAME;
        }
        set
        {

            //ValidationContext validationContext = new ValidationContext(this, null, null);
            //validationContext.MemberName = "GENERIC_NAME";
            //Validator.ValidateProperty(value, validationContext);


            //var oldvalue = _GENERIC_NAME;
            SetProperty(ref _GENERIC_NAME, value, true);
            //_GENERIC_NAME = value;

            //if (oldvalue == null && _action != "INSERT")
            //    return;

            trackChanges(value, "GENERIC_NAME");
        }
    }

    private string _TRADE_NAME;
    [MaxLength(50, ErrorMessage = "The field {0}'s length must not be greater than {1} chars.")]
    public string TRADE_NAME
    {
        get
        {
            return _TRADE_NAME;
        }
        set
        {
            //var oldvalue = _TRADE_NAME;
            SetProperty(ref _TRADE_NAME, value, true);
           // _TRADE_NAME = value;

            //if (oldvalue == null && _action != "INSERT")
            //    return;

            trackChanges(value, "TRADE_NAME");
        }
    }
    private bool? _CKPT_INHIB_IND;
    [Required]
    public bool? CKPT_INHIB_IND
    {
        get
        {
            return _CKPT_INHIB_IND;
        }
        set
        {
            //var oldvalue = _CKPT_INHIB_IND;
            SetProperty(ref _CKPT_INHIB_IND, value, true); 
            //_CKPT_INHIB_IND = value;

            //if (oldvalue == null && _action != "INSERT")
                //return;

            trackChanges(value, "CKPT_INHIB_IND");
        }
    }
    private bool? _ANTI_EMETIC_IND;
    [Required]
    public bool? ANTI_EMETIC_IND
    {
        get
        {
            return _ANTI_EMETIC_IND;
        }
        set
        {
            //var oldvalue = _ANTI_EMETIC_IND;
            SetProperty(ref _ANTI_EMETIC_IND, value, true); 
            //_ANTI_EMETIC_IND = value;

            //if (oldvalue == null && _action != "INSERT")
                //return;

            trackChanges(value, "ANTI_EMETIC_IND");
        }
    }

    [ObservableProperty]
    private string cODE_TYPE;
    public string CODE_TYPE_REF { get; set; }

    private DateTime? _CODE_EFF_DT;
    public DateTime? CODE_EFF_DT
    {
        get
        {
            return _CODE_EFF_DT;
        }
        set
        {
            //var oldvalue = _CODE_EFF_DT;
            SetProperty(ref _CODE_EFF_DT, value, true); 
            //_CODE_EFF_DT = value;

            //if (oldvalue == null && _action != "INSERT")
            //    return;

            trackChanges(value, "CODE_EFF_DT");
        }
    }

    [ObservableProperty]
    private DateTime? cODE_END_DT;
    public DateTime? CODE_END_DT_REF { get; set; }

    private bool? _NHNR_CANCER_THERAPY;
    [Required]
    public bool? NHNR_CANCER_THERAPY
    {
        get
        {
            return _NHNR_CANCER_THERAPY;
        }
        set
        {
            //var oldvalue = _NHNR_CANCER_THERAPY;
            SetProperty(ref _NHNR_CANCER_THERAPY, value, true); 
            //_NHNR_CANCER_THERAPY = value;

            //if (oldvalue == null && _action != "INSERT")
            //    return;

            trackChanges(value, "NHNR_CANCER_THERAPY");
        }
    }
    public string CODE_CATEGORY { get; set; }

    private Int16? _CODE_CATEGORY_ID;
    [Required]
    [Range(1, 50, ErrorMessage = "The field {0} is required.")]
    public Int16? CODE_CATEGORY_ID
    {
        get
        {
            return _CODE_CATEGORY_ID;
        }
        set
        {
            //var oldvalue = _CODE_CATEGORY_ID;
            SetProperty(ref _CODE_CATEGORY_ID, value, true); 
            //_CODE_CATEGORY_ID = value;

            //if (oldvalue == null && _action != "INSERT")
            //    return;

            trackChanges(value, "CODE_CATEGORY_ID");
        }
    }

    public string ASP_CATEGORY { get; set; }

    private Int16? _ASP_CATEGORY_ID;
    public Int16? ASP_CATEGORY_ID
    {
        get
        {
            return _ASP_CATEGORY_ID;
        }
        set
        {
            //var oldvalue = _ASP_CATEGORY_ID;
            SetProperty(ref _ASP_CATEGORY_ID, value, true);
            //_ASP_CATEGORY_ID = value;

            //if (oldvalue == null && _action != "INSERT")
            //    return;

            trackChanges(value, "ASP_CATEGORY_ID");
        }
    }

    public string DRUG_ADM_MODE { get; set; }

    private Int16? _DRUG_ADM_MODE_ID;
    [Required]
    [Range(1, 50, ErrorMessage = "The field {0} is required.")]
    public Int16? DRUG_ADM_MODE_ID
    {
        get
        {
            return _DRUG_ADM_MODE_ID;
        }
        set
        {
            //var oldvalue = _DRUG_ADM_MODE_ID;
            SetProperty(ref _DRUG_ADM_MODE_ID, value, true); 
            //_DRUG_ADM_MODE_ID = value;

            //if (oldvalue == null && _action != "INSERT")
            //    return;

            trackChanges(value, "DRUG_ADM_MODE_ID");
        }
    }
    public string PA_DRUGS { get; set; }


    private Int16? _PA_DRUGS_ID;
    [Required]
    [Range(1, 50, ErrorMessage = "The field {0} is required.")]
    public Int16? PA_DRUGS_ID
    {
        get
        {
            return _PA_DRUGS_ID;
        }
        set
        {
            //var oldvalue = _PA_DRUGS_ID;
            SetProperty(ref _PA_DRUGS_ID, value, true); 
            //_PA_DRUGS_ID = value;

            //if (oldvalue == null && _action != "INSERT")
                //return;

            trackChanges(value, "PA_DRUGS_ID");
        }
    }

    private DateTime? _PA_EFF_DT;
    [Required]
    public DateTime? PA_EFF_DT
    {
        get
        {
            return _PA_EFF_DT;
        }
        set
        {
            //var oldvalue = _PA_EFF_DT;
            SetProperty(ref _PA_EFF_DT, value, true); 
            //_PA_EFF_DT = value;

            //if (oldvalue == null && _action != "INSERT")
            //    return;

            trackChanges(value, "PA_EFF_DT");
        }
    }

    private DateTime? _PA_END_DT;
    [Required]
    public DateTime? PA_END_DT
    {
        get
        {
            return _PA_END_DT;
        }
        set
        {
            //var oldvalue = _PA_END_DT;
            SetProperty(ref _PA_END_DT, value, true); 
            //_PA_END_DT = value;

            //if (oldvalue == null && _action != "INSERT")
                //return;

            trackChanges(value, "PA_END_DT");
        }
    }

    public string CEP_PAY_CD { get; set; }

    private Int16? _CEP_PAY_CD_ID;
    [Required]
    [Range(1, 50, ErrorMessage = "The field {0} is required.")]
    public Int16? CEP_PAY_CD_ID
    {
        get
        {
            return _CEP_PAY_CD_ID;
        }
        set
        {
            //var oldvalue = _CEP_PAY_CD_ID;
            SetProperty(ref _CEP_PAY_CD_ID, value, true); 
            //_CEP_PAY_CD_ID = value;

            //if (oldvalue == null && _action != "INSERT")
            //    return;

            trackChanges(value, "CEP_PAY_CD_ID");
        }
    }

    public string CEP_ENROLL_CD { get; set; }

    private Int16? _CEP_ENROLL_CD_ID;
    [Required]
    [Range(1, 50, ErrorMessage = "The field {0} is required.")]
    public Int16? CEP_ENROLL_CD_ID
    {
        get
        {
            return _CEP_ENROLL_CD_ID;
        }
        set
        {
            //var oldvalue = _CEP_ENROLL_CD_ID;
            SetProperty(ref _CEP_ENROLL_CD_ID, value, true); 
            //_CEP_ENROLL_CD_ID = value;

            //if (oldvalue == null && _action != "INSERT")
            //    return;

            trackChanges(value, "CEP_ENROLL_CD_ID");
        }
    }

    private string _CEP_ENROLL_EXCL_DESC;
    [MaxLength(12)]
    public string CEP_ENROLL_EXCL_DESC
    {
        get
        {
            return _CEP_ENROLL_EXCL_DESC;
        }
        set
        {
           // var oldvalue = _CEP_ENROLL_EXCL_DESC;
            SetProperty(ref _CEP_ENROLL_EXCL_DESC, value, true); 
            //_CEP_ENROLL_EXCL_DESC = value;

            //if (oldvalue == null && _action != "INSERT")
            //    return;

            trackChanges(value, "CEP_ENROLL_EXCL_DESC");
        }
    }

    private bool? _NOVEL_STATUS_IND;
    public bool? NOVEL_STATUS_IND
    {
        get
        {
            return _NOVEL_STATUS_IND;
        }
        set
        {
            //var oldvalue = _NOVEL_STATUS_IND;
            SetProperty(ref _NOVEL_STATUS_IND, value, true); 
            //_NOVEL_STATUS_IND = value;

            //if (oldvalue == null && _action != "INSERT")
            //    return;

            trackChanges(value, "NOVEL_STATUS_IND");
        }
    }

    private int? _FIRST_NOVEL_MNTH;
    public int? FIRST_NOVEL_MNTH
    {
        get
        {
            return _FIRST_NOVEL_MNTH;
        }
        set
        {
            //var oldvalue = (_FIRST_NOVEL_MNTH == 0 ? null : _FIRST_NOVEL_MNTH);
            SetProperty(ref _FIRST_NOVEL_MNTH, value, true); 
            //_FIRST_NOVEL_MNTH = (value == 0 ? null : value);

            //if (oldvalue == null && _action != "INSERT")
            //    return;

            //trackChanges((value == 0 ? null : value), "FIRST_NOVEL_MNTH");
            trackChanges( value, "FIRST_NOVEL_MNTH");
        }
    }

    private string _SOURCE;
    [Required]
    public string SOURCE
    {
        get
        {
            return _SOURCE;
        }
        set
        {
            //var oldvalue = _SOURCE;
            SetProperty(ref _SOURCE, value, true);
            //_SOURCE = value;

            //if (oldvalue == null && _action != "INSERT")
            //    return;

            trackChanges(value, "SOURCE");
        }
    }



    public ChemotherapyPXViewModel(ChemotherapyPX_ReadDto chmpx)
    {
        _chmpx = chmpx;

        //Is_Valid = true;

        _code = chmpx.CODE;
        CODE_DESC = chmpx.CODE_DESC;
        CODE_DESC_REF = chmpx.CODE_DESC;
        GENERIC_NAME = chmpx.GENERIC_NAME;
        TRADE_NAME = chmpx.TRADE_NAME;
        CKPT_INHIB_IND = chmpx.CKPT_INHIB_IND;
        ANTI_EMETIC_IND = chmpx.ANTI_EMETIC_IND;
        CODE_TYPE = chmpx.CODE_TYPE;
        CODE_TYPE_REF = chmpx.CODE_TYPE;
        CODE_EFF_DT = chmpx.CODE_EFF_DT;
        CODE_END_DT = chmpx.CODE_END_DT;
        CODE_END_DT_REF = chmpx.CODE_END_DT;
        NHNR_CANCER_THERAPY = chmpx.NHNR_CANCER_THERAPY;
        CODE_CATEGORY = chmpx.CODE_CATEGORY;
        CODE_CATEGORY_ID = chmpx.CODE_CATEGORY_ID;
        ASP_CATEGORY = chmpx.ASP_CATEGORY;
        ASP_CATEGORY_ID = chmpx.ASP_CATEGORY_ID;
        DRUG_ADM_MODE = chmpx.DRUG_ADM_MODE;
        DRUG_ADM_MODE_ID = chmpx.DRUG_ADM_MODE_ID;
        PA_DRUGS = chmpx.PA_DRUGS;
        PA_DRUGS_ID = chmpx.PA_DRUGS_ID;
        PA_EFF_DT = chmpx.PA_EFF_DT;
        PA_END_DT = chmpx.PA_END_DT;
        CEP_PAY_CD = chmpx.CEP_PAY_CD;
        CEP_PAY_CD_ID = chmpx.CEP_PAY_CD_ID;
        CEP_ENROLL_CD = chmpx.CEP_ENROLL_CD;
        CEP_ENROLL_CD_ID = chmpx.CEP_ENROLL_CD_ID;
        CEP_ENROLL_EXCL_DESC = chmpx.CEP_ENROLL_EXCL_DESC;
        NOVEL_STATUS_IND = chmpx.NOVEL_STATUS_IND;
        FIRST_NOVEL_MNTH = chmpx.FIRST_NOVEL_MNTH;
        SOURCE = chmpx.SOURCE;

    }



    private void trackChanges(object newValue, string propName)
    {
        if(newValue == null)
        {
            return;
        }


        //FIX VALIDATION public class ValidationBase
        //ValidateProperty(newValue, propName);
        //base.NotifyPropertyChanged(propName);


        var chemo = SharedChemoObjects.ChemotherapyPX_Tracking_List.FirstOrDefault(x => x.CODE == _code);
        if (chemo == null)
        {
            //chemo = new ChemotherapyPX_Tracking_CUD_Dto();

            chemo = AutoMapping<ChemotherapyPX_ReadDto, ChemotherapyPX_Tracking_CUD_Dto>.Map(_chmpx);


            if(chemo.CODE == null)
            {
                chemo.CODE = _code;
            }

            if (Id == null)
            {
                //chemo.CODE = _code;
                chemo.UPDATE_ACTION = "INSERT";
            }
            else
            {
                chemo.ChemoPX_Id = Id;
                chemo.CODE = _code;
                chemo.UPDATE_ACTION = "UPDATE";
            }

            
            SharedChemoObjects.ChemotherapyPX_Tracking_List.Add(chemo);
        }




        switch (propName)
        {
            case "CODE":
                chemo.CODE = newValue.ToString();
                _chmpx.CODE = chemo.CODE;
                break;
            case "GENERIC_NAME":
                chemo.GENERIC_NAME = newValue.ToString();
                _chmpx.GENERIC_NAME = chemo.GENERIC_NAME;
                break;
            case "TRADE_NAME":
                chemo.TRADE_NAME = newValue.ToString();
                _chmpx.TRADE_NAME = chemo.TRADE_NAME;
                break;
            case "CKPT_INHIB_IND":
                chemo.CKPT_INHIB_IND = newValue as bool?;
                _chmpx.CKPT_INHIB_IND = chemo.CKPT_INHIB_IND;
                break;
            case "ANTI_EMETIC_IND":
                chemo.ANTI_EMETIC_IND = newValue as bool?;
                _chmpx.ANTI_EMETIC_IND = chemo.ANTI_EMETIC_IND;
                break;
            case "CODE_EFF_DT":
                chemo.CODE_EFF_DT = newValue as DateTime?;
                _chmpx.CODE_EFF_DT = chemo.CODE_EFF_DT;
                break;
            case "NHNR_CANCER_THERAPY":
                chemo.NHNR_CANCER_THERAPY = newValue as bool?;
                _chmpx.NHNR_CANCER_THERAPY = chemo.NHNR_CANCER_THERAPY;
                break;
            case "CODE_CATEGORY_ID":
                chemo.CODE_CATEGORY_ID = newValue as short?;
                _chmpx.CODE_CATEGORY_ID = chemo.CODE_CATEGORY_ID;
                break;
            case "ASP_CATEGORY_ID":
                chemo.ASP_CATEGORY_ID = newValue as short?;
                _chmpx.ASP_CATEGORY_ID = chemo.ASP_CATEGORY_ID;
                break;
            case "DRUG_ADM_MODE_ID":
                chemo.DRUG_ADM_MODE_ID = newValue as short?;
                _chmpx.DRUG_ADM_MODE_ID = chemo.DRUG_ADM_MODE_ID;
                break;
            case "PA_DRUGS_ID":
                chemo.PA_DRUGS_ID = newValue as short?;
                _chmpx.PA_DRUGS_ID = chemo.PA_DRUGS_ID;
                break;
            case "PA_EFF_DT":
                chemo.PA_EFF_DT = newValue as DateTime?;
                _chmpx.PA_EFF_DT = chemo.PA_EFF_DT;
                break;
            case "PA_END_DT":
                chemo.PA_END_DT = newValue as DateTime?;
                _chmpx.PA_END_DT = chemo.PA_END_DT;
                break;
            case "CEP_PAY_CD_ID":
                chemo.CEP_PAY_CD_ID = newValue as short?;
                _chmpx.CEP_PAY_CD_ID = chemo.CEP_PAY_CD_ID;
                break;
            case "CEP_ENROLL_CD_ID":
                chemo.CEP_ENROLL_CD_ID = newValue as short?;
                _chmpx.CEP_ENROLL_CD_ID = chemo.CEP_ENROLL_CD_ID;
                break;
            case "CEP_ENROLL_EXCL_DESC":
                chemo.CEP_ENROLL_EXCL_DESC = newValue.ToString();
                _chmpx.CEP_ENROLL_EXCL_DESC = chemo.CEP_ENROLL_EXCL_DESC;
                break;
            case "NOVEL_STATUS_IND":
                chemo.NOVEL_STATUS_IND = newValue as bool?;
                _chmpx.NOVEL_STATUS_IND = chemo.NOVEL_STATUS_IND;
                break;
            case "FIRST_NOVEL_MNTH":
                chemo.FIRST_NOVEL_MNTH = newValue as int?;
                //chemo.FIRST_NOVEL_MNTH = (chemo.FIRST_NOVEL_MNTH == 0 ? null : chemo.FIRST_NOVEL_MNTH);
                _chmpx.FIRST_NOVEL_MNTH = chemo.FIRST_NOVEL_MNTH;
                break;
            case "SOURCE":
                chemo.SOURCE = newValue.ToString();
                _chmpx.SOURCE = chemo.SOURCE;
                break;
            default:
                // code block
                break;
        }

        ValidationContext context = new ValidationContext(chemo, null, null);
        List<ValidationResult> validationResults = new List<ValidationResult>();
        chemo.IsValid = Validator.TryValidateObject(chemo, context, validationResults, true);
        if (!chemo.IsValid)
        {
            foreach (ValidationResult validationResult in validationResults)
            {
                Console.WriteLine("{0}", validationResult.ErrorMessage);
            }
        }

    }



    //bool disposed;
    //protected virtual void Dispose(bool disposing)
    //{
    //    if (!disposed)
    //    {
    //        if (disposing)
    //        {
    //        }
    //    }
    //    //dispose unmanaged resources
    //    disposed = true;
    //}

    //public void Dispose()
    //{
    //    Dispose(true);
    //    GC.SuppressFinalize(this);
    //}


}
