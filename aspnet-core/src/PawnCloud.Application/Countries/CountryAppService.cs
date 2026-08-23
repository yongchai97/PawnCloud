using Abp.Application.Services;
using Abp.Domain.Repositories;
using PawnCloud.CommonDtos;
using PawnCloud.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.Countries
{
    public class CountryAppService : ApplicationService
    {
        private readonly IRepository<Country> _repository;

        public CountryAppService(IRepository<Country> repository)
        {
            _repository = repository;
        }

        public async Task<List<Country>> GetAll()
        {
            var countries = await _repository.GetAllListAsync();
            // You can return the list of countries or perform any other operation as needed
            return countries;
        }
        public async Task <List<Country>> GetAllViaYear(int year)
        {
            var countries = await _repository.GetAllListAsync(x => x.effectiveYear == year);
            return countries;
        }
        public async Task<Country> ReturnCountryRecordViaId(IdFetcher input)
        {
            var country = await _repository.FirstOrDefaultAsync(x => x.Id == input.Id);

            return country;
        }
    }
}
