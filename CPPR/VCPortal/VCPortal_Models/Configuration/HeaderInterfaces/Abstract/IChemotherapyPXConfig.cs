using VCPortal_Models.Configuration.HeaderInterfaces.Concrete;

namespace VCPortal_Models.Configuration.HeaderInterfaces.Abstract;

public interface IChemotherapyPXConfig
{
    List<APIConfig> APIS { get; set; }
    string Name { get; set; }
}