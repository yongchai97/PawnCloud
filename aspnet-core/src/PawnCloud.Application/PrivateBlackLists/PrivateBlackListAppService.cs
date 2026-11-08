using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using AutoMapper.Internal.Mappers;
using PawnCloud.Authorization;
using PawnCloud.ItemListings;
using PawnCloud.ItemListings.Dto;
using PawnCloud.PrivateBlackLists.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.PrivateBlackLists
{
    [AbpAuthorize(PermissionNames.Pages_PrivateBlackLists)]
    public class PrivateBlackListAppService : ApplicationService
    {
        private readonly IRepository<PrivateBlackList> _repository;

        public PrivateBlackListAppService(IRepository<PrivateBlackList> repository)
        {
            _repository = repository;
        }
        public async Task<List<PrivateBlackList>> GetAll()
        {
            var listings = await _repository.GetAllListAsync();
            return listings;
        }
        public async Task<PrivateBlackList> GetById(int id)
        {
            var listing = await _repository.FirstOrDefaultAsync(id);
            return listing;
        }
        public async Task CreateOrEdit(CreateOrEditPrivateBlackListDto input)
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
        [AbpAuthorize(PermissionNames.Pages_PrivateBlackLists_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _repository.DeleteAsync(input.Id);
        }
        [AbpAuthorize(PermissionNames.Pages_PrivateBlackLists_Create)]
        private async Task Create(CreateOrEditPrivateBlackListDto input)
        {
            //prevent duplicate
            var listingChecker = await _repository.FirstOrDefaultAsync(x => x.FullName == input.FullName);
            if (listingChecker != null)
            {
                return;
            }
            var listing = new PrivateBlackList(input.FullName, input.banReason);
            await _repository.InsertAsync(listing);
        }
        [AbpAuthorize(PermissionNames.Pages_PrivateBlackLists_Edit)]
        private async Task Edit(CreateOrEditPrivateBlackListDto input)
        {
            var listing = await _repository.FirstOrDefaultAsync((int)input.Id);
            if (listing != null)
            {
                listing.FullName = input.FullName;
                listing.banReason = input.banReason;
                await _repository.UpdateAsync(listing);
            }
        }
    }
}
