using Abp.Configuration.Startup;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Xml;
using Abp.Reflection.Extensions;

namespace PawnCloud.Localization;

public static class PawnCloudLocalizationConfigurer
{
    public static void Configure(ILocalizationConfiguration localizationConfiguration)
    {
        localizationConfiguration.Sources.Add(
            new DictionaryBasedLocalizationSource(PawnCloudConsts.LocalizationSourceName,
                new XmlEmbeddedFileLocalizationDictionaryProvider(
                    typeof(PawnCloudLocalizationConfigurer).GetAssembly(),
                    "PawnCloud.Localization.SourceFiles"
                )
            )
        );
    }
}
