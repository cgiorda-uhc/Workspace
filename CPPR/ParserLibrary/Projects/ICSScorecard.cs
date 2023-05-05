namespace ProjectManagerLibrary.Projects
{
    public interface ICSScorecard
    {
        Task<long> LoadCSScorecardData();
    }
}