using Abp.BlobStoring;
using Abp.BlobStoring.Azure;
using Abp.BlobStoring.FileSystem;
using Abp.Localization;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Runtime.Security;
using Abp.Timing;
using Abp.Zero;
using Abp.Zero.Configuration;
using PawnCloud.Authorization.Roles;
using PawnCloud.Authorization.Users;
using PawnCloud.Configuration;
using PawnCloud.Localization;
using PawnCloud.MultiTenancy;
using PawnCloud.Timing;
using System;

namespace PawnCloud;

[DependsOn(typeof(AbpZeroCoreModule), typeof(AbpBlobStoringModule),
    typeof(AbpBlobStoringFileSystemModule),
    typeof(AbpBlobStoringAzureModule))]
public class PawnCloudCoreModule : AbpModule
{
    
    public override void PreInitialize()
    {
        Configuration.Auditing.IsEnabledForAnonymousUsers = true;

        // Declare entity types
        Configuration.Modules.Zero().EntityTypes.Tenant = typeof(Tenant);
        Configuration.Modules.Zero().EntityTypes.Role = typeof(Role);
        Configuration.Modules.Zero().EntityTypes.User = typeof(User);

        PawnCloudLocalizationConfigurer.Configure(Configuration.Localization);

        // Enable this line to create a multi-tenant application.
        Configuration.MultiTenancy.IsEnabled = PawnCloudConsts.MultiTenancyEnabled;

        // Configure roles
        AppRoleConfig.Configure(Configuration.Modules.Zero().RoleManagement);

        Configuration.Settings.Providers.Add<AppSettingProvider>();

        Configuration.Localization.Languages.Add(new LanguageInfo("fa", "فارسی", "famfamfam-flags ir"));

        Configuration.Settings.SettingEncryptionConfiguration.DefaultPassPhrase = PawnCloudConsts.DefaultPassPhrase;
        SimpleStringCipher.DefaultPassPhrase = PawnCloudConsts.DefaultPassPhrase;
        ConfigureBlobStorage();
    }

    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(typeof(PawnCloudCoreModule).GetAssembly());
    }

    public override void PostInitialize()
    {
        IocManager.Resolve<AppTimes>().StartupTime = Clock.Now;
    }
    private void ConfigureBlobStorage()
    {
        var environmentName =
    Environment.GetEnvironmentVariable(
        "ASPNETCORE_ENVIRONMENT");

        var configuration =
            AppConfigurations.Get(
                AppContext.BaseDirectory,
                environmentName);

        var provider =
            configuration["BlobStorage:Provider"];

        if (string.Equals(
            provider,
            "FileSystem",
            StringComparison.OrdinalIgnoreCase))
        {
            var basePath =
                configuration[
                    "BlobStorage:FileSystem:BasePath"];

            Configuration.Modules
                .AbpBlobStoring()
                .Containers
                .ConfigureDefault(container =>
                {
                    container.UseFileSystem(fileSystem =>
                    {
                        fileSystem.BasePath = basePath;
                    });
                });

            return;
        }

        if (string.Equals(
            provider,
            "Azure",
            StringComparison.OrdinalIgnoreCase))
        {
            var connectionString =
                configuration[
                    "BlobStorage:Azure:ConnectionString"];

            var containerName =
                configuration[
                    "BlobStorage:Azure:ContainerName"];

            Configuration.Modules
                .AbpBlobStoring()
                .Containers
                .ConfigureDefault(container =>
                {
                    container.UseAzure(azure =>
                    {
                        azure.ConnectionString =
                            connectionString;

                        azure.ContainerName =
                            containerName;

                        azure.CreateContainerIfNotExists =
                            true;
                    });
                });

            return;
        }

        throw new InvalidOperationException(
            $"Unsupported BlobStorage provider: '{provider}'. " +
            "Supported providers are 'FileSystem' and 'Azure'.");
    }
}
