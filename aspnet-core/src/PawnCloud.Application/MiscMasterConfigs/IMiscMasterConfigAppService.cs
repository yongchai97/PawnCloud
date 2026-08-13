using Abp.Application.Services;
using Abp.Application.Services.Dto;
using PawnCloud.MiscMasterConfigs.Dto;
using System.Threading.Tasks;

namespace PawnCloud.MiscMasterConfigs;

public interface IMiscMasterConfigAppService : IApplicationService
{
    Task<ListResultDto<MiscMasterConfigLookupDto>> GetForLookup();
}
