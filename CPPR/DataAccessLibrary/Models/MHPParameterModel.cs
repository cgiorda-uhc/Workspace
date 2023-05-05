

using DataAccessLibrary.Shared;

namespace DataAccessLibrary.Models;

public class MHPParameterModel
{
    public string MHPSQL { get; set; }
    public string UGAPSQL { get; set; }
    public string SearchMethod { get; set; }
    public LOS LOS { get; set; }
}


