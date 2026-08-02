using Abp.Application.Services.Dto;

namespace PawnCloud.PawnTickets.Dto;

public class PagedPawnTicketResultRequestDto : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
}
