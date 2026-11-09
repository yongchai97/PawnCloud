using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Auditing;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using PawnCloud.AuditLogs.Dto;
using PawnCloud.Authorization;
using PawnCloud.Authorization.Users;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PawnCloud.AuditLogs;

[AbpAuthorize(PermissionNames.Pages_AuditLogs)]
public class AuditLogAppService : ApplicationService
{
    private readonly IRepository<AuditLog, long> _repository;
    private readonly IRepository<User, long> _userRepository;

    public AuditLogAppService(IRepository<AuditLog, long> repository, IRepository<User, long> userRepository)
    {
        _repository = repository;
        _userRepository = userRepository;
    }

    [DisableAuditing]
    public async Task<PagedResultDto<AuditLogDto>> GetAll(PagedAuditLogResultRequestDto input)
    {
        await PermissionChecker.AuthorizeAsync(PermissionNames.Pages_AuditLogs);

        if (AbpSession.TenantId.HasValue)
        {
            return await QueryAuditLogs(input);
        }

        // Hosts can review audit activity across all tenants; tenant users stay within ABP's tenant filter.
        using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
        {
            return await QueryAuditLogs(input);
        }
    }

    private async Task<PagedResultDto<AuditLogDto>> QueryAuditLogs(PagedAuditLogResultRequestDto input)
    {
        var query = _repository.GetAll()
            .Where(log => log.UserId.HasValue)
            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), log =>
                (log.ServiceName != null && log.ServiceName.Contains(input.Filter)) ||
                (log.MethodName != null && log.MethodName.Contains(input.Filter)) ||
                (log.ClientName != null && log.ClientName.Contains(input.Filter)) ||
                (log.ClientIpAddress != null && log.ClientIpAddress.Contains(input.Filter)) ||
                (log.ExceptionMessage != null && log.ExceptionMessage.Contains(input.Filter)));

        var totalCount = await query.CountAsync();
        var logs = await query
            .OrderByDescending(log => log.ExecutionTime)
            .PageBy(input)
            .ToListAsync();

        var userIds = logs
            .Where(log => log.UserId.HasValue)
            .Select(log => log.UserId.Value)
            .Distinct()
            .ToList();
        var userNames = await _userRepository.GetAll()
            .IgnoreQueryFilters()
            .Where(user => userIds.Contains(user.Id))
            .ToDictionaryAsync(user => (user.TenantId, user.Id), user => user.UserName);

        var items = logs.Select(log => new AuditLogDto
        {
            Id = log.Id,
            TenantId = log.TenantId,
            UserId = log.UserId,
            UserName = log.UserId.HasValue && userNames.TryGetValue((log.TenantId, log.UserId.Value), out var userName)
                ? userName
                : null,
            ServiceName = log.ServiceName,
            MethodName = log.MethodName,
            Parameters = log.Parameters,
            ReturnValue = log.ReturnValue,
            ExecutionTime = log.ExecutionTime,
            ExecutionDuration = log.ExecutionDuration,
            ClientIpAddress = log.ClientIpAddress,
            ClientName = log.ClientName,
            BrowserInfo = log.BrowserInfo,
            ExceptionMessage = log.ExceptionMessage,
        }).ToList();

        return new PagedResultDto<AuditLogDto>(totalCount, items);
    }
}
