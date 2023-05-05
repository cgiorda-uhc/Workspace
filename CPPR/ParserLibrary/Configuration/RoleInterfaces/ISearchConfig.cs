

namespace ProjectManagerLibrary.Configuration.RoleInterfaces;

public interface ISearchConfig
{
    public string SearchString { get; set; }
    public SearchType SearchType { get; set; }
}
