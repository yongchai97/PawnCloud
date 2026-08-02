using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Threading.Tasks;
using PawnCloud.Misc;

namespace PawnCloud.Misc;

public interface IMiscFunctionAppService : IApplicationService
{
    Task<ListResultDto<StatusDto>> GetStatuses();
}
