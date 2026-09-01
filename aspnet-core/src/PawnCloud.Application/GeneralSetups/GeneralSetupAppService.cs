using Abp.Application.Services;
using Abp.Domain.Repositories;
using PawnCloud.ItemListings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PawnCloud.GeneralSetups.Dto;  // Add this

namespace PawnCloud.GeneralSetups
{
    public class GeneralSetupAppService : ApplicationService
    {
        private readonly IRepository<GeneralSetup> _repository;

        public GeneralSetupAppService(IRepository<GeneralSetup> repository)
        {
            _repository = repository;
        }
        public async Task<GeneralSetup> GetGeneralSetup()
        {
            var list = await _repository.GetAllListAsync();
            var setup = list.FirstOrDefault();
            return setup;
        }

        public async Task<List<GeneralSetup>> GetAll()
        {
            return await _repository.GetAll().OrderBy(x => x.outletName).ToListAsync();
        }

        public async Task<GeneralSetup> CreateOrEdit(GeneralSetup input)
        {
            if (input.Id > 0)
            {
                var existingSetup = await _repository.GetAsync(input.Id);
                existingSetup.serviceCharge = input.serviceCharge;
                existingSetup.maximumAllowedPercentage = input.maximumAllowedPercentage;
                existingSetup.monthsBetweenPledgeAndExpiry = input.monthsBetweenPledgeAndExpiry;
                existingSetup.ticketIdMethod = input.ticketIdMethod;
                existingSetup.AppendYearMonth = input.AppendYearMonth;
                existingSetup.appendedString = input.appendedString;
                existingSetup.appendedStringBackMethod = input.appendedStringBackMethod;
                existingSetup.outletName = input.outletName;
                existingSetup.outletRegistrationNumber = input.outletRegistrationNumber;
                await _repository.UpdateAsync(existingSetup);
                return existingSetup;
            }

            input.TenantId = AbpSession.TenantId;
            return await _repository.InsertAsync(input);
        }

        public async Task Delete(int id)
        {
            await _repository.DeleteAsync(id);
        }
        public async Task<GeneralSetup> UpdateGeneralSetup(GeneralSetup input)
        {
            var existingSetup = await _repository.GetAll().FirstOrDefaultAsync();
            if (existingSetup != null)
            {
                existingSetup.serviceCharge = input.serviceCharge;
                existingSetup.maximumAllowedPercentage = input.maximumAllowedPercentage;
                existingSetup.monthsBetweenPledgeAndExpiry = input.monthsBetweenPledgeAndExpiry;
                existingSetup.ticketIdMethod = input.ticketIdMethod;
                existingSetup.AppendYearMonth = input.AppendYearMonth;
                existingSetup.appendedString = input.appendedString;
                existingSetup.appendedStringBackMethod = input.appendedStringBackMethod;
                existingSetup.outletName = input.outletName;
                existingSetup.outletRegistrationNumber = input.outletRegistrationNumber;

                await _repository.UpdateAsync(existingSetup);
                return existingSetup;
            }
            else
            {
                var newSetup = new GeneralSetup(input.serviceCharge, input.maximumAllowedPercentage,
                    input.monthsBetweenPledgeAndExpiry, input.ticketIdMethod, input.appendedString, 
                    input.AppendYearMonth, input.appendedStringBackMethod, input.outletName, input.outletRegistrationNumber);
                newSetup.TenantId = AbpSession.TenantId;
                await _repository.InsertAsync(newSetup);
                return newSetup;
            }

        }
        public IdMethodEnumList ReturnOption()
        {
            var option = new IdMethodEnumList();
            return option;
        }
    }
}
