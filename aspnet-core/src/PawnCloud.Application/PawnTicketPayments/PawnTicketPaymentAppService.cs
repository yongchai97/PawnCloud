using Abp.BlobStoring;
using Abp.Domain.Repositories;
using Abp.UI;
using PawnCloud.CustomerPictures;
using PawnCloud.Customers;
using PawnCloud.PawnTicketPayments.Dto;
using PawnCloud.PawnTickets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.PawnTicketPayments
{
    public class PawnTicketPaymentAppService : PawnCloudAppServiceBase
    {
        private readonly IRepository<PawnTicketPayment> _pawnTicketPaymentRepository;
        private readonly IRepository<PawnTicket, int> _pawnTicketRepository;
        public PawnTicketPaymentAppService(
                    IRepository<PawnTicketPayment> pawnTicketPaymentRepository,
                    IRepository<PawnTicket, int> pawnTicketRepository)
        {
            _pawnTicketPaymentRepository = pawnTicketPaymentRepository;
            _pawnTicketRepository = pawnTicketRepository;
        }
        public async Task<List<PawnTicketPayment>> GetAll()
        {
            return await _pawnTicketPaymentRepository.GetAllListAsync();
        }
        public async Task<PawnTicketPayment> GetById(int id)
        {
            return await _pawnTicketPaymentRepository.GetAsync(id);
        }
        public async Task<List<PawnTicketPayment>> GetByPawnTicketId(int pawnTicketId)
        {
            return await _pawnTicketPaymentRepository.GetAllListAsync(x => x.PawnTicket == pawnTicketId);
        }
        public async Task CreateOrEdit(PawnTicketPaymentDto input)
        {
            if(input.Id == null)
            {
                await CreateAsync(input);
            }
            else
            {
                await UpdateAsync(input);
            }
        }
        public async Task UpdateAsync(PawnTicketPaymentDto pawnTicketPayment)
        {
            await ValidateAmountAsync(pawnTicketPayment);
            var pawnTicketChecker = await _pawnTicketPaymentRepository.FirstOrDefaultAsync(x => x.Id == pawnTicketPayment.Id.Value);
            if (pawnTicketChecker != null) 
            {
                pawnTicketChecker.PawnTicket = pawnTicketPayment.PawnTicket;
                pawnTicketChecker.amount = pawnTicketPayment.amount;
                pawnTicketChecker.PaymentMethod = pawnTicketPayment.PaymentMethod;

            }
            await _pawnTicketPaymentRepository.UpdateAsync(pawnTicketChecker);
        }
        public async Task CreateAsync(PawnTicketPaymentDto pawnTicketPayment)
        {
            await ValidateAmountAsync(pawnTicketPayment);
            await _pawnTicketPaymentRepository.InsertAsync(new PawnTicketPayment
            {
                PawnTicket = pawnTicketPayment.PawnTicket,
                amount = pawnTicketPayment.amount,
                PaymentMethod = pawnTicketPayment.PaymentMethod,
                TenantId = AbpSession.TenantId
            });
        }

        private async Task ValidateAmountAsync(PawnTicketPaymentDto payment)
        {
            if (!payment.PawnTicket.HasValue)
                throw new UserFriendlyException("A pawn ticket is required.");

            if (payment.amount <= 0)
                throw new UserFriendlyException("Payment amount must be greater than zero.");

            var ticket = await _pawnTicketRepository.GetAsync(payment.PawnTicket.Value);
            var existingTotal = await _pawnTicketPaymentRepository
                .GetAllListAsync(x => x.PawnTicket == payment.PawnTicket && (!payment.Id.HasValue || x.Id != payment.Id.Value));

            if (existingTotal.Sum(x => x.amount) + payment.amount > ticket.amount)
                throw new UserFriendlyException("Payment total cannot exceed the pawn ticket amount.");
        }
        public async Task DeleteAsync(int id)
        {
            await _pawnTicketPaymentRepository.DeleteAsync(id);
        }
    }
}
