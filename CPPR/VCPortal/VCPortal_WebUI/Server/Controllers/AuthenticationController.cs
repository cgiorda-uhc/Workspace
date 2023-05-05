using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VCPortal_WebUI.Client.Services.Shared;
using VCPortal_WebUI.Server.Services;

namespace VCPortal_WebUI.Server.Controllers;
public class AuthenticationController : Controller
{
    //private readonly IHttpContextWrapper _iHttpContextWrapper;
    //private readonly IClientAuthorizationService _iClientAuthorizationService;


    //public AuthenticationController(IHttpContextWrapper iHttpContextWrapper, IClientAuthorizationService iClientAuthorizationService)
    //{
    //    _iHttpContextWrapper = iHttpContextWrapper;
    //    _iClientAuthorizationService = iClientAuthorizationService;
    //}

    //// GET: AuthenticationController
    //public ActionResult Index()
    //{
    //    var id = _iHttpContextWrapper.GetValueFromRequestHeader("Authorization", "default");

    //    _iClientAuthorizationService.UserName = id;

    //    return View();
    //}

}
