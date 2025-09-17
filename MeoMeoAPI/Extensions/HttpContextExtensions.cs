using System.IdentityModel.Tokens.Jwt;
using Microsoft.Net.Http.Headers;

namespace MeoMeo.API.Extensions;

public static class HttpContextExtensions
{
    public static string GetEmailOfUserLogin(this HttpContext httpContext)
    {
        bool isValidToken = httpContext.Request.Headers.TryGetValue(HeaderNames.Authorization, out var tokenString);
        if (!isValidToken || string.IsNullOrEmpty(tokenString.ToString()))
            return default;
        var jwtEncodedString = tokenString.ToString()[7..];
        var token = new JwtSecurityToken(jwtEncodedString);
        var claim = token.Claims.FirstOrDefault(x => x.Type == "email");
        string email = claim?.Value;
        return email;
    }
    public static Guid GetCurrentUserId(this HttpContext httpContext)
    {
        bool isValidToken = httpContext.Request.Headers.TryGetValue(HeaderNames.Authorization, out var tokenString);
        if (!isValidToken || string.IsNullOrEmpty(tokenString.ToString()))
            return  Guid.Empty;
        var jwtEncodedString = tokenString.ToString()[7..];
        var token = new JwtSecurityToken(jwtEncodedString);
        var claim = token.Claims.FirstOrDefault(x => x.Type == "id");
        var UserId = claim?.Value != null ? Guid.Parse(claim.Value) : Guid.Empty;
        return UserId;
    }  
    public static Guid GetCurrentCustomerId(this HttpContext httpContext)
    {
        bool isValidToken = httpContext.Request.Headers.TryGetValue(HeaderNames.Authorization, out var tokenString);
        if (!isValidToken || string.IsNullOrEmpty(tokenString.ToString()))
            return  Guid.Empty;
        var jwtEncodedString = tokenString.ToString()[7..];
        var token = new JwtSecurityToken(jwtEncodedString);
        var claim = token.Claims.FirstOrDefault(x => x.Type == "customerId");
        var customerId = claim?.Value != null ? Guid.Parse(claim.Value) : Guid.Empty;
        return customerId;
    }  
    public static Guid GetCurrentEmployeeId(this HttpContext httpContext)
    {
        bool isValidToken = httpContext.Request.Headers.TryGetValue(HeaderNames.Authorization, out var tokenString);
        if (!isValidToken || string.IsNullOrEmpty(tokenString.ToString()))
            return  Guid.Empty;
        var jwtEncodedString = tokenString.ToString()[7..];
        var token = new JwtSecurityToken(jwtEncodedString);
        var claim = token.Claims.FirstOrDefault(x => x.Type == "employeeId");
        var employeeId = claim?.Value != null ? Guid.Parse(claim.Value) : Guid.Empty;
        return employeeId;
    }
}
