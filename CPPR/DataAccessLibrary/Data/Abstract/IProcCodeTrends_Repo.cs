using VCPortal_Models.Models.ProcCodeTrends;
using VCPortal_Models.Parameters.ProcCodeTrends;

namespace DataAccessLibrary.Data.Abstract
{
    public interface IProcCodeTrends_Repo
    {
        Task<IEnumerable<MM_FINAL_Model>> GetMM_FINAL_Async(CancellationToken token);

        Task<IEnumerable<CLM_PHYS_Model>> GetCLM_PHYS_Async(ProcCodeTrends_Parameters pct_param, CancellationToken token);


        Task<IEnumerable<MM_FINAL_Model>> GetCLM_OP_Async(string LOB, string Region, string State, string Product, string CSProduct, string FundingType, string LegalEntity, string Source, string CSDualIndicator, string MRDualIndicator, CancellationToken token);
    }
}