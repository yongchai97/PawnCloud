using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using PawnCloud.Authorization;
using PawnCloud.PawnItems.Dto;

namespace PawnCloud.PawnItems;

[AbpAuthorize(PermissionNames.Pages_PawnItems)]
public class PawnItemAppService : ApplicationService, IPawnItemAppService
{
    private readonly IRepository<PawnItem, int> _repository;

    public PawnItemAppService(IRepository<PawnItem, int> repository)
    {
        _repository = repository;
    }

    public async Task<PagedResultDto<PawnItemDto>> GetAll(PagedPawnItemResultRequestDto input)
    {
        await PermissionChecker.AuthorizeAsync(PermissionNames.Pages_PawnItems);

        var query = _repository.GetAll()
            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                p => p.Category.Contains(input.Filter) || p.Description.Contains(input.Filter));

        var totalCount = await query.CountAsync();

        var entities = await query.OrderByDescending(p => p.CreationTime).PageBy(input).ToListAsync();

        var dtos = ObjectMapper.Map<List<PawnItemDto>>(entities);

        return new PagedResultDto<PawnItemDto>(totalCount, dtos);
    }

    public async Task<GetPawnItemForEditOutput> GetViaIdForEdit(EntityDto<int> input)
    {
        await PermissionChecker.AuthorizeAsync(PermissionNames.Pages_PawnItems);
        var entity = await _repository.GetAsync(input.Id);
        var dto = ObjectMapper.Map<CreateOrEditPawnItemDto>(entity);
        return new GetPawnItemForEditOutput { PawnItem = dto };
    }

    public async Task<int> CreateOrEdit(CreateOrEditPawnItemDto input)
    {
        if (input.Id > 0)
        {
            await PermissionChecker.AuthorizeAsync(PermissionNames.Pages_PawnItems_Edit);
            var entity = await _repository.GetAsync(input.Id);
            ObjectMapper.Map(input, entity);
            await _repository.UpdateAsync(entity);
            return entity.Id;
        }
        else
        {
            await PermissionChecker.AuthorizeAsync(PermissionNames.Pages_PawnItems_Create);
            var entity = ObjectMapper.Map<PawnItem>(input);
            entity.TenantId = AbpSession.TenantId;
            await _repository.InsertAndGetIdAsync(entity);
            return entity.Id;
        }
    }

    public async Task Delete(EntityDto<int> input)
    {
        await PermissionChecker.AuthorizeAsync(PermissionNames.Pages_PawnItems_Delete);
        await _repository.DeleteAsync(input.Id);
    }

    public async Task<ListResultDto<PawnItemDto>> GetByPawnTicketId(EntityDto<int> input)
    {
        await PermissionChecker.AuthorizeAsync(PermissionNames.Pages_PawnItems);
        var items = await _repository.GetAllListAsync(p => p.PawnTicketId == input.Id);
        var dtos = ObjectMapper.Map<List<PawnItemDto>>(items);
        return new ListResultDto<PawnItemDto>(dtos);
    }
}