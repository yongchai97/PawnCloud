using Abp.Modules;
using Abp.Reflection.Extensions;
using PawnCloud.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace PawnCloud.Web.Host.Startup
{
    [DependsOn(
       typeof(PawnCloudWebCoreModule))]
    public class PawnCloudWebHostModule : AbpModule
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public PawnCloudWebHostModule(IWebHostEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(PawnCloudWebHostModule).GetAssembly());
        }
    }
}
