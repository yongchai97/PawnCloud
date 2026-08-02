using Abp.Application.Services.Dto;
using System;

namespace PawnCloud.Customers.Dto;

public class CustomerDto : EntityDto<int>
{
    public int? TenantId { get; set; }

    public string CustomerNo { get; set; }

    public string FullName { get; set; }

    public string? NRIC { get; set; }

    public string? PassportNo { get; set; }

    public string? PhoneNo { get; set; }

    public string? Email { get; set; }

    public string? Address1 { get; set; }

    public string? Address2 { get; set; }

    public string? Postcode { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public string? Nationality { get; set; }

    public string? Occupation { get; set; }

    public decimal? MonthlyIncome { get; set; }

    public DateTime CreationTime { get; set; }
}
