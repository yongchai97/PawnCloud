using Abp.Application.Services;
using PawnCloud.Authorization.Accounts.Dto;
using System.Threading.Tasks;

namespace PawnCloud.Authorization.Accounts;

public interface IAccountAppService : IApplicationService
{
    Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

    Task<RegisterOutput> Register(RegisterInput input);
}
