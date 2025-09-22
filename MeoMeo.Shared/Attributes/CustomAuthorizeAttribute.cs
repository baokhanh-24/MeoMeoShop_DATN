using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MeoMeo.Shared.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class CustomAuthorizeAttribute(params string[] roles) : Attribute, IAuthorizationFilter
{
    private readonly string[] _roles = roles;

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        try
        {
            string token = context.HttpContext.Request.Headers.Authorization.FirstOrDefault()?.Split(" ")[1];
            if (string.IsNullOrEmpty(token))
            {
                context.Result = new UnauthorizedObjectResult(string.Empty);
                return;
            }

            var handler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtSecurityToken = handler.ReadJwtToken(token);

            var rolesClaim = jwtSecurityToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role || claim.Type == "Roles");
            if (rolesClaim != null)
            {
                string[] userRoles = rolesClaim.Value.Split(';');
                if (!_roles.Any(requiredRole => userRoles.Contains(requiredRole)))
                {
                    context.Result = new UnauthorizedObjectResult(string.Empty);
                    return;
                }
            }
            else
            {
                context.Result = new UnauthorizedObjectResult(string.Empty);
                return;
            }
        }
        catch (Exception)
        {
            context.Result = new UnauthorizedObjectResult(string.Empty);
        }
    }
}