using VCPortal_Models.Configuration.HeaderInterfaces.Concrete;

namespace VCPortal_Models.Configuration.HeaderInterfaces.Abstract;

public interface IProcCodeTrendConfig
{
    List<APIConfig> APIS { get; set; }
    List<CommentsConfig> Comments { get; set; }
    string Name { get; set; }
}