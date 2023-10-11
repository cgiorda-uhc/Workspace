using VCPortal_Models.Parameters.MHP;

namespace DataAccessLibrary.Data.Abstract
{
    public interface IEDCAdhoc_Repo
    {
        Task<IEnumerable<MHP_Group_State_Model>> GetMHP_Group_State_Async(CancellationToken token);
    }
}