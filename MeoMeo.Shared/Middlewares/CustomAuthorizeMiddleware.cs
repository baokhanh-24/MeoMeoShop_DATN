using System.Net;
using MeoMeo.Shared.Attributes;
using MeoMeo.Shared.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace MeoMeo.Shared.Middlewares;

public class CustomAuthorizeMiddleware
{
    private RequestDelegate Next { get; }

    private AppSettings AppSettings { get; }
    public CustomAuthorizeMiddleware(RequestDelegate next, IOptions<AppSettings> options)
    {
        Next = next;

        AppSettings = options.Value;
    }
    public async Task Invoke(HttpContext httpContext)
    {
        Endpoint endpoint = httpContext.Features.Get<IEndpointFeature>()?.Endpoint;

        CustomAuthorizeAttribute attribute = endpoint?.Metadata.GetMetadata<CustomAuthorizeAttribute>();

        if (attribute != null)
        {
            try
            {
                bool isValidToken = httpContext.Request.Headers.TryGetValue(HeaderNames.Authorization, out var tokenString);

                string token = tokenString;

                if (!isValidToken || string.IsNullOrEmpty(token))
                {

                    httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

                    return;
                }
            }
            catch (Exception ex)
            {

                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                return;
            }
        }

        await Next(httpContext);
    }
}