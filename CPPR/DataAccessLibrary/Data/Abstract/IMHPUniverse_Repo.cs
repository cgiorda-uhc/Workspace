

using VCPortal_Models.Models.MHP;
using VCPortal_Models.Parameters.MHP;

namespace DataAccessLibrary.Data.Abstract
{
    public interface IMHPUniverse_Repo
    {
        Task<IEnumerable<MHP_EI_Model>> GetMHP_EI_Async(List<string> strState, string strStartDate, string strEndDate, List<string> strFINC_ARNG_DESC, List<string> strMKT_SEG_RLLP_DESC, List<string> lstLegalEntities, List<string> strMKT_TYP_DESC, CancellationToken token);

        Task<IEnumerable<MHP_CS_Model>> GetMHP_CS_Async(List<string> strState, string strStartDate, string strEndDate, List<string> strCS_TADM_PRDCT_MAP, CancellationToken token);


        Task<IEnumerable<MHP_IFP_Model>> GetMHP_IFP_Async(List<string> strState, string strStartDate, string strEndDate, List<string> strFINC_ARNG_DESC, List<string> strMKT_SEG_RLLP_DESC, List<string> lstLegalEntities, List<string> strMKT_TYP_DESC, CancellationToken token);

        Task<IEnumerable<MPHUniverseDetails_Model>> GetMHPEIDetailsAsync(List<string> strState, string strStartDate, string strEndDate, List<string> strFINC_ARNG_DESC, List<string> strMKT_SEG_RLLP_DESC, List<string> lstLegalEntities, List<string> strMKT_TYP_DESC, CancellationToken token);


        Task<IEnumerable<MHP_Reporting_Filters>> GetMHP_Filters_Async(CancellationToken token);
    }
}