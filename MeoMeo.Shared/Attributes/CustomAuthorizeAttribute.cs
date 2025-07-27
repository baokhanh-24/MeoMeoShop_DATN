using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using MeoMeo.Domain.Commons.Enums;
using MeoMeo.Shared.Constants;
using MeoMeo.Shared.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MeoMeo.Shared.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class CustomAuthorizeAttribute(EFunctionCode functionCode, ECommandCode commandCode) : Attribute, IAuthorizationFilter
{
    private readonly ECommandCode _commandCode = commandCode;
    private readonly EFunctionCode _functionCode = functionCode;

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        string token = context.HttpContext.Request.Headers.Authorization.FirstOrDefault().Split(" ")[1];
        var handler = new JwtSecurityTokenHandler();
        JwtSecurityToken jwtSecurityToken = handler.ReadJwtToken(token);
        System.Security.Claims.Claim permissionsClaim = jwtSecurityToken.Claims.First(claim => claim.Type == ClaimTypeConst.Permissions);
        if (permissionsClaim != null)
        {
            List<string> permissions = JsonSerializer.Deserialize<List<string>>(permissionsClaim.Value);
            if (!permissions.Contains(FunctionHelper.PermissionAuthorize.GetPermission(_functionCode, _commandCode)))
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
}