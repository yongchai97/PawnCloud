using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using PawnCloud.Authorization;
using PawnCloud.Customers.Dto;
using PawnCloud.GoldTypes.Dto;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PawnCloud.GoldTypes
{
    public class GoldTypeAppService : ApplicationService
    {
        private readonly IRepository<GoldType, int> _repository;

        public GoldTypeAppService(IRepository<GoldType, int> repository)
        {
            _repository = repository;
        }
        public async Task<List<GoldType>> GetAll()
        {
            var goldTypes = await _repository.GetAllListAsync();
            return goldTypes;
        }
        public async Task<GoldType> GetById(int id)
        {
            var goldType = await _repository.FirstOrDefaultAsync(id);
            return goldType;
        }
        public async Task DeactiveGoldType(int id)
        {
            var goldType = await _repository.FirstOrDefaultAsync(id);
            if (goldType != null)
            {
                goldType.active = !goldType.active;
                await _repository.UpdateAsync(goldType);
            }
            return;
        }
        public async Task CreateOrEdit(CreateOrEditGoldType input)
        {
            if(input.Id == null)
            {
                await Create(input);
            }
            else
            {
                await Edit(input);
            }

            
        }
        private async Task Create(CreateOrEditGoldType input)
        {
            var goldTypeExists = await _repository.FirstOrDefaultAsync(x => x.purity == input.purity);
            if (goldTypeExists != null)
            {
                return;
            }
            var goldType = new GoldType(input.purity,input.description, input.defaultPercentage,true);
            await _repository.InsertAsync(goldType);
        }
        private async Task Edit(CreateOrEditGoldType input)
        {
            var goldType = await _repository.FirstOrDefaultAsync((int)input.Id);
            if (goldType != null)
            {
                goldType.purity = input.purity;
                goldType.description = input.description;
                goldType.defaultPercentage = input.defaultPercentage;
                await _repository.UpdateAsync(goldType);
            }
        }
    }
}
