using Abp.Application.Services.Dto;
using System;

namespace PawnCloud.PawnTickets.Dto;

public class PawnTicketDto : EntityDto<int>
{
    public int? TenantId { get; set; }

    public string TicketNo { get; set; }


    public int? Customer { get; set; }
    public virtual int? ItemListing { get; set; }
    public virtual int? ItemStatus { get; set; }
    public string description { get; set; }
    public virtual int? GoldType { get; set; }
    public decimal weight { get; set; }
    public decimal length { get; set; }
    public string brand { get; set; }
    public virtual int? includedItems { get; set; } //getting data from basic code included items
    public decimal value { get; set; }
    public decimal includedItemWeight { get; set; }
}
