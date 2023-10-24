using VCPortal_Models.Models.ProcCodeTrends;

namespace DataAccessLibrary.Data.Abstract
{
    public interface IProcCodeTrends_Repo
    {
        Task<IEnumerable<MM_FINAL_Model>> GetMM_FINAL_Async(CancellationToken token);
    }
}