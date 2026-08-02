using Abp.Application.Services.Dto;

namespace PawnCloud.Customers.Dto;

public class PagedCustomerResultRequestDto : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
}
