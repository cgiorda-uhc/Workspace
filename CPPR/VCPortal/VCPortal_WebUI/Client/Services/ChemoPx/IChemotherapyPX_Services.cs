

namespace VCPortal_WebUI.Client.Services.ChemoPx;
public interface IChemotherapyPX_Services
{
    Task DeleteChemoPXAsync(int? Id);
    Task<List<ASP_Category_Model>> GetAllASPCategory();
    Task<List<CEP_Enroll_Cd_Model>> GetAllCEPEnrollCd();
    Task<List<CEP_Pay_Cd_Model>> GetAllCEPPayCd();
    Task<List<Code_Category_Model>> GetAllCodeCategory();
    Task<List<Drug_Adm_Mode_Model>> GetAllDrugAdmMode();
    Task<List<ChemotherapyPXFilters>> GetAllFilters();
    Task<List<PA_Drugs_Model>> GetAllPADrugs();
    Task<List<ProcCodesModel>> GetAllProcCodes();
    Task<List<ChemotherapyPX_ReadDto>> GetChemoPXListAsync();
    Task<List<ChemotherapyPX_ReadDto>> GetChemoPXSingleAsync(int? Id);
    Task<int> InsertChemoPXAsync(ChemotherapyPX_CreateDto chemoPXToInsert);
    Task UpdateChemoPXAsync(ChemotherapyPX_UpdateDto chemoPXToUpdate);
}