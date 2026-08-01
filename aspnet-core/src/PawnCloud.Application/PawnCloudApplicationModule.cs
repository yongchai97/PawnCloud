using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using PawnCloud.Authorization;

namespace PawnCloud;

[DependsOn(
    typeof(PawnCloudCoreModule),
    typeof(AbpAutoMapperModule))]
public class PawnCloudApplicationModule : AbpModule
{
    public override void PreInitialize()
    {
        Configuration.Authorization.Providers.Add<PawnCloudAuthorizationProvider>();
    }

    public override void Initialize()
    {
        var thisAssembly = typeof(PawnCloudApplicationModule).GetAssembly();

        IocManager.RegisterAssemblyByConvention(thisAssembly);

        Configuration.Modules.AbpAutoMapper().Configurators.Add(
            // Scan the assembly for classes which inherit from AutoMapper.Profile
            cfg => cfg.AddMaps(thisAssembly)
        );
    }
}
