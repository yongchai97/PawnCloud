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
using PawnCloud.PawnItems;
using PawnCloud.GeneralSetups;
using PawnCloud.GoldTypes;
using PawnCloud.DailyGoldPrices;
using PawnCloud.SharedFunctions;

namespace PawnCloud.PawnTickets;

[AbpAuthorize(PermissionNames.Pages_PawnTickets)]
public class PawnTicketAppService : ApplicationService, IPawnTicketAppService
{
    private readonly IRepository<PawnTicket, int> _repository;
    private readonly IRepository<PawnItem> _PawnItem;
    private readonly IRepository<GeneralSetup> _GeneralSetup;
    private readonly IRepository<GoldType> _GoldType;
    private readonly IRepository<DailyGoldPrice> _DailyGoldPrice;
    private SharedFunction sharedFunction;
    public PawnTicketAppService(
        IRepository<PawnTicket, int> repository,
        IRepository<PawnItem> pawnItemRepository,
        IRepository<GeneralSetup> generalSetupRepository,
        IRepository<GoldType> goldTypeRepository,
        IRepository<DailyGoldPrice> dailyGoldPriceRepository,
        SharedFunction sharedFunction)
    {
        _repository = repository;
        _PawnItem = pawnItemRepository;
        _GeneralSetup = generalSetupRepository;
        _GoldType = goldTypeRepository;
        this.sharedFunction = sharedFunction;
        _DailyGoldPrice = dailyGoldPriceRepository;
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
        await _PawnItem.DeleteAsync(x=>x.PawnTicket == input.Id);
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

    public async Task<string> CreateTicketAndPawnItem(CreatePawnTicketWithItemsDto input)
    {
        if(input.Ticket == null)
        {
            return "";
        }
        if(input.Ticket.GeneralSetup == null)
        {
            return "";
        }
        var generalSetup = await _GeneralSetup.FirstOrDefaultAsync(x=>x.Id == input.Ticket.GeneralSetup);
        var dailyGoldPrices = await _DailyGoldPrice.GetAll().Where(x => x.effectiveDate <= input.Ticket.pledgedDate)
            .OrderByDescending(x => x.effectiveDate).FirstOrDefaultAsync();
        if (input.Ticket.Id <= 0)
        {
            var allTickets = await _repository.GetAll().ToListAsync();
            bool repeatedTicketNumber = true;
            while (repeatedTicketNumber)
            {
                string ticketNumber = sharedFunction.GenerateTicketNumber(allTickets.Count, generalSetup, input.Ticket.pledgedDate, input.Ticket.TicketNo);
                var ticketNumberChecker = await _repository.GetAll().Where(x => x.TicketNo == ticketNumber).FirstOrDefaultAsync();
                if (ticketNumberChecker == null)
                {
                    repeatedTicketNumber = false;
                    input.Ticket.TicketNo = ticketNumber;
                }
                allTickets = await _repository.GetAll().ToListAsync();
            }
        }
        PawnTicket ticket;
        if (input.Ticket.Id > 0)
        {
            ticket = await _repository.GetAsync(input.Ticket.Id);
            ObjectMapper.Map(input.Ticket, ticket);
            await _repository.UpdateAsync(ticket);
        }
        else
        {
            ticket = ObjectMapper.Map<PawnTicket>(input.Ticket);
            ticket.TenantId = AbpSession.TenantId;
            await _repository.InsertAsync(ticket);
        }
        if(generalSetup != null && dailyGoldPrices != null)
        {
            if(input.Ticket.amount / input.Ticket.weight > dailyGoldPrices.inputPrice * generalSetup.maximumAllowedPercentage / 100)
            {
                return "Error: The amount per weight exceeds the maximum allowed percentage of the daily gold price.";
            }
        }
        foreach (var itemDto in input.Items)
        {
            var item = ObjectMapper.Map<PawnItem>(itemDto);
            item.PawnTicket = ticket.Id;
            await _PawnItem.InsertAsync(item);
        }
        return "Pawn Ticket and Items created successfully";
    }
    public async Task<CreateOrEditPawnTicketDto> DynamicCalculate(CreatePawnTicketWithItemsDto input)
    {
        if (input.Ticket == null)
        {
            return null;
        }
        if(input.Ticket.GeneralSetup == null)
        {
            return null;
        }
        var generalSetup = await _GeneralSetup.FirstOrDefaultAsync(x=>x.Id == input.Ticket.GeneralSetup);
        var allDailyGoldPrices = await _DailyGoldPrice.GetAll().Where(x=>x.effectiveDate <= input.Ticket.pledgedDate).ToListAsync();
        input.Ticket.weight = 0;
        input.Ticket.value = 0;
        input.Ticket.serviceCharge = generalSetup == null ? (decimal)0.5 : generalSetup.serviceCharge;
        foreach (var itemDto in input.Items)
        {
            var currentGoldPrice = allDailyGoldPrices
                .Where(x => x.GoldType == itemDto.GoldType)
                .OrderByDescending(x => x.effectiveDate)
                .FirstOrDefault();
            // Example calculation: Adjust weight based on gold type
            if (currentGoldPrice != null)
            {
                decimal temporaryValue = itemDto.weight * currentGoldPrice.price;
                input.Ticket.value += temporaryValue;
                input.Ticket.weight += itemDto.weight;
            }
        }
        input.Ticket.expiryDate = input.Ticket.pledgedDate.AddMonths(generalSetup == null ? 6 : generalSetup.monthsBetweenPledgeAndExpiry);
        return input.Ticket;
    }
}