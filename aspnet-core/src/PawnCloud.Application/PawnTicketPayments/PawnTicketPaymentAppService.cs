using Abp.BlobStoring;
using Abp.Domain.Repositories;
using PawnCloud.CustomerPictures;
using PawnCloud.Customers;
using PawnCloud.PawnTicketPayments.Dto;
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
        public PawnTicketPaymentAppService(
                    IRepository<PawnTicketPayment> pawnTicketPaymentRepository)
        {
            _pawnTicketPaymentRepository = pawnTicketPaymentRepository;
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
            var pawnTicketChecker = await _pawnTicketPaymentRepository.FirstOrDefaultAsync(x => x.Id == pawnTicketPayment.Id.Value);
            if (pawnTicketChecker != null) 
            {
                pawnTicketChecker.PawnTicket = pawnTicketPayment.PawnTicket;
                pawnTicketChecker.amount = pawnTicketPayment.amount;
            }
            await _pawnTicketPaymentRepository.UpdateAsync(pawnTicketChecker);
        }
        public async Task CreateAsync(PawnTicketPaymentDto pawnTicketPayment)
        {
            await _pawnTicketPaymentRepository.InsertAsync(new PawnTicketPayment
            {
                PawnTicket = pawnTicketPayment.PawnTicket,
                amount = pawnTicketPayment.amount,
                TenantId = AbpSession.TenantId
            });
        }
        public async Task DeleteAsync(int id)
        {
            await _pawnTicketPaymentRepository.DeleteAsync(id);
        }
    }
}
