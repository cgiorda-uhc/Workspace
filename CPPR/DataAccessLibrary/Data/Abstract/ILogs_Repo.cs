using VCPortal_Models.Models.Shared;

namespace DataAccessLibrary.Data.Abstract
{
    public interface ILog_Repo
    {
        Task InsertLog(VCLog log);
    }
}