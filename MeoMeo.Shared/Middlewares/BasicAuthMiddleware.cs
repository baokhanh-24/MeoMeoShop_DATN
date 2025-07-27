using System.IdentityModel.Tokens.Jwt;
using System.Text;
using MeoMeo.Domain.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace MeoMeo.Shared.Middlewares;

public class BasicAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _username = "usersw"; 
    private readonly string _password = "Qtrrsw453"; 

    public BasicAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
    {
        try
        {
            bool isValidToken = context.Request.Headers.TryGetValue(HeaderNames.Authorization, out var tokenString);
            if (isValidToken && !string.IsNullOrEmpty(tokenString.ToString()) && tokenString.ToString() != "Bearer null")
            {
                try
                {
                    // var jwtEncodedString = tokenString.ToString()[7..];
                    // var token = new JwtSecurityToken(jwtEncodedString);
                    // var claim = token.Claims.FirstOrDefault(x => x.Type == "id");
                    // Guid UserId = Guid.Parse(claim.Value);
                    // var userRepository = serviceProvider.GetService(typeof(IUserRepository)) as IUserRepository;
                    // var userInfo = await userRepository.GetUserByIdAsync(UserId);
                    // if (userInfo != null)
                    // {
                    //     context.Response.Headers["WWW-Authenticate"] = "Basic";
                    //     context.Response.StatusCode = StatusCodes.Status409Conflict;
                    //     return;
                    // }
                }
                catch (Exception ex)
                { }
            }
            if (context.Request.Path.StartsWithSegments("/swagger"))
            {
                string authHeader = context.Request.Headers["Authorization"];

                if (authHeader != null && authHeader.StartsWith("Basic "))
                {
                    var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                    var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                    var usernamePassword = decodedUsernamePassword.Split(':');

                    if (usernamePassword[0] == _username && usernamePassword[1] == _password)
                    {
                        await _next(context);
                        return;
                    }
                }

                context.Response.Headers["WWW-Authenticate"] = "Basic";
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }
            else
            {
                if (_next == null)
                {

                }
            }

            await _next(context);
        }
        catch (Exception ex)
        {

        }
    }
}
