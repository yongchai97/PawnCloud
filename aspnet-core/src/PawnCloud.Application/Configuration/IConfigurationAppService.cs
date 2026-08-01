using PawnCloud.Configuration.Dto;
using System.Threading.Tasks;

namespace PawnCloud.Configuration;

public interface IConfigurationAppService
{
    Task ChangeUiTheme(ChangeUiThemeInput input);
}
