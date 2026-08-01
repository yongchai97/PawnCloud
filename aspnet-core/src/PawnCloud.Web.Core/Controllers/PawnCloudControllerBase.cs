using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace PawnCloud.Controllers
{
    public abstract class PawnCloudControllerBase : AbpController
    {
        protected PawnCloudControllerBase()
        {
            LocalizationSourceName = PawnCloudConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
