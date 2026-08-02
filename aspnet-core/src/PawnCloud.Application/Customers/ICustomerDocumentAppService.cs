using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Threading.Tasks;
using PawnCloud.Customers.Dto;

namespace PawnCloud.Customers;

public interface ICustomerDocumentAppService : IApplicationService
{
    Task<PagedResultDto<CustomerDocumentDto>> GetAll(PagedCustomerResultRequestDto input);

    Task<GetCustomerDocumentForEditOutput> GetViaIdForEdit(EntityDto<int> input);

    Task<int> CreateOrEdit(CreateOrEditCustomerDocumentDto input);

    Task Delete(EntityDto<int> input);

    Task<ListResultDto<CustomerDocumentSummaryDto>> GetByCustomerId(EntityDto<int> input);
}