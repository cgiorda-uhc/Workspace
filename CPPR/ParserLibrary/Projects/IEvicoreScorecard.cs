namespace ProjectManagerLibrary.Projects
{
    public interface IEvicoreScorecard
    {
        Task<long> LoadEvicoreScorecardData();
    }
}