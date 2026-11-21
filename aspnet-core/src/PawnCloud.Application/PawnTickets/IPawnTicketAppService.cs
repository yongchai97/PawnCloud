using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Threading.Tasks;
using PawnCloud.PawnTickets.Dto;

namespace PawnCloud.PawnTickets;

public interface IPawnTicketAppService : IApplicationService
{
    Task<PagedResultDto<PawnTicketDto>> GetAll(PagedPawnTicketResultRequestDto input);

    Task<GetPawnTicketForEditOutput> GetViaIdForEdit(EntityDto<int> input);

    //Task<int> CreateOrEdit(CreateOrEditPawnTicketDto input, string reason);

    Task<string> Delete(EntityDto<int> input, string reason);

    Task<ListResultDto<PawnTicketLookupDto>> GetPawnTicketsForLookup();

}