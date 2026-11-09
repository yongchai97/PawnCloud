using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using PawnCloud.AuthorizedPeoples;
using PawnCloud.AuthorizedPeoples.Dto;
using PawnCloud.Customers;
using PawnCloud.PawnTickets;
using PawnCloud.PawnTickets.Dto;
using PawnCloud.RedeemRecords.Dto;
using PawnCloud.RedeemRecordTickets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.RedeemRecords
{
    public class RedeemRecordAppService : ApplicationService
    {
        private readonly IRepository<RedeemRecord> _RedeemRecord;
        private readonly IRepository<RedeemRecordTicket> _RedeemRecordTicket;
        private readonly IRepository<PawnTicket> _PawnTicket;
        private readonly IRepository<Customer> _Customer;
        private readonly IRepository<AuthorizedPeople> _AuthorizedPeople;

        public RedeemRecordAppService(IRepository<RedeemRecord> RedeemRecord,
            IRepository<RedeemRecordTicket> RedeemRecordTicket, IRepository<PawnTicket> PawnTicket, IRepository<Customer> Customer, IRepository<AuthorizedPeople> AuthorizedPeople )
        {
            _RedeemRecord = RedeemRecord;
            _RedeemRecordTicket = RedeemRecordTicket;
            _PawnTicket = PawnTicket;
            _Customer = Customer;
            _AuthorizedPeople = AuthorizedPeople;
        }
        public async Task<List<RedeemRecord>> GetAll()
        {
            var listings = await _RedeemRecord.GetAllListAsync();
            return listings;
        }
        public async Task<RedeemRecord> GetById(int id)
        {
            var listing = await _RedeemRecord.FirstOrDefaultAsync(id);
            return listing;
        }
        public async Task CreateOrEdit(CreateOrEditRedeemRecordDto input)
        {

            if (input.Id == null)
            {
                await Create(input);
            }
            else
            {
                await Edit(input);
            }
        }
        public async Task Delete(EntityDto input)
        {
            await _RedeemRecordTicket.DeleteAsync(x=>x.RedeemRecord == input.Id);
            await _RedeemRecord.DeleteAsync(input.Id);
        }
        public async Task<List<RedeemRecordTicket>> GetAllRedeemRecordTicket()
        {
            var listings = await _RedeemRecordTicket.GetAllListAsync();
            return listings;
        }
        public async Task<List<RedeemRecordTicket>> GetAllRedeemRecordTicketWithId(int id)
        {
            var listings = await _RedeemRecordTicket.GetAllListAsync(x => x.RedeemRecord == id);
            return listings;
        }
        public async Task CreateRedeemRecordTicket(CreateOrEditRedeemRecordTicketDto input )
        {
            var redeemRecordTicketChecker = await _RedeemRecordTicket.FirstOrDefaultAsync(x=>x.PawnTicket == input.PawnTicket);
            if(redeemRecordTicketChecker != null)
            {
                throw new UserFriendlyException("This Ticket has been redeemed, operation cancelled");
            }
            var redeemRecordTicket = new RedeemRecordTicket(input.PawnTicket,input.RedeemRecord);
            redeemRecordTicket.TenantId = AbpSession.TenantId;
            await _RedeemRecordTicket.InsertAsync(redeemRecordTicket);
        }
        public async Task DeleteRedeemRecordTicket(EntityDto input)
        {
            await _RedeemRecordTicket.DeleteAsync(input.Id);
        }
        /*
        public async Task<List<PawnTicket>> GetAuthorizedPawnTickets(CheckIfAuthorized input)
        {
            var authorizedPeople = await _AuthorizedPeople.GetAll().Where(x => x.NRIC == input.NRIC).Select(y=>y.Customer).ToListAsync();
            var anotherCustomerLists = await _Customer.GetAll().Where(x => x.NRIC == input.NRIC || authorizedPeople.Contains(x.Id)).Select(y=>y.Id).ToListAsync();
            var pawnTickets = await _PawnTicket.GetAll().Where(x=> anotherCustomerLists.Contains((int)x.Customer)).ToListAsync();
            var pawnTicketCheckers = pawnTickets.Select(y=>y.Id).ToList();
            var redeemRecordTickets = await _RedeemRecordTicket.GetAll().Where(x=>pawnTicketCheckers.Contains((int)x.PawnTicket)).Select(y=>y.PawnTicket).ToListAsync();
            pawnTickets = pawnTickets.Where(x=>!redeemRecordTickets.Contains(x.Id)).ToList();
            return pawnTickets;

        }
        */
        public async Task<CreateOrEditPawnTicketDto> GetTheRelatedPawnTicketViaTicketNumber(CheckIfAuthorized input)
        {
            var authorizedPeople = await _AuthorizedPeople.GetAll().Where(x => x.NRIC == input.NRIC).Select(y => y.Customer).ToListAsync();
            var anotherCustomerLists = await _Customer.GetAll().Where(x => x.NRIC == input.NRIC || authorizedPeople.Contains(x.Id)).Select(y => y.Id).ToListAsync();

            CreateOrEditPawnTicketDto createOrEditPawnTicketDto = new CreateOrEditPawnTicketDto();

            var pawnTicket = await _PawnTicket.FirstOrDefaultAsync(x=>x.TicketNo == input.ticketNumber && anotherCustomerLists.Contains((int)x.Customer));
            if(pawnTicket != null)
            {
                createOrEditPawnTicketDto = new CreateOrEditPawnTicketDto(pawnTicket.Id,
                    pawnTicket.TicketNo, pawnTicket.Customer, pawnTicket.weight,
                    pawnTicket.value, pawnTicket.pledgedDate, pawnTicket.expiryDate, pawnTicket.amount,
                    pawnTicket.monthlyCustody, pawnTicket.serviceCharge, pawnTicket.slotNumber,
                    pawnTicket.GeneralSetup, pawnTicket.amountPerGram, pawnTicket.discount);
            }
            return createOrEditPawnTicketDto;
        }
        public async Task<RedeemRecordConsolidated> DynamicCalculateGrandTotal(RedeemRecordConsolidated input)
        {
            RedeemRecordConsolidated currentCalculation = input;
            currentCalculation.createOrEditRedeemRecordDto.numberOfTickets = currentCalculation.createOrEditPawnTicketDtos.Count();
            currentCalculation.createOrEditRedeemRecordDto.totalAmount = currentCalculation.createOrEditPawnTicketDtos.Sum(x=>x.amount);
            currentCalculation.createOrEditRedeemRecordDto.totalCustody = currentCalculation.createOrEditPawnTicketDtos.Sum(x=>x.monthlyCustody);
            currentCalculation.createOrEditRedeemRecordDto.totalService = currentCalculation.createOrEditPawnTicketDtos.Sum(x=>x.serviceCharge);
            currentCalculation.createOrEditRedeemRecordDto.totalDiscount = currentCalculation.createOrEditPawnTicketDtos.Sum(x => x.discount);
            currentCalculation.createOrEditRedeemRecordDto.grandTotal = currentCalculation.createOrEditPawnTicketDtos.Sum(x => x.amount)
                + currentCalculation.createOrEditPawnTicketDtos.Sum(x => x.monthlyCustody) + currentCalculation.createOrEditPawnTicketDtos.Sum(x => x.serviceCharge)
                - currentCalculation.createOrEditRedeemRecordDto.totalDiscount;
            foreach(var singlePawnTicket in currentCalculation.createOrEditPawnTicketDtos)
            {
                var pawnTicket = await _PawnTicket.FirstOrDefaultAsync(x=>x.Id == singlePawnTicket.Id);
                if(pawnTicket != null)
                {
                    pawnTicket.discount = singlePawnTicket.discount;
                    pawnTicket.serviceCharge = singlePawnTicket.serviceCharge;
                    await _PawnTicket.UpdateAsync(pawnTicket);
                }
            }
            return currentCalculation;
        }

        private async Task Create(CreateOrEditRedeemRecordDto input)
        {
            var listing = new RedeemRecord(input.redeemedBy,DateTime.Now,input.numberOfTickets,input.totalAmount,
                input.totalCustody,input.totalService,input.grandTotal,input.totalDiscount);
            listing.TenantId = AbpSession.TenantId;
            await _RedeemRecord.InsertAsync(listing);
        }
        private async Task Edit(CreateOrEditRedeemRecordDto input)
        {
            var listing = await _RedeemRecord.FirstOrDefaultAsync(x => x.Id == (int)input.Id);
            if (listing != null)
            {


                listing.redeemedBy = input.redeemedBy;
                listing.redeemedDate = input.redeemedDate;
                listing.numberOfTickets = input.numberOfTickets;
                listing.totalAmount = input.totalAmount;
                listing.totalCustody = input.totalCustody;
                listing.totalService = input.totalService;
                listing.grandTotal = input.grandTotal;
                listing.totalDiscount = input.totalDiscount;

                await _RedeemRecord.UpdateAsync(listing);
            }
        }
    }
}
