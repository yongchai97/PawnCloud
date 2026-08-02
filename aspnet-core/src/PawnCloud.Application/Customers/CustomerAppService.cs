using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using PawnCloud.Authorization;
using PawnCloud.Customers.Dto;

namespace PawnCloud.Customers;

[AbpAuthorize(PermissionNames.Pages_Customers)]
public class CustomerAppService : ApplicationService, ICustomerAppService
{
    private readonly IRepository<Customer, int> _repository;

    public CustomerAppService(IRepository<Customer, int> repository)
    {
        _repository = repository;
    }

    public async Task<PagedResultDto<CustomerDto>> GetAll(PagedCustomerResultRequestDto input)
    {
        await PermissionChecker.AuthorizeAsync(PermissionNames.Pages_Customers);

        var query = _repository.GetAll()
            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                c => c.CustomerNo.Contains(input.Filter) || c.FullName.Contains(input.Filter) || (c.Email != null && c.Email.Contains(input.Filter)));

        var totalCount = await query.CountAsync();

        var entities = await query.OrderByDescending(c => c.CreationTime).PageBy(input).ToListAsync();

        var dtos = ObjectMapper.Map<List<CustomerDto>>(entities);

        return new PagedResultDto<CustomerDto>(totalCount, dtos);
    }

    public async Task<GetCustomerForEditOutput> GetViaIdForEdit(EntityDto<int> input)
    {
        await PermissionChecker.AuthorizeAsync(PermissionNames.Pages_Customers);
        var entity = await _repository.GetAsync(input.Id);
        var dto = ObjectMapper.Map<CreateOrEditCustomerDto>(entity);
        return new GetCustomerForEditOutput { Customer = dto };
    }

    public async Task<int> CreateOrEdit(CreateOrEditCustomerDto input)
    {
        if (input.Id > 0)
        {
            await PermissionChecker.AuthorizeAsync(PermissionNames.Pages_Customers_Edit);
            var entity = await _repository.GetAsync(input.Id);
            ObjectMapper.Map(input, entity);
            await _repository.UpdateAsync(entity);
            return entity.Id;
        }
        else
        {
            await PermissionChecker.AuthorizeAsync(PermissionNames.Pages_Customers_Create);
            var entity = ObjectMapper.Map<Customer>(input);
            entity.TenantId = AbpSession.TenantId;
            await _repository.InsertAndGetIdAsync(entity);
            return entity.Id;
        }
    }

    public async Task Delete(EntityDto<int> input)
    {
        await PermissionChecker.AuthorizeAsync(PermissionNames.Pages_Customers_Delete);
        await _repository.DeleteAsync(input.Id);
    }

    public async Task<ListResultDto<CustomerLookupDto>> GetCustomersForLookup()
    {
        await PermissionChecker.AuthorizeAsync(PermissionNames.Pages_Customers);
        var customers = await _repository.GetAllListAsync();
        var lookup = customers
            .OrderBy(c => c.CustomerNo)
            .Select(c => new CustomerLookupDto
            {
                Id = c.Id,
                DisplayName = c.CustomerNo + " - " + c.FullName
            })
            .ToList();
        return new ListResultDto<CustomerLookupDto>(lookup);
    }
}