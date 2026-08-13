using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using PawnCloud.Authorization;
using PawnCloud.Customers.Dto;
using PawnCloud.GoldTypes;
using PawnCloud.GoldTypes.Dto;
using PawnCloud.ItemStatuses.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.ItemStatuses
{
    public class ItemStatusAppService : ApplicationService
    {
        private readonly IRepository<ItemStatus, int> _repository;

        public ItemStatusAppService(IRepository<ItemStatus, int> repository)
        {
            _repository = repository;
        }
        public async Task<PagedResultDto<ItemStatusDto>> GetAll(PagedCustomerResultRequestDto input)
        {
            await PermissionChecker.AuthorizeAsync(PermissionNames.Pages_Customers);

            var query = _repository.GetAll()
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    c => c.code.Contains(input.Filter) || c.description.Contains(input.Filter));  // Changed from c.purity to c.code

            var totalCount = await query.CountAsync();

            var entities = await query.OrderByDescending(c => c.CreationTime).PageBy(input).ToListAsync();

            var dtos = ObjectMapper.Map<List<ItemStatusDto>>(entities);

            return new PagedResultDto<ItemStatusDto>(totalCount, dtos);
        }
    }
}
