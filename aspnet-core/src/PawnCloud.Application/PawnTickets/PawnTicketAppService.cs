using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using PawnCloud.Authorization;
using PawnCloud.Customers;
using PawnCloud.DailyGoldPrices;
using PawnCloud.GeneralSetups;
using PawnCloud.GoldTypes;
using PawnCloud.PawnItems;
using PawnCloud.PawnTickets.Dto;
using PawnCloud.SharedFunctions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        CreateOrEditPawnTicketDto dto = new CreateOrEditPawnTicketDto();
        if (entity != null) 
        {
            dto = new CreateOrEditPawnTicketDto(entity.Id, entity.TicketNo, entity.Customer, entity.weight, entity.value,
    entity.pledgedDate, entity.expiryDate, entity.amount, entity.monthlyCustody, entity.serviceCharge,
    entity.slotNumber, entity.GeneralSetup);
        }
        return new GetPawnTicketForEditOutput { PawnTicket = dto };
    }

    public async Task<int> CreateOrEdit(CreateOrEditPawnTicketDto input)
    {
        if (input.Id > 0)
        {
            //await PermissionChecker.AuthorizeAsync(PermissionNames.Pages_PawnTickets_Edit);
            var entity = await _repository.GetAsync(input.Id.Value);
            if(entity != null)
            {
                entity.TicketNo = input.TicketNo;
                entity.Customer = input.Customer;
                entity.weight = input.weight;
                entity.value = input.value;
                entity.pledgedDate = input.pledgedDate;
                entity.expiryDate = input.expiryDate;
                entity.amount = input.amount;
                entity.monthlyCustody = input.monthlyCustody;
                entity.serviceCharge = input.serviceCharge;
                entity.slotNumber = input.slotNumber;
                entity.GeneralSetup = input.GeneralSetup;
                await _repository.UpdateAsync(entity);
            }
            return entity.Id;
        }
        else
        {
            //await PermissionChecker.AuthorizeAsync(PermissionNames.Pages_PawnTickets_Create);
            var entity = new PawnTicket(input.TicketNo, input.Customer, input.weight, input.value,
    input.pledgedDate, input.expiryDate, input.amount, input.monthlyCustody, input.serviceCharge,
    input.slotNumber, input.GeneralSetup);
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
        if (input.Ticket.Id == null)
        {
            if(string.IsNullOrEmpty(input.Ticket.TicketNo))
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
        }
        var editingExistingTicket = input.Ticket.Id != null;
        PawnTicket ticket;
        if (input.Ticket.Id != null)
        {
            ticket = await _repository.GetAsync(input.Ticket.Id.Value);
            if(ticket!=null)
            {
                ticket.TicketNo = input.Ticket.TicketNo;
                ticket.Customer = input.Ticket.Customer;
                ticket.weight = input.Ticket.weight;
                ticket.value = input.Ticket.value;
                ticket.pledgedDate = input.Ticket.pledgedDate;
                ticket.expiryDate = input.Ticket.expiryDate;
                ticket.amount = input.Ticket.amount;
                ticket.monthlyCustody = input.Ticket.monthlyCustody;
                ticket.serviceCharge = input.Ticket.serviceCharge;
                ticket.slotNumber = input.Ticket.slotNumber;
                ticket.GeneralSetup = input.Ticket.GeneralSetup;
                await _repository.UpdateAsync(ticket);
            }
        }
        else
        {
            var allTickets = await _repository.GetAll().ToListAsync();
            ticket = new PawnTicket(input.Ticket.TicketNo, input.Ticket.Customer, input.Ticket.weight, 
                input.Ticket.value, input.Ticket.pledgedDate, input.Ticket.expiryDate, 
                input.Ticket.amount, input.Ticket.monthlyCustody, input.Ticket.serviceCharge,
                input.Ticket.slotNumber, input.Ticket.GeneralSetup);
            ticket.TenantId = AbpSession.TenantId;
            input.Ticket.Id = await _repository.InsertAndGetIdAsync(ticket);
        }
        if(generalSetup != null && dailyGoldPrices != null)
        {
            if(input.Ticket.amount / input.Ticket.weight > dailyGoldPrices.inputPrice * generalSetup.maximumAllowedPercentage / 100)
            {
                return "Error: The amount per weight exceeds the maximum allowed percentage of the daily gold price.";
            }
        }
        if (editingExistingTicket)
        {
            await _PawnItem.DeleteAsync(item => item.PawnTicket == ticket.Id);
        }

        foreach (var itemDto in input.Items)
        {
            var item = new PawnItem(itemDto.pawnItemNumber,itemDto.quantity, input.Ticket.Id, itemDto.ItemListing,
                itemDto.ItemStatus,itemDto.description,itemDto.GoldType,itemDto.weight,itemDto.length,itemDto.brand,
                itemDto.IncludedItem,itemDto.includedItemWeight,itemDto.includedItemValue);
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