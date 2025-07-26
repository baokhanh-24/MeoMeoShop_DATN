using MeoMeo.Shared.Constants;
using Microsoft.AspNetCore.Authorization;

namespace MeoMeo.Shared.Utilities;

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var userPermissions = context.User.FindFirst(c => c.Type == ClaimTypeConst.Permissions)?.Value;

        if (userPermissions != null && !requirement.RequiresAll )
        {
            if (userPermissions.Split(';').Intersect(requirement.PermissionNames).Any())
            {
                context.Succeed(requirement);
            }
        } 
        else if (userPermissions != null && requirement.RequiresAll)
        {
            if (requirement.PermissionNames.All(p => userPermissions.Split(';').Contains(p)))
            {
                context.Succeed(requirement); 
            }
        }
        else
        {
            context.Fail(); 
        }

        return Task.CompletedTask;
    }
}

