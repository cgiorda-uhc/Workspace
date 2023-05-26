
using ActiveDirectoryLibrary;
using CommunityToolkit.Mvvm.ComponentModel;
using DocumentFormat.OpenXml.Office2010.Excel;
using SharedFunctionsLibrary;
using System.ComponentModel;
using VCPortal_Models.Dtos.ChemoPx;
using VCPortal_Models.Dtos.ETGFactSymmetry;
using VCPortal_Models.Models.ActiveDirectory;
using VCPortal_Models.Shared;
using VCPortal_WPF_ViewModel.Projects.ChemotherapyPX;
using VCPortal_WPF_ViewModel.Shared;

namespace VCPortal_WPF_ViewModel.Projects.ETGFactSymmetry;
public partial class ETGFactSymmetryViewModel : ObservableObject
{
    private ETGFactSymmetry_ReadDto _etg;

    public long ETG_Fact_Symmetry_Id => _etg.ETG_Fact_Symmetry_Id;
    public long ETG_Fact_Symmetry_Id_Previous => _etg.ETG_Fact_Symmetry_Id_Previous;
    public string ETG_Base_Class => _etg.ETG_Base_Class;
    public string ETG_Description => _etg.ETG_Description;
    public short Premium_Specialty_Id => _etg.Premium_Specialty_Id;
    public string Premium_Specialty => _etg.Premium_Specialty;
    private string _LOB;
    public string LOB
    {
        get
        {
            return _LOB;
        }
        set
        {
            _LOB = value;

            trackChanges(value, "LOB");
        }
    }
    public string LOBPrevious => _etg.LOBPrevious;

    private bool? _never_mapped;
    public bool? Never_Mapped
    {
        get
        {
            return _never_mapped;
        }
        set
        {
            _never_mapped = value;

            trackChanges(value, "Never_Mapped");
        }
    }
    public bool? Never_Mapped_Previous { get; set; }

    public bool? Has_Commercial { get; set; }
    public bool? Has_Medicare { get; set; }
    public bool? Has_Medicaid { get; set; }

    public bool? Has_Commercial_Previous => _etg.Has_Commercial_Previous;
    public bool? Has_Medicare_Previous => _etg.Has_Medicare_Previous;
    public bool? Has_Medicaid_Previous => _etg.Has_Medicaid_Previous;

    public bool? Has_RX { get; set; }
    public bool? Has_NRX { get; set; }

    public bool? Has_RX_Previous => _etg.Has_RX_Previous;
    public bool? Has_NRX_Previous => _etg.Has_NRX_Previous;
    private string _RX_NRX;
    public string RX_NRX
    {
        get
        {
            return _RX_NRX;
        }
        set
        {
            _RX_NRX = value;

            trackChanges(value, "RX_NRX");
        }
    }
    public string RX_NRXPrevious => _etg.RX_NRXPrevious;

    public char Is_Config => _etg.Is_Config;
    private string _PC_Treatment_Indicator;
    public string PC_Treatment_Indicator
    {
        get
        {
            return _PC_Treatment_Indicator;
        }
        set
        {
            _PC_Treatment_Indicator = value;

            trackChanges(value, "PC_Treatment_Indicator");
        }
    }
    public string PC_Treatment_Indicator_Previous => _etg.PC_Treatment_Indicator_Previous;

    private string _PC_Attribution;
    public string PC_Attribution
    {
        get
        {
            return _PC_Attribution;
        }
        set
        {
            _PC_Attribution = value;

            trackChanges(value, "PC_Attribution");
        }
    }
    public string PC_Attribution_Previous => _etg.PC_Attribution_Previous;
    public float PC_Episode_Count => _etg.PC_Episode_Count;
    public float PC_Total_Cost => _etg.PC_Total_Cost;
    public float PC_Average_Cost => _etg.PC_Average_Cost;
    public float PC_Coefficients_of_Variation => _etg.PC_Coefficients_of_Variation;
    public float PC_Normalized_Pricing_Episode_Count => _etg.PC_Normalized_Pricing_Episode_Count;
    public float PC_Normalized_Pricing_Total_Cost => _etg.PC_Normalized_Pricing_Total_Cost;
    public float PC_Spec_Episode_Count => _etg.PC_Spec_Episode_Count;

    public float PC_Spec_Episode_Count_Previous => _etg.PC_Spec_Episode_Count_Previous;

    public float? PC_Spec_Episode_Count_Diff => _etg.PC_Spec_Episode_Count_Diff;


    public float PC_Spec_Episode_Distribution => _etg.PC_Spec_Episode_Distribution;
    public float PC_Spec_Percent_of_Episodes => _etg.PC_Spec_Percent_of_Episodes;
    public float PC_Spec_Total_Cost => _etg.PC_Spec_Total_Cost;
    public float PC_Spec_Average_Cost => _etg.PC_Spec_Average_Cost;
    public float PC_Spec_CV => _etg.PC_Spec_CV;
    public string PC_Measure_Status => _etg.PC_Measure_Status;
    public string PC_Changes_Made => _etg.PC_Changes_Made;

    private string _PC_Change_Comments;
    public string PC_Change_Comments
    {
        get
        {
            return _PC_Change_Comments;
        }
        set
        {
            _PC_Change_Comments = value;

            trackChanges(value, "PC_Change_Comments");
        }
    }
    private string _Patient_Centric_Mapping;
    public string Patient_Centric_Mapping
    {
        get
        {
            return _Patient_Centric_Mapping;
        }
        set
        {
            _Patient_Centric_Mapping = value;

            trackChanges(value, "Patient_Centric_Mapping");
        }
    }
    public string Patient_Centric_Mapping_Previous => _etg.Patient_Centric_Mapping_Previous;

    private string _Patient_Centric_Change_Comments;
    public string Patient_Centric_Change_Comments
    {
        get
        {
            return _Patient_Centric_Change_Comments;
        }
        set
        {
            _Patient_Centric_Change_Comments = value;

            trackChanges(value, "Patient_Centric_Change_Comments");
        }
    }
    private string _EC_Treatment_Indicator;
    public string EC_Treatment_Indicator
    {
        get
        {
            return _EC_Treatment_Indicator;
        }
        set
        {
            var oldvalue = _EC_Treatment_Indicator;
            _EC_Treatment_Indicator = value;

            if (oldvalue == null)
                return;

            trackChanges(value, "EC_Treatment_Indicator");
        }
    }
    public string EC_Treatment_Indicator_Previous => _etg.EC_Treatment_Indicator_Previous;
    public float EC_Spec_Episode_Distribution => _etg.EC_Spec_Episode_Distribution;
    public float EC_Spec_Percent_of_Episodes => _etg.EC_Spec_Percent_of_Episodes;
    public float EC_Spec_Total_Cost => _etg.EC_Spec_Total_Cost;
    public float EC_Spec_Average_Cost => _etg.EC_Spec_Average_Cost;
    public float EC_Coefficients_of_Variation => _etg.EC_Coefficients_of_Variation;
    public float EC_Episode_Count => _etg.EC_Episode_Count;




    public float EC_Normalized_Pricing_Total_Cost => _etg.EC_Normalized_Pricing_Total_Cost;
    public float EC_Spec_Episode_Count => _etg.EC_Spec_Episode_Count;

    public float EC_Spec_Episode_Count_Previous => _etg.EC_Spec_Episode_Count_Previous;
    public float? EC_Spec_Episode_Count_Diff => _etg.EC_Spec_Episode_Count_Diff;






    public float EC_Total_Cost => _etg.EC_Total_Cost;
    public float EC_Average_Cost => _etg.EC_Average_Cost;
    public float EC_Spec_CV => _etg.EC_Spec_CV;
    public string EC_Changes_Made => _etg.EC_Changes_Made;

    private string _EC_Mapping;
    public string EC_Mapping
    {
        get
        {
            return _EC_Mapping;
        }
        set
        {
            _EC_Mapping = value;

            trackChanges(value, "EC_Mapping");
        }
    }
    public string EC_Mapping_Previous => _etg.EC_Mapping_Previous;
    private string _EC_Change_Comments;
    public string EC_Change_Comments
    {
        get
        {
            return _EC_Change_Comments;
        }
        set
        {
            _EC_Change_Comments = value;

            trackChanges(value, "EC_Change_Comments");
        }
    }
    public string Data_Period => _etg.Data_Period;
    public string Data_Period_Previous => _etg.Data_Period_Previous;
    public float Symmetry_Version => _etg.Symmetry_Version;
    public float Symmetry_Version_Previous => _etg.Symmetry_Version_Previous;



    public ETGFactSymmetryViewModel(ETGFactSymmetry_ReadDto etg)
    {
        _etg = etg;
        LOB = etg.LOB;
        Has_Commercial = etg.Has_Commercial;
        Has_Medicare = etg.Has_Medicare;
        Has_Medicaid = etg.Has_Medicaid;
        Has_RX = etg.Has_RX;
        Has_NRX = etg.Has_NRX;
        RX_NRX = etg.RX_NRX;
        PC_Treatment_Indicator = etg.PC_Treatment_Indicator;
        PC_Attribution = etg.PC_Attribution;
        Patient_Centric_Mapping = etg.Patient_Centric_Mapping;
        EC_Treatment_Indicator = etg.EC_Treatment_Indicator;
        EC_Mapping = etg.EC_Mapping;
        PC_Change_Comments = etg.PC_Change_Comments;
        EC_Change_Comments = etg.EC_Change_Comments;
        Patient_Centric_Change_Comments = etg.Patient_Centric_Change_Comments;
        Never_Mapped = etg.Never_Mapped;
        Never_Mapped_Previous = etg.Never_Mapped_Previous;

}


    private void trackChanges(object newValue, string propName)
    {
        if (newValue == null)
        {
            return;
        }

        var etg = SharedETGSymmObjects.ETGFactSymmetry_Tracking_List.FirstOrDefault(x => x.ETG_Fact_Symmetry_id == _etg.ETG_Fact_Symmetry_Id);
        if (etg == null)
        {
            //etg = new ETGFactSymmetry_Tracking_UpdateDto();
            // etg.ETG_Fact_Symmetry_id = _etg.ETG_Fact_Symmetry_Id;
            etg = AutoMapping<ETGFactSymmetry_ReadDto, ETGFactSymmetry_Tracking_UpdateDto>.Map(_etg);
            //VCAutoMapper.AutoMapUserAccess<ETGFactSymmetry_ReadDto, ETGFactSymmetry_Tracking_UpdateDto>(_etg);

            //etg = _etg;
            SharedETGSymmObjects.ETGFactSymmetry_Tracking_List.Add(etg);
        }

        switch (propName)
        {
            case "LOB":
                etg.Has_Commercial = (newValue.ToString().ToLower().Contains("not mapped") ? null :( newValue.ToString().ToLower().Contains("commercial")  || newValue.ToString().ToLower().Contains("all") ? true : false));
                etg.Has_Medicare = (newValue.ToString().ToLower().Contains("not mapped") ? null : (newValue.ToString().ToLower().Contains("medicare") || newValue.ToString().ToLower().Contains("all") ? true : false));
                etg.Has_Medicaid = (newValue.ToString().ToLower().Contains("not mapped") ? null : (newValue.ToString().ToLower().Contains("medicaid") || newValue.ToString().ToLower().Contains("all") ? true : false));
                _etg.Has_Commercial = etg.Has_Commercial;
                _etg.Has_Medicare = etg.Has_Medicare;
                _etg.Has_Medicaid = etg.Has_Medicaid;
                break;
            case "RX_NRX":
                etg.Has_RX = (newValue.ToString().ToLower().Contains("rx: y /") ? true : (newValue.ToString().ToLower().Contains("rx: n /") ? false : null));
                etg.Has_NRX = (newValue.ToString().ToLower().Contains("/ nrx: y") ? true : (newValue.ToString().ToLower().Contains("/ nrx: n") ? false : null));
                _etg.Has_RX = etg.Has_RX;
                _etg.Has_NRX = etg.Has_NRX;
                break;
            case "PC_Treatment_Indicator":
                etg.PC_Treatment_Indicator = (newValue.ToString().ToLower().Contains("not mapped") ? null : newValue.ToString());
                _etg.PC_Treatment_Indicator = etg.PC_Treatment_Indicator;
                break;
            case "PC_Attribution":
                etg.PC_Attribution = (newValue.ToString().ToLower().Contains("not mapped") ? null : newValue.ToString());
                _etg.PC_Attribution = etg.PC_Attribution;
                break;
            case "PC_Change_Comments":
                etg.PC_Change_Comments = newValue.ToString();
                _etg.PC_Change_Comments = etg.PC_Change_Comments;
                break;
            case "Patient_Centric_Mapping":
                etg.Patient_Centric_Mapping = (newValue.ToString().ToLower().Contains("not mapped") ? null : newValue.ToString());
                _etg.Patient_Centric_Mapping = etg.Patient_Centric_Mapping;
                break;
            case "Patient_Centric_Change_Comments":
                etg.Patient_Centric_Change_Comments = newValue.ToString();
                _etg.Patient_Centric_Change_Comments = etg.Patient_Centric_Change_Comments;
                break;
            case "EC_Treatment_Indicator":
                etg.EC_Treatment_Indicator = newValue.ToString();
                _etg.EC_Treatment_Indicator = etg.EC_Treatment_Indicator;
                break;
            case "EC_Mapping":
                etg.EC_Mapping = newValue.ToString();
                _etg.EC_Mapping =etg.EC_Mapping;
                break;
            case "EC_Change_Comments":
                etg.EC_Change_Comments = newValue.ToString();
                _etg.EC_Change_Comments = etg.EC_Change_Comments;
                break;
            case "Never_Mapped":
                etg.Never_Mapped = newValue as bool?;
                _etg.Never_Mapped = etg.Never_Mapped;
                break;
            default:
                // code block
                break;
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
