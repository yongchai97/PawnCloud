using Abp.Authorization;
using PawnCloud.Authorization.Roles;
using PawnCloud.Authorization.Users;

namespace PawnCloud.Authorization;

public class PermissionChecker : PermissionChecker<Role, User>
{
    public PermissionChecker(UserManager userManager)
        : base(userManager)
    {
    }
}
