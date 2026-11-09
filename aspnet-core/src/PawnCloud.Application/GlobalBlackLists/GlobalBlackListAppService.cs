using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using PawnCloud.Authorization;
using PawnCloud.GlobalBlackLists.Dto;
using PawnCloud.PrivateBlackLists;
using PawnCloud.PrivateBlackLists.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.GlobalBlackLists
{
    [AbpAuthorize(PermissionNames.Pages_GlobalBlackLists)]
    public class GlobalBlackListAppService : ApplicationService
    {
        private readonly IRepository<GlobalBlackList> _repository;
        private readonly IRepository<GlobalBlackListRequest> _requestRepository;

        public GlobalBlackListAppService(IRepository<GlobalBlackList> repository, IRepository<GlobalBlackListRequest> requestRepository)
        {
            _repository = repository;
            _requestRepository = requestRepository;
        }
        public async Task<List<GlobalBlackList>> GetAll()
        {
            var listings = await _repository.GetAllListAsync();
            return listings;
        }
        public async Task<GlobalBlackList> GetById(int id)
        {
            var listing = await _repository.FirstOrDefaultAsync(id);
            return listing;
        }
        [AbpAuthorize(PermissionNames.Pages_GlobalBlackLists_Create)]
        public async Task CreateOrEdit(CreateOrEditGlobalBlackListDto input)
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
        public async Task<List<GlobalBlackListRequest>> GetAllRequestFromAllTenant()
        {
            //await PermissionChecker.AuthorizeAsync(PermissionNames.Pages_GlobalBlackLists_Edit);
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                return await _requestRepository.GetAllListAsync();
            }
        }
        [AbpAuthorize(PermissionNames.Pages_GlobalBlackLists_Create)]
        public async Task ConvertTenantRequestToGlobalBlackList(CreateOrEditGlobalBlackListRequestDto input)
        {
            //await PermissionChecker.AuthorizeAsync(PermissionNames.Pages_GlobalBlackLists_Edit);
            if (!input.Id.HasValue)
            {
                return;
            }

            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                var request = await _requestRepository.FirstOrDefaultAsync(input.Id.GetValueOrDefault());
                if (request == null)
                {
                    return;
                }

                request.FullName = input.FullName;
                request.banReason = input.banReason;
                request.status = input.status;
                await _requestRepository.UpdateAsync(request);

                if (!string.Equals(input.status, "approved", StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                var listingChecker = await _repository.FirstOrDefaultAsync(x => x.FullName == input.FullName);
                if (listingChecker == null)
                {
                    await _repository.InsertAsync(new GlobalBlackList(input.FullName, input.banReason) { TenantId = null });
                }
            }

        }
        [AbpAuthorize(PermissionNames.Pages_GlobalBlackLists_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _repository.DeleteAsync(input.Id);
        }
        private async Task Create(CreateOrEditGlobalBlackListDto input)
        {
            //prevent duplicate
            var listingChecker = await _repository.FirstOrDefaultAsync(x => x.FullName == input.FullName);
            if (listingChecker != null)
            {
                return;
            }
            var listing = new GlobalBlackList(input.FullName, input.banReason);
            listing.TenantId = AbpSession.TenantId;
            await _repository.InsertAsync(listing);
        }
        private async Task Edit(CreateOrEditGlobalBlackListDto input)
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
