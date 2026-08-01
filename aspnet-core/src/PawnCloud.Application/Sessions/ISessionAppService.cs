using Abp.Application.Services;
using PawnCloud.Sessions.Dto;
using System.Threading.Tasks;

namespace PawnCloud.Sessions;

public interface ISessionAppService : IApplicationService
{
    Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
}
