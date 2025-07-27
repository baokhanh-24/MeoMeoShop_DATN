using Microsoft.AspNetCore.Authorization;

namespace MeoMeo.Shared.Utilities;

public class PermissionRequirement : IAuthorizationRequirement
{
    public string[] PermissionNames { get; }

    public bool RequiresAll { get; }

    public PermissionRequirement( string[] permissionNames, bool requiresAll)
    {
        PermissionNames = permissionNames;
        RequiresAll = requiresAll;
    }

    public override string ToString()
    {
        return $"PermissionsRequirement: {string.Join(", ", PermissionNames)}";
    }
}
