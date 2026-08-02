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

[AbpAuthorize(PermissionNames.Pages_CustomerDocuments)]
public class CustomerDocumentAppService : ApplicationService, ICustomerDocumentAppService
{
    private readonly IRepository<CustomerDocument, int> _repository;

    public CustomerDocumentAppService(IRepository<CustomerDocument, int> repository)
    {
        _repository = repository;
    }

    public async Task<PagedResultDto<CustomerDocumentDto>> GetAll(PagedCustomerResultRequestDto input)
    {
        await PermissionChecker.AuthorizeAsync(PermissionNames.Pages_CustomerDocuments);

        var query = _repository.GetAll()
            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                d => d.FileName.Contains(input.Filter) || d.DocumentType.Contains(input.Filter));

        var totalCount = await query.CountAsync();

        var entities = await query.OrderByDescending(d => d.CreationTime).PageBy(input).ToListAsync();

        var dtos = ObjectMapper.Map<List<CustomerDocumentDto>>(entities);

        return new PagedResultDto<CustomerDocumentDto>(totalCount, dtos);
    }

    public async Task<GetCustomerDocumentForEditOutput> GetViaIdForEdit(EntityDto<int> input)
    {
        await PermissionChecker.AuthorizeAsync(PermissionNames.Pages_CustomerDocuments);
        var entity = await _repository.GetAsync(input.Id);
        var dto = ObjectMapper.Map<CreateOrEditCustomerDocumentDto>(entity);
        return new GetCustomerDocumentForEditOutput { Document = dto };
    }

    public async Task<int> CreateOrEdit(CreateOrEditCustomerDocumentDto input)
    {
        if (input.Id > 0)
        {
            await PermissionChecker.AuthorizeAsync(PermissionNames.Pages_CustomerDocuments_Edit);
            var entity = await _repository.GetAsync(input.Id);
            ObjectMapper.Map(input, entity);
            await _repository.UpdateAsync(entity);
            return entity.Id;
        }
        else
        {
            await PermissionChecker.AuthorizeAsync(PermissionNames.Pages_CustomerDocuments_Create);
            var entity = ObjectMapper.Map<CustomerDocument>(input);
            entity.TenantId = AbpSession.TenantId;
            await _repository.InsertAndGetIdAsync(entity);
            return entity.Id;
        }
    }

    public async Task Delete(EntityDto<int> input)
    {
        await PermissionChecker.AuthorizeAsync(PermissionNames.Pages_CustomerDocuments_Delete);
        await _repository.DeleteAsync(input.Id);
    }

    public async Task<ListResultDto<CustomerDocumentSummaryDto>> GetByCustomerId(EntityDto<int> input)
    {
        await PermissionChecker.AuthorizeAsync(PermissionNames.Pages_CustomerDocuments);
        var docs = await _repository.GetAllListAsync(d => d.Customer == input.Id);
        var summary = docs
            .OrderByDescending(d => d.CreationTime)
            .Select(d => new CustomerDocumentSummaryDto
            {
                Id = d.Id,
                Customer = d.Customer,
                DocumentType = d.DocumentType,
                FileName = d.FileName,
                CreationTime = d.CreationTime
            })
            .ToList();
        return new ListResultDto<CustomerDocumentSummaryDto>(summary);
    }
}