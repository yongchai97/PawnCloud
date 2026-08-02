using Abp.Application.Services.Dto;

namespace PawnCloud.Loans.Dto;

public class PagedLoanResultRequestDto : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
}
