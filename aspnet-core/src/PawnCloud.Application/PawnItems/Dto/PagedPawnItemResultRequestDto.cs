using Abp.Application.Services.Dto;

namespace PawnCloud.PawnItems.Dto;

public class PagedPawnItemResultRequestDto : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
}
