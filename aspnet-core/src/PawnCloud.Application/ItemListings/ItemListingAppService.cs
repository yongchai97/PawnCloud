using Abp.Application.Services;
using Abp.Domain.Repositories;
using PawnCloud.ItemListings.Dto;
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
        public async Task<ItemListing> GetById(int id)
        {
            var listing = await _repository.FirstOrDefaultAsync(id);
            return listing;
        }
        public async Task DeactiveItemListing(int id)
        {
            var listing = await _repository.FirstOrDefaultAsync(id);
            if (listing != null)
            {
                listing.IsDeleted = true;
                listing.DeletionTime = DateTime.Now;
                listing.DeleterUserId = AbpSession.UserId;
                await _repository.UpdateAsync(listing);
            }
            return;
        }
        public async Task CreateOrEdit(CreateOrEditItemListingDto input)
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

        private async Task Create(CreateOrEditItemListingDto input)
        {
            //prevent duplicate
            var listingChecker = await _repository.FirstOrDefaultAsync(x => x.code == input.code && !x.IsDeleted);
            if (listingChecker != null)
            {
                return;
            }
            var listing = new ItemListing(input.code,input.description,input.category);
            await _repository.InsertAsync(listing);
        }
        private async Task Edit(CreateOrEditItemListingDto input)
        {
            var listing = await _repository.FirstOrDefaultAsync((int)input.Id);
            if (listing != null)
            {
                ObjectMapper.Map(input, listing);
                await _repository.UpdateAsync(listing);
            }
        }
    }
}
