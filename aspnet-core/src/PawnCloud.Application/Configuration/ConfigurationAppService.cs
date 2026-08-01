using Abp.Authorization;
using Abp.Runtime.Session;
using PawnCloud.Configuration.Dto;
using System.Threading.Tasks;

namespace PawnCloud.Configuration;

[AbpAuthorize]
public class ConfigurationAppService : PawnCloudAppServiceBase, IConfigurationAppService
{
    public async Task ChangeUiTheme(ChangeUiThemeInput input)
    {
        await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
    }
}
