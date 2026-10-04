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
        private readonly IRepository<GeneralSetupTable> _repositoryTable;

        public GeneralSetupAppService(IRepository<GeneralSetup> repository, IRepository<GeneralSetupTable> repositoryTable)
        {
            _repository = repository;
            _repositoryTable = repositoryTable;
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

        public async Task CreateOrEdit(GeneralSetup input)
        {
            if (input.Id != null)
            {
                await Update(input);
            }
            else
            {
                await Create(input);
            }
        }

        public async Task Delete(int id)
        {
            await _repositoryTable.DeleteAsync(x=>x.GeneralSetup == id);
            await _repository.DeleteAsync(id);
        }
        private async Task Create(GeneralSetup input)
        {
            var generalSetupChecker = await _repository.FirstOrDefaultAsync(x=>x.outletName == input.outletName);
            if (generalSetupChecker != null) 
            {
                return;
            }
            var newSetup = new GeneralSetup
            {
                serviceCharge = input.serviceCharge,
                maximumAllowedPercentage = input.maximumAllowedPercentage,
                monthsBetweenPledgeAndExpiry = input.monthsBetweenPledgeAndExpiry,
                ticketIdMethod = input.ticketIdMethod,
                AppendYearMonth = input.AppendYearMonth,
                appendedString = input.appendedString,
                appendedStringBackMethod = input.appendedStringBackMethod,
                outletName = input.outletName,
                outletRegistrationNumber = input.outletRegistrationNumber,
                bandarayaLicenseExpiryDate = input.bandarayaLicenseExpiryDate,
                kpktLicenseExpiryDate = input.kpktLicenseExpiryDate,
                kpktPermitIklanExpiryDate = input.kpktPermitIklanExpiryDate,
                insuranceExpiryDate = input.insuranceExpiryDate,
                pdpaExpiryDate = input.pdpaExpiryDate,
                outletAddress = input.outletAddress,
                outletCity = input.outletCity,
                outletState = input.outletState,
                outletPostcode = input.outletPostcode,
                outletCountry = input.outletCountry,
                insurancePolicyNumber = input.insurancePolicyNumber,
                bandarayaLicenseLastUpdate = DateTime.Now,
                kpktLicenseLastUpdate = DateTime.Now,
                kpktPermitIklanLastUpdate = DateTime.Now,
                insuranceLastUpdate = DateTime.Now,
                pdpaLastUpdate = DateTime.Now
            };
            newSetup.TenantId = AbpSession.TenantId;
            await _repository.InsertAsync(newSetup);
        }
        private async Task Update(GeneralSetup input)
        {
            var existingSetup = await _repository.GetAll().FirstOrDefaultAsync(x=>x.Id == input.Id);
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
                existingSetup.bandarayaLicenseLastUpdate = input.bandarayaLicenseExpiryDate != existingSetup.bandarayaLicenseExpiryDate ? DateTime.Now : existingSetup.bandarayaLicenseLastUpdate;
                existingSetup.kpktLicenseLastUpdate = input.kpktLicenseExpiryDate != existingSetup.kpktLicenseExpiryDate ? DateTime.Now : existingSetup.kpktLicenseLastUpdate;
                existingSetup.kpktPermitIklanLastUpdate = input.kpktPermitIklanExpiryDate != existingSetup.kpktPermitIklanExpiryDate ? DateTime.Now : existingSetup.kpktPermitIklanLastUpdate;
                existingSetup.insuranceLastUpdate = input.insuranceExpiryDate != existingSetup.insuranceExpiryDate ? DateTime.Now : existingSetup.insuranceLastUpdate;
                existingSetup.pdpaLastUpdate = input.pdpaExpiryDate != existingSetup.pdpaExpiryDate ? DateTime.Now : existingSetup.pdpaLastUpdate;
                existingSetup.bandarayaLicenseExpiryDate = input.bandarayaLicenseExpiryDate;
                existingSetup.kpktLicenseExpiryDate = input.kpktLicenseExpiryDate;
                existingSetup.kpktPermitIklanExpiryDate = input.kpktPermitIklanExpiryDate;
                existingSetup.insuranceExpiryDate = input.insuranceExpiryDate;
                existingSetup.pdpaExpiryDate = input.pdpaExpiryDate;
                existingSetup.outletAddress = input.outletAddress;
                existingSetup.outletCity = input.outletCity;
                existingSetup.outletState = input.outletState;
                existingSetup.outletPostcode = input.outletPostcode;
                existingSetup.outletCountry = input.outletCountry;
                existingSetup.insurancePolicyNumber = input.insurancePolicyNumber;
                await _repository.UpdateAsync(existingSetup);
            }
        }
        public IdMethodEnumList ReturnOption()
        {
            var option = new IdMethodEnumList();
            return option;
        }

        public async Task<List<GeneralSetupTable>> GetGeneralSetupTable(int id)
        {
            var setupTable = await _repositoryTable.GetAll().Where(x => x.GeneralSetup == id).ToListAsync();
            return setupTable;
        }
        public async Task DeleteGeneralSetupTable(int id)
        {
            var setupTable = await _repositoryTable.GetAll().FirstOrDefaultAsync(x => x.Id == id);
            if (setupTable != null)
            {
                await _repositoryTable.DeleteAsync(setupTable);
            }
        }
        public async Task CreateOrEditGeneralSetupTable(GeneralSetupTable input)
        {
            if(input.Id == null)
            {
                await CreateGeneralSetupTable(input);
            }
            else
            {
                await UpdateGeneralSetupTable(input);
            }
        }
        private async Task CreateGeneralSetupTable(GeneralSetupTable input)
        {
            //check if already existing one with same date
            var generalSetupTableChecker = await _repositoryTable.FirstOrDefaultAsync(x => x.GeneralSetup == input.GeneralSetup && x.effectiveDate == input.effectiveDate);
            if (generalSetupTableChecker != null)
            {
                return;
            }
            var generalSetupTable = new GeneralSetupTable
            {
                GeneralSetup = input.GeneralSetup,
                effectiveDate = input.effectiveDate,
                maximumPercentage = input.maximumPercentage,
                firstMonthInterestRate = input.firstMonthInterestRate,
                secondMonthInterestRate = input.secondMonthInterestRate,
                thirdMonthInterestRate = input.thirdMonthInterestRate,
                fourthMonthInterestRate = input.fourthMonthInterestRate,
                fifthMonthInterestRate = input.fifthMonthInterestRate,
                sixthMonthInterestRate = input.sixthMonthInterestRate,
                seventhMonthInterestRate = input.seventhMonthInterestRate,
                eighthMonthInterestRate = input.eighthMonthInterestRate,
                ninthMonthInterestRate = input.ninthMonthInterestRate,
                tenthMonthInterestRate = input.tenthMonthInterestRate,
                eleventhMonthInterestRate = input.eleventhMonthInterestRate,
                twelfthMonthInterestRate = input.twelfthMonthInterestRate
            };
            generalSetupTable.TenantId = AbpSession.TenantId;
            await _repositoryTable.InsertAsync(generalSetupTable);
        }
        private async Task UpdateGeneralSetupTable(GeneralSetupTable input)
        {
            var generalSetupTable = await _repositoryTable.GetAll().FirstOrDefaultAsync(x => x.Id == input.Id);
            if (generalSetupTable != null)
            {
                var generalSetupTableChecker = await _repositoryTable.FirstOrDefaultAsync(x => x.GeneralSetup == input.GeneralSetup && x.effectiveDate == input.effectiveDate && x.Id != input.Id);
                if (generalSetupTableChecker != null)
                {
                    return;
                }   
                generalSetupTable.GeneralSetup = input.GeneralSetup;
                generalSetupTable.effectiveDate = input.effectiveDate;
                generalSetupTable.maximumPercentage = input.maximumPercentage;
                generalSetupTable.firstMonthInterestRate = input.firstMonthInterestRate;
                generalSetupTable.secondMonthInterestRate = input.secondMonthInterestRate;
                generalSetupTable.thirdMonthInterestRate = input.thirdMonthInterestRate;
                generalSetupTable.fourthMonthInterestRate = input.fourthMonthInterestRate;
                generalSetupTable.fifthMonthInterestRate = input.fifthMonthInterestRate;
                generalSetupTable.sixthMonthInterestRate = input.sixthMonthInterestRate;
                generalSetupTable.seventhMonthInterestRate = input.seventhMonthInterestRate;
                generalSetupTable.eighthMonthInterestRate = input.eighthMonthInterestRate;
                generalSetupTable.ninthMonthInterestRate = input.ninthMonthInterestRate;
                generalSetupTable.tenthMonthInterestRate = input.tenthMonthInterestRate;
                generalSetupTable.eleventhMonthInterestRate = input.eleventhMonthInterestRate;
                generalSetupTable.twelfthMonthInterestRate = input.twelfthMonthInterestRate;
                await _repositoryTable.UpdateAsync(generalSetupTable);
            }
        }
    }
}
