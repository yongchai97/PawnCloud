using Abp.Application.Services;
using Abp.Application.Services.Dto;
using PawnCloud.BasicCodes.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PawnCloud.BasicCodes;

public interface IBasicCodeAppService : IApplicationService
{
    Task<PagedResultDto<BasicCodeDto>> GetAll(PagedBasicCodeResultRequestDto input);
    Task<GetBasicCodeForEditOutput> GetViaIdForEdit(EntityDto<int> input);
    Task<int> CreateOrEdit(CreateOrEditBasicCodeDto input);
    Task Delete(EntityDto<int> input);
    Task<ListResultDto<BasicCodeLookupDto>> GetBasicCodesForLookup();
    Task<ListResultDto<BasicCodeLookupDto>> GetBasicCodesByCategory(int? categoryId);
}
