using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Linq;

namespace VCPortal_WebUI.Server.Services;


//https://www.code4it.dev/blog/inject-httpcontext
public class HttpContextWrapper : IHttpContextWrapper
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextWrapper(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetValueFromRequestHeader(string key, string defaultValue)
    {
        if (_httpContextAccessor.HttpContext.Request.Headers.TryGetValue(key, out StringValues headerValues) && headerValues.Any())
        {
            return headerValues.First();
        }

        return defaultValue;
    }
}
