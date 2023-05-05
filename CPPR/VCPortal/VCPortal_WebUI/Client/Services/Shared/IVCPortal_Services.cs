

namespace VCPortal_WebUI.Client.Services.Shared;

public interface IVCPortal_Services
{

    Task Insertlog(VCPortal_Models.Models.Shared.VCLog log);


    void RunExcelExport<T>(List<T> list, string worksheetTitle, List<string[]> columns);

}