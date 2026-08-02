using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Threading.Tasks;
using PawnCloud.Loans.Dto;

namespace PawnCloud.Loans;

public interface ILoanAppService : IApplicationService
{
    Task<PagedResultDto<LoanDto>> GetAll(PagedLoanResultRequestDto input);

    Task<GetLoanForEditOutput> GetViaIdForEdit(EntityDto<int> input);

    Task<int> CreateOrEdit(CreateOrEditLoanDto input);

    Task Delete(EntityDto<int> input);
}