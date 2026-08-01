using Abp.Application.Services;
using PawnCloud.MultiTenancy.Dto;

namespace PawnCloud.MultiTenancy;

public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
{
}

