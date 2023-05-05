namespace ProjectManagerLibrary.Projects
{
    public interface IPBIMembership
    {
        Task<long> RefreshTable();
    }
}