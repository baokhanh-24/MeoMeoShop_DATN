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
    public static int GetUserIdOfUserLogin(this HttpContext httpContext)
    {
        bool isValidToken = httpContext.Request.Headers.TryGetValue(HeaderNames.Authorization, out var tokenString);
        if (!isValidToken || string.IsNullOrEmpty(tokenString.ToString()))
            return default;
        var jwtEncodedString = tokenString.ToString()[7..];
        var token = new JwtSecurityToken(jwtEncodedString);
        var claim = token.Claims.FirstOrDefault(x => x.Type == "id");
        int UserId = Convert.ToInt32( claim?.Value);
        return UserId;
    }
}
