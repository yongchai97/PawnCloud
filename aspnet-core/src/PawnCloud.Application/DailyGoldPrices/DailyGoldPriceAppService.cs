using Abp.Application.Services;
using Abp.Domain.Repositories;
using PawnCloud.DailyGoldPrices.Dto;
using PawnCloud.GoldTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.DailyGoldPrices
{
    public class DailyGoldPriceAppService : ApplicationService
    {
        private readonly IRepository<GoldType> _repository;
        private readonly IRepository<DailyGoldPrice> _dailyGoldPriceRepository;

        public DailyGoldPriceAppService(IRepository<GoldType> repository, IRepository<DailyGoldPrice> dailyGoldPriceRepository)
        {
            _repository = repository;
            _dailyGoldPriceRepository = dailyGoldPriceRepository;
        }
        public async Task<List<DailyGoldPriceDetailDto>> GetAll(DateTime date)
        {
            var goldTypes = await _repository.GetAllListAsync();
            var dailyGoldPrices = await _dailyGoldPriceRepository.GetAllListAsync(x => x.effectiveDate.Date == date.Date);
            List<DailyGoldPriceDetailDto> result = new List<DailyGoldPriceDetailDto>();
            foreach (var goldType in goldTypes)
            {
                var goldPrice = dailyGoldPrices.FirstOrDefault(x => x.GoldType == goldType.Id);
                if (goldPrice != null)
                {
                    result.Add(new DailyGoldPriceDetailDto
                    {
                        Id = goldPrice.Id,
                        GoldType = goldType.Id,
                        purity = goldType.purity,
                        description = goldType.description,
                        effectiveDate = goldPrice.effectiveDate,
                        price = goldPrice.price,
                        memberPrice = goldPrice.memberPrice,
                        nonMemberPrice = goldPrice.nonMemberPrice
                    });
                }
                else
                {
                    result.Add(new DailyGoldPriceDetailDto
                    {
                        GoldType = goldType.Id,
                        purity = goldType.purity,
                        description = goldType.description,
                        effectiveDate = date,
                        price = 0,
                        memberPrice = 0,
                        nonMemberPrice = 0
                    });
                }
            }
            return result;
        }
        public async Task<string> KeyDailyGoldPrice(DailyGoldPriceInputDto input)
        {
            var goldTypes = await _repository.GetAllListAsync();
            var allDailyGoldPrices = await _dailyGoldPriceRepository.GetAllListAsync(x => x.effectiveDate.Date == input.todayDate.Date);
            foreach (var goldType in goldTypes)
            {
                var dailyGoldPriceChecker = allDailyGoldPrices.FirstOrDefault(x => x.GoldType == goldType.Id);
                if (dailyGoldPriceChecker != null)
                {
                    dailyGoldPriceChecker.price = Math.Round(input.price * goldType.defaultPercentage / 100, 2);
                    dailyGoldPriceChecker.memberPrice = Math.Round(input.price * goldType.defaultPercentage / 100, 2);
                    dailyGoldPriceChecker.nonMemberPrice = Math.Round(input.price * goldType.defaultPercentage / 100, 2);
                    await _dailyGoldPriceRepository.UpdateAsync(dailyGoldPriceChecker);
                }
                else
                {
                    var dailyGoldPrice = new DailyGoldPrice
                    {
                        effectiveDate = input.todayDate,
                        price = Math.Round(input.price * goldType.defaultPercentage / 100, 2),
                        GoldType = goldType.Id,
                        memberPrice = Math.Round(input.price * goldType.defaultPercentage / 100 , 2),
                        nonMemberPrice = Math.Round(input.price * goldType.defaultPercentage / 100, 2)
                    };
                    dailyGoldPrice.TenantId = AbpSession.TenantId;
                    await _dailyGoldPriceRepository.InsertAsync(dailyGoldPrice);
                }
            }
            return "Daily Gold Price Updated";
        }
        public async Task CreateOrEdit(DailyGoldPriceDetailDto input)
        {
            var goldType = await _repository.GetAsync(input.GoldType);
            if (goldType == null)
            {
                return;
            }
            if (input.Id == null)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }
        private async Task Create(DailyGoldPriceDetailDto input)
        {
            var dailyGoldPrice = new DailyGoldPrice
            {
                effectiveDate = input.effectiveDate,
                price = input.price,
                GoldType = input.GoldType,
                memberPrice = input.memberPrice,
                nonMemberPrice = input.nonMemberPrice
            };
            dailyGoldPrice.TenantId = AbpSession.TenantId;
            await _dailyGoldPriceRepository.InsertAsync(dailyGoldPrice);
        }
        private async Task Update(DailyGoldPriceDetailDto input)
        {
            var dailyGoldPrice = await _dailyGoldPriceRepository.GetAsync(input.Id.Value);
            if (dailyGoldPrice == null)
            {
                return;
            }
            dailyGoldPrice.effectiveDate = input.effectiveDate;
            dailyGoldPrice.price = input.price;
            dailyGoldPrice.GoldType = input.GoldType;
            dailyGoldPrice.memberPrice = input.memberPrice;
            dailyGoldPrice.nonMemberPrice = input.nonMemberPrice;
            await _dailyGoldPriceRepository.UpdateAsync(dailyGoldPrice);
        }
    }
}

