using Abp.Events.Bus;
using Abp.Modules;
using Abp.Reflection.Extensions;
using PawnCloud.Configuration;
using PawnCloud.EntityFrameworkCore;
using PawnCloud.Migrator.DependencyInjection;
using Castle.MicroKernel.Registration;
using Microsoft.Extensions.Configuration;

namespace PawnCloud.Migrator;

[DependsOn(typeof(PawnCloudEntityFrameworkModule))]
public class PawnCloudMigratorModule : AbpModule
{
    private readonly IConfigurationRoot _appConfiguration;

    public PawnCloudMigratorModule(PawnCloudEntityFrameworkModule abpProjectNameEntityFrameworkModule)
    {
        abpProjectNameEntityFrameworkModule.SkipDbSeed = true;

        _appConfiguration = AppConfigurations.Get(
            typeof(PawnCloudMigratorModule).GetAssembly().GetDirectoryPathOrNull()
        );
    }

    public override void PreInitialize()
    {
        Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(
            PawnCloudConsts.ConnectionStringName
        );

        Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
        Configuration.ReplaceService(
            typeof(IEventBus),
            () => IocManager.IocContainer.Register(
                Component.For<IEventBus>().Instance(NullEventBus.Instance)
            )
        );
    }

    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(typeof(PawnCloudMigratorModule).GetAssembly());
        ServiceCollectionRegistrar.Register(IocManager);
    }
}
