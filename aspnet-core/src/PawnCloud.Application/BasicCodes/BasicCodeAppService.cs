using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PawnCloud.Authorization;
using PawnCloud.BasicCodes.Dto;

namespace PawnCloud.BasicCodes;

[AbpAuthorize(PermissionNames.Pages_BasicCodes)]
public class BasicCodeAppService : ApplicationService, IBasicCodeAppService
{
    private readonly IRepository<BasicCode, int> _repository;

    public BasicCodeAppService(IRepository<BasicCode, int> repository)
    {
        _repository = repository;
    }

    public async Task<PagedResultDto<BasicCodeDto>> GetAll(PagedBasicCodeResultRequestDto input)
    {
        await PermissionChecker.AuthorizeAsync(PermissionNames.Pages_BasicCodes);

        var query = _repository.GetAll()
            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                b => b.codeName.Contains(input.Filter) || b.codeDescription.Contains(input.Filter));

        var totalCount = await query.CountAsync();

        var entities = await query.OrderByDescending(b => b.CreationTime).PageBy(input).ToListAsync();

        var dtos = ObjectMapper.Map<List<BasicCodeDto>>(entities);

        return new PagedResultDto<BasicCodeDto>(totalCount, dtos);
    }

    public async Task<GetBasicCodeForEditOutput> GetViaIdForEdit(EntityDto<int> input)
    {
        await PermissionChecker.AuthorizeAsync(PermissionNames.Pages_BasicCodes);
        var entity = await _repository.GetAsync(input.Id);
        var dto = ObjectMapper.Map<CreateOrEditBasicCodeDto>(entity);
        return new GetBasicCodeForEditOutput { BasicCode = dto };
    }

    public async Task<int> CreateOrEdit(CreateOrEditBasicCodeDto input)
    {
        if (input.Id > 0)
        {
            await PermissionChecker.AuthorizeAsync(PermissionNames.Pages_BasicCodes_Edit);
            var entity = await _repository.GetAsync(input.Id);
            if (entity.systemProvidedValue)
            {
                throw new UserFriendlyException("System-provided basic codes cannot be edited.");
            }

            input.SystemProvidedValue = false;
            ObjectMapper.Map(input, entity);
            await _repository.UpdateAsync(entity);
            return entity.Id;
        }
        else
        {
            await PermissionChecker.AuthorizeAsync(PermissionNames.Pages_BasicCodes_Create);
            var entity = ObjectMapper.Map<BasicCode>(input);
            entity.systemProvidedValue = false;
            entity.TenantId = AbpSession.TenantId;
            await _repository.InsertAndGetIdAsync(entity);
            return entity.Id;
        }
    }

    public async Task Delete(EntityDto<int> input)
    {
        await PermissionChecker.AuthorizeAsync(PermissionNames.Pages_BasicCodes_Delete);
        var entity = await _repository.GetAsync(input.Id);
        if (entity.systemProvidedValue)
        {
            throw new UserFriendlyException("System-provided basic codes cannot be deleted.");
        }

        await _repository.DeleteAsync(input.Id);
    }

    public async Task<ListResultDto<BasicCodeLookupDto>> GetBasicCodesForLookup()
    {
        var basicCodes = await _repository.GetAllListAsync();
        var lookup = basicCodes
            .OrderBy(b => b.codeName)
            .Select(b => new BasicCodeLookupDto
            {
                Id = b.Id,
                DisplayName = b.codeName + " - " + b.codeDescription
            })
            .ToList();
        return new ListResultDto<BasicCodeLookupDto>(lookup);
    }

    public async Task<ListResultDto<BasicCodeLookupDto>> GetBasicCodesByCategory(int? categoryId)
    {
        var basicCodes = await _repository.GetAllListAsync();
        var lookup = basicCodes
            .Where(b => b.MiscMasterConfig == categoryId)
            .OrderBy(b => b.codeName)
            .Select(b => new BasicCodeLookupDto
            {
                Id = b.Id,
                DisplayName = b.codeName + " - " + b.codeDescription
            })
            .ToList();
        return new ListResultDto<BasicCodeLookupDto>(lookup);
    }
}
