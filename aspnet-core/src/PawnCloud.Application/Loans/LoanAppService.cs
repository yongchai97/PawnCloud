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
using PawnCloud.Loans.Dto;

namespace PawnCloud.Loans;

[AbpAuthorize(PermissionNames.Pages_Loans)]
public class LoanAppService : ApplicationService, ILoanAppService
{
    private readonly IRepository<Loan, int> _repository;

    public LoanAppService(IRepository<Loan, int> repository)
    {
        _repository = repository;
    }

    public async Task<PagedResultDto<LoanDto>> GetAll(PagedLoanResultRequestDto input)
    {
        await PermissionChecker.AuthorizeAsync(PermissionNames.Pages_Loans);

        var query = _repository.GetAll()
            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                l => l.Status.Contains(input.Filter));

        var totalCount = await query.CountAsync();

        var entities = await query.OrderByDescending(l => l.CreationTime).PageBy(input).ToListAsync();

        var dtos = ObjectMapper.Map<List<LoanDto>>(entities);

        return new PagedResultDto<LoanDto>(totalCount, dtos);
    }

    public async Task<GetLoanForEditOutput> GetViaIdForEdit(EntityDto<int> input)
    {
        await PermissionChecker.AuthorizeAsync(PermissionNames.Pages_Loans);
        var entity = await _repository.GetAsync(input.Id);
        var dto = ObjectMapper.Map<CreateOrEditLoanDto>(entity);
        return new GetLoanForEditOutput { Loan = dto };
    }

    public async Task<int> CreateOrEdit(CreateOrEditLoanDto input)
    {
        if (input.Id > 0)
        {
            await PermissionChecker.AuthorizeAsync(PermissionNames.Pages_Loans_Edit);
            var entity = await _repository.GetAsync(input.Id);
            ObjectMapper.Map(input, entity);
            await _repository.UpdateAsync(entity);
            return entity.Id;
        }
        else
        {
            await PermissionChecker.AuthorizeAsync(PermissionNames.Pages_Loans_Create);
            var entity = ObjectMapper.Map<Loan>(input);
            entity.TenantId = AbpSession.TenantId;
            await _repository.InsertAndGetIdAsync(entity);
            return entity.Id;
        }
    }

    public async Task Delete(EntityDto<int> input)
    {
        await PermissionChecker.AuthorizeAsync(PermissionNames.Pages_Loans_Delete);
        await _repository.DeleteAsync(input.Id);
    }
}