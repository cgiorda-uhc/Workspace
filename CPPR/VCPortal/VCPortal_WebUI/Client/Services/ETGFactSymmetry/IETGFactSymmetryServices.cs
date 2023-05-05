using VCPortal_Models.Dtos.ETGFactSymmetry;

namespace VCPortal_WebUI.Client.Services.ETGFactSymmetry;
public interface IETGFactSymmetryServices
{
    Task<List<ETGFactSymmetry_ReadDto>> GetETGFactSymmetryDisplayAsync();


}