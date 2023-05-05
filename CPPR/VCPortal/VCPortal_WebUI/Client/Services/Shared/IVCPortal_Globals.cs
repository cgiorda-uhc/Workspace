namespace VCPortal_WebUI.Client.Services.Shared;

public interface IVCPortal_Globals
{
    List<ChemotherapyPXFilters> ChemotherapyPXFilters { get; set; }
    List<ProcCodesModel> Proc_Codes { get; set; }

    DateTime Min { get;  }

    DateTime Max { get;  }
}