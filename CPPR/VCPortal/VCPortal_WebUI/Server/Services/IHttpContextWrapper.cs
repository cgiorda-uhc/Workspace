namespace VCPortal_WebUI.Server.Services;

public interface IHttpContextWrapper
{
    string GetValueFromRequestHeader(string key, string defaultValue);
}