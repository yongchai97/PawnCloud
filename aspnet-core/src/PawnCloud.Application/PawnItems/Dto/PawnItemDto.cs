using Abp.Application.Services.Dto;
using System;

namespace PawnCloud.PawnItems.Dto;

public class PawnItemDto : EntityDto<int>
{
    public int? TenantId { get; set; }

    public int PawnTicketId { get; set; }

    public string Category { get; set; }

    public string Description { get; set; }

    public decimal? Weight { get; set; }

    public decimal? Purity { get; set; }

    public string SerialNumber { get; set; }

    public decimal? EstimatedValue { get; set; }

    public decimal? MarketValue { get; set; }

    public decimal? LoanValue { get; set; }

    public string Condition { get; set; }
}
