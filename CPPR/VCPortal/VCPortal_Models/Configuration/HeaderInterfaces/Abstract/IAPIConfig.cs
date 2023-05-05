namespace VCPortal_Models.Configuration.HeaderInterfaces.Concrete;

public interface IAPIConfig
{
    string BaseUrl { get; set; }
    string Name { get; set; }
    string Display { get; set; }
    string Url { get; set; }
}