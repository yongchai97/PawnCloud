using Abp.AspNetCore;
using Abp.AspNetCore.TestBase;
using Abp.Modules;
using Abp.Reflection.Extensions;
using PawnCloud.EntityFrameworkCore;
using PawnCloud.Web.Startup;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace PawnCloud.Web.Tests;

[DependsOn(
    typeof(PawnCloudWebMvcModule),
    typeof(AbpAspNetCoreTestBaseModule)
)]
public class PawnCloudWebTestModule : AbpModule
{
    public PawnCloudWebTestModule(PawnCloudEntityFrameworkModule abpProjectNameEntityFrameworkModule)
    {
        abpProjectNameEntityFrameworkModule.SkipDbContextRegistration = true;
    }

    public override void PreInitialize()
    {
        Configuration.UnitOfWork.IsTransactional = false; //EF Core InMemory DB does not support transactions.
    }

    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(typeof(PawnCloudWebTestModule).GetAssembly());
    }

    public override void PostInitialize()
    {
        IocManager.Resolve<ApplicationPartManager>()
            .AddApplicationPartsIfNotAddedBefore(typeof(PawnCloudWebMvcModule).Assembly);
    }
}