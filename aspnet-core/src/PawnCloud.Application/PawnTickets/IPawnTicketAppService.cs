using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Threading.Tasks;
using PawnCloud.PawnTickets.Dto;
using PawnCloud.PawnItems.Dto;

namespace PawnCloud.PawnTickets;

public interface IPawnTicketAppService : IApplicationService
{
    Task<PagedResultDto<PawnTicketDto>> GetAll(PagedPawnTicketResultRequestDto input);

    Task<GetPawnTicketForEditOutput> GetViaIdForEdit(EntityDto<int> input);

    Task<int> CreateOrEdit(CreateOrEditPawnTicketDto input);

    Task Delete(EntityDto<int> input);

    Task<ListResultDto<PawnTicketLookupDto>> GetPawnTicketsForLookup();

    Task<int> CreateWithItems(CreatePawnTicketWithItemsDto input);
}