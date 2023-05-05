using VCPortal_Models.Dtos.ETGFactSymmetry;

namespace DataAccessLibrary.Data.Abstract
{
    public interface IMHPData_Repo
    {
        Task<IEnumerable<string>> GetStatesAsync(CancellationToken token);
    }
}