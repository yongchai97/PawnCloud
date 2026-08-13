using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using PawnCloud.Authorization;
using PawnCloud.Customers.Dto;
using PawnCloud.GoldTypes.Dto;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PawnCloud.GoldTypes
{
    public class GoldTypeAppService : ApplicationService
    {
        private readonly IRepository<GoldType, int> _repository;

        public GoldTypeAppService(IRepository<GoldType, int> repository)
        {
            _repository = repository;
        }
        public async Task<PagedResultDto<GoldTypeDto>> GetAll(PagedCustomerResultRequestDto input)
        {
            await PermissionChecker.AuthorizeAsync(PermissionNames.Pages_Customers);

            var query = _repository.GetAll()
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    c => c.purity.Contains(input.Filter) || c.description.Contains(input.Filter));

            var totalCount = await query.CountAsync();

            var entities = await query.OrderByDescending(c => c.CreationTime).PageBy(input).ToListAsync();

            var dtos = ObjectMapper.Map<List<GoldTypeDto>>(entities);

            return new PagedResultDto<GoldTypeDto>(totalCount, dtos);
        }
    }
}
