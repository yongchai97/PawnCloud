using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Threading.Tasks;
using PawnCloud.Customers.Dto;

namespace PawnCloud.Customers;

public interface ICustomerAppService : IApplicationService
{
    Task<PagedResultDto<CustomerDto>> GetAll(PagedCustomerResultRequestDto input);

    Task<GetCustomerForEditOutput> GetViaIdForEdit(EntityDto<int> input);

    Task<int> CreateOrEdit(CreateOrEditCustomerDto input);

    Task Delete(EntityDto<int> input);

    Task<ListResultDto<CustomerLookupDto>> GetCustomersForLookup(string filter = null);
}