
using VCPortal_Models.Dtos.ChemoPx;
using VCPortal_Models.Models.ChemoPx;
using VCPortal_Models.Models.Shared;

namespace DataAccessLibrary.Data.Abstract;
public interface IChemotherapyPX_Repo
{

    Task<IEnumerable<ChemotherapyPXModel>> GetAllChemotherapyPX();
    Task<ChemotherapyPXModel?> GetChemotherapyPX(int id);

    Task InsertChemotherapyPXTracking(List<ChemotherapyPX_Tracking_CUD_Dto> chemPX);


    Task<IEnumerable<ChemotherapyPXFilters>> GetAllFilters();

    Task<IEnumerable<ProcCodesModel>> GetAllProcCodes();

    Task<IEnumerable<Code_Category_Model>> GetAllCodeCategory();

    Task<IEnumerable<ASP_Category_Model>> GetAllASPCategory();
    Task<IEnumerable<Drug_Adm_Mode_Model>> GetAllDrugAdmMode();

   Task<IEnumerable<PA_Drugs_Model>> GetAllPADrugs();
    Task<IEnumerable<CEP_Pay_Cd_Model>> GetAllCEPPayCd();

    Task<IEnumerable<CEP_Enroll_Cd_Model>> GetAllCEPEnrollCd();


    Task<IEnumerable<string>> GetSource();
    Task<IEnumerable<string>> GetCEP_Enroll_Excl_Desc();
}