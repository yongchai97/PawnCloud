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
using PawnCloud.PawnTickets.Dto;

namespace PawnCloud.PawnTickets;

[AbpAuthorize(PermissionNames.Pages_PawnTickets)]
public class PawnTicketAppService : ApplicationService, IPawnTicketAppService
{
    private readonly IRepository<PawnTicket, int> _repository;

    public PawnTicketAppService(
        IRepository<PawnTicket, int> repository)
    {
        _repository = repository;
    }

    public async Task<PagedResultDto<PawnTicketDto>> GetAll(PagedPawnTicketResultRequestDto input)
    {
        //await PermissionChecker.AuthorizeAsync(PermissionNames.Pages_PawnTickets);

        var query = _repository.GetAll()
            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                p => p.TicketNo.Contains(input.Filter));

        var totalCount = await query.CountAsync();

        var entities = await query.OrderByDescending(p => p.CreationTime).PageBy(input).ToListAsync();

        var dtos = ObjectMapper.Map<List<PawnTicketDto>>(entities);

        return new PagedResultDto<PawnTicketDto>(totalCount, dtos);
    }

    public async Task<GetPawnTicketForEditOutput> GetViaIdForEdit(EntityDto<int> input)
    {
        //await PermissionChecker.AuthorizeAsync(PermissionNames.Pages_PawnTickets);
        var entity = await _repository.GetAsync(input.Id);
        var dto = ObjectMapper.Map<CreateOrEditPawnTicketDto>(entity);
        return new GetPawnTicketForEditOutput { PawnTicket = dto };
    }

    public async Task<int> CreateOrEdit(CreateOrEditPawnTicketDto input)
    {
        if (input.Id > 0)
        {
            //await PermissionChecker.AuthorizeAsync(PermissionNames.Pages_PawnTickets_Edit);
            var entity = await _repository.GetAsync(input.Id);
            ObjectMapper.Map(input, entity);
            await _repository.UpdateAsync(entity);
            return entity.Id;
        }
        else
        {
            //await PermissionChecker.AuthorizeAsync(PermissionNames.Pages_PawnTickets_Create);
            var entity = ObjectMapper.Map<PawnTicket>(input);
            entity.TenantId = AbpSession.TenantId;
            await _repository.InsertAndGetIdAsync(entity);
            return entity.Id;
        }
    }

    public async Task Delete(EntityDto<int> input)
    {
        //await PermissionChecker.AuthorizeAsync(PermissionNames.Pages_PawnTickets_Delete);
        await _repository.DeleteAsync(input.Id);
    }

    public async Task<ListResultDto<PawnTicketLookupDto>> GetPawnTicketsForLookup()
    {
        //await PermissionChecker.AuthorizeAsync(PermissionNames.Pages_PawnTickets);
        var tickets = await _repository.GetAllListAsync();
        var lookup = tickets
            .OrderByDescending(p => p.CreationTime)
            .Select(p => new PawnTicketLookupDto
            {
                Id = p.Id,
                DisplayName = p.TicketNo 
            })
            .ToList();
        return new ListResultDto<PawnTicketLookupDto>(lookup);
    }


}