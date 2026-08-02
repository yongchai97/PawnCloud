using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Threading.Tasks;
using PawnCloud.PawnItems.Dto;

namespace PawnCloud.PawnItems;

public interface IPawnItemAppService : IApplicationService
{
    Task<PagedResultDto<PawnItemDto>> GetAll(PagedPawnItemResultRequestDto input);

    Task<GetPawnItemForEditOutput> GetViaIdForEdit(EntityDto<int> input);

    Task<int> CreateOrEdit(CreateOrEditPawnItemDto input);

    Task Delete(EntityDto<int> input);

    Task<ListResultDto<PawnItemDto>> GetByPawnTicketId(EntityDto<int> input);
}