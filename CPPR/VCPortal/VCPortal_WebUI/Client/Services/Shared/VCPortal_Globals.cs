

using Microsoft.AspNetCore.Components.Web;
using System.Net.Http;

namespace VCPortal_WebUI.Client.Services.Shared;

public class VCPortal_Globals : IVCPortal_Globals
{


    public DateTime Min { get;  }

    public DateTime Max { get; }


    public VCPortal_Globals()
    {
        Min = new DateTime(1800, 1, 1, 8, 15, 0);
        Max = new DateTime(2025, 1, 1, 19, 30, 45);
    }


    public List<ProcCodesModel> Proc_Codes { get; set; }


    public List<ChemotherapyPXFilters> ChemotherapyPXFilters { get; set; }





}
