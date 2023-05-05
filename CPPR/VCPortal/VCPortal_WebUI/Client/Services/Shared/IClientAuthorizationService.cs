using VCPortal_Models.Models.ActiveDirectory;

namespace VCPortal_WebUI.Client.Services.Shared;

public interface IClientAuthorizationService
{
    string UserName { get; set; }
    UserAccessModel CurrentUser { get; set; }
    Task<UserAccessModel> GetCurrentUserAsync(string username);
}