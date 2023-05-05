namespace ProjectManagerLibrary.Projects
{
    public interface IDataSourceVerification
    {
        Task<long> CheckDataSources();
    }
}