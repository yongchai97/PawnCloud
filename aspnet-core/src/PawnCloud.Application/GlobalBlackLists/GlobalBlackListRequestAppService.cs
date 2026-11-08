using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using PawnCloud.Authorization;
using PawnCloud.GlobalBlackLists.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.GlobalBlackLists
{
    [AbpAuthorize(PermissionNames.Pages_GlobalBlackListRequests)]
    public class GlobalBlackListRequestAppService : ApplicationService
    {
        private readonly IRepository<GlobalBlackListRequest> _repository;

        public GlobalBlackListRequestAppService(IRepository<GlobalBlackListRequest> repository)
        {
            _repository = repository;
        }
        public async Task<List<GlobalBlackListRequest>> GetAll()
        {
            var listings = await _repository.GetAllListAsync();
            return listings;
        }
        public async Task<GlobalBlackListRequest> GetById(int id)
        {
            var listing = await _repository.FirstOrDefaultAsync(id);
            return listing;
        }
        public async Task CreateOrEdit(CreateOrEditGlobalBlackListRequestDto input)
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
        [AbpAuthorize(PermissionNames.Pages_GlobalBlackListRequests_Delete)]
        public async Task Delete(EntityDto input)
        {
            var listing = await _repository.FirstOrDefaultAsync(input.Id);
            if (listing == null)
            {
                return;
            }
            if(listing.status == "approved")
            {
                throw new UserFriendlyException("Cannot delete an approved request.");
            }
            await _repository.DeleteAsync(input.Id);
        }
        [AbpAuthorize(PermissionNames.Pages_GlobalBlackListRequests_Create)]
        private async Task Create(CreateOrEditGlobalBlackListRequestDto input)
        {
            //prevent duplicate
            var listingChecker = await _repository.FirstOrDefaultAsync(x => x.FullName == input.FullName);
            if (listingChecker != null)
            {
                return;
            }
            var listing = new GlobalBlackListRequest(input.FullName, input.banReason, "pending");
            listing.TenantId = AbpSession.TenantId;
            await _repository.InsertAsync(listing);
        }
        [AbpAuthorize(PermissionNames.Pages_GlobalBlackListRequests_Edit)]
        private async Task Edit(CreateOrEditGlobalBlackListRequestDto input)
        {
            var listing = await _repository.FirstOrDefaultAsync((int)input.Id);
            if (listing != null)
            {
                listing.FullName = input.FullName;
                listing.banReason = input.banReason;
                if (!AbpSession.TenantId.HasValue)
                {
                    listing.status = input.status;
                }
                await _repository.UpdateAsync(listing);
            }
        }
    }
}
