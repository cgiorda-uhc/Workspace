namespace ProjectManagerLibrary.Projects
{
    public interface ISnowflakeDashboardData
    {
        Task<long> SnowflakeDashboardDataRefresh();
    }
}