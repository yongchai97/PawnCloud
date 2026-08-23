using Abp.Application.Services;
using Abp.Domain.Repositories;
using PawnCloud.ItemStatuses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.ItemListings
{
    public class ItemListingAppService : ApplicationService
    {
        private readonly IRepository<ItemListing> _repository;

        public ItemListingAppService(IRepository<ItemListing> repository)
        {
            _repository = repository;
        }
        public async Task<List<ItemListing>> GetAll()
        {
            var listings = await _repository.GetAllListAsync();
            return listings;
        }
    }
}
