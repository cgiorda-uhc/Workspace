

using VCPortal_Models.Models.MHP;
using VCPortal_Models.Parameters.MHP;

namespace DataAccessLibrary.Data.Abstract
{
    public interface IMHPUniverse_Repo
    {
        Task<IEnumerable<MHP_EI_Model>> GetMHP_EI_Async(string strState, string strStartDate, string strEndDate, string strFINC_ARNG_DESC, string strMKT_SEG_RLLP_DESC, List<string> lstLegalEntities, string strMKT_TYP_DESC, string strCUST_SEG, CancellationToken token);

        Task<IEnumerable<MHP_CS_Model>> GetMHP_CS_Async(string strState, string strStartDate, string strEndDate, string strCS_TADM_PRDCT_MAP, string strGroupNumbers, CancellationToken token);


        Task<IEnumerable<MHP_IFP_Model>> GetMHP_IFP_Async(string strState, string strStartDate, string strEndDate, List<string> lstProductCode, CancellationToken token);

        public Task<IEnumerable<MHPEIDetails_Model>> GetMHPEIDetailsAsync(string strState, string strStartDate, string strEndDate, string strFINC_ARNG_DESC, string strMKT_SEG_RLLP_DESC, List<string> lstLegalEntities, string strMKT_TYP_DESC, string strCUST_SEG, CancellationToken token);


        Task<IEnumerable<MHPIFPDetails_Model>> GetMHPIFPDetailsAsync(string strState, string strStartDate, string strEndDate, List<string> lstProductCode, CancellationToken token);

        Task<IEnumerable<MHPCSDetails_Model>> GetMHPCSDetailsAsync(string strState, string strStartDate, string strEndDate, string strCS_TADM_PRDCT_MAP, string strGroupNumbers, CancellationToken token);

        Task<IEnumerable<MHP_Reporting_Filters>> GetMHP_Filters_Async(CancellationToken token);

        Task<IEnumerable<MHP_Group_State_Model>> GetMHP_Group_State_Async(CancellationToken token);
    }
}