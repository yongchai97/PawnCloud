using Abp.Application.Services;
using Abp.Domain.Repositories;
using PawnCloud.GeneralSetups;
using PawnCloud.ItemStatuses;
using PawnCloud.PawnItems.Dto;
using PawnCloud.PawnTickets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.PawnItems
{
    public class PawnItemAppService : ApplicationService
    {
        private readonly IRepository<PawnItem> _repository;
        private readonly IRepository<PawnTicket> _pawnTicketRepository;
        private readonly IRepository<GeneralSetup> _GeneralSetup;


        public PawnItemAppService(IRepository<PawnItem> repository, IRepository<PawnTicket> pawnTicketRepository, IRepository<GeneralSetup> generalSetupRepository)
        {
            _repository = repository;
            _pawnTicketRepository = pawnTicketRepository;
            _GeneralSetup = generalSetupRepository;
        }
        public async Task<List<PawnItem>> GetAll()
        {
            var pawnItems = await _repository.GetAllListAsync();
            return pawnItems;
        }
        public async Task <PawnItem> GetById(int id)
        {
            var pawnItem = await _repository.GetAsync(id);
            return pawnItem;
        }
        public async Task <List<PawnItemDto>> GetAllViaPawnTicketId(GetPawnItemViaPawnTicket input)
        {
            var pawnItems = await _repository.GetAllListAsync(p => p.PawnTicket == input.PawnTicket);
            return pawnItems.Select(p => ObjectMapper.Map<PawnItemDto>(p)).ToList();
        }
        public async Task CreateOrEdit(CreateOrEditPawnItemDto input)
        {
            if(input.Id == null)
            {
                var pawnItem = ObjectMapper.Map<PawnItem>(input);
                await _repository.InsertAsync(pawnItem);
            }
            else
            {
                var pawnItem = await _repository.GetAsync(input.Id.Value);
                ObjectMapper.Map(input, pawnItem);
                await _repository.UpdateAsync(pawnItem);
            }
        }
    }
}
