using Abp.Domain.Entities.Auditing;
using PawnCloud.Customers;
using PawnCloud.Customers;
using PawnCloud.GoldTypes;
using PawnCloud.ItemListings;  // Add this line
using PawnCloud.ItemStatuses;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PawnCloud.PawnTickets;

public class PawnTicket : FullAuditedEntity<int>
{
    public virtual int? TenantId { get; set; }

    public virtual string TicketNo { get; set; }


    public virtual int? Customer { get; set; }
    [ForeignKey("Customer")]

    public Customer CustomerFk { get; set; }

    public virtual int? ItemListing { get; set; }
    [ForeignKey("ItemListing")]
    public ItemListing ItemListingFk { get; set; }

    public virtual int? ItemStatus { get; set; }
    [ForeignKey("ItemStatus")]
    public ItemStatus ItemStatusFk { get; set; }

    public string description { get; set; }
    public virtual int? GoldType { get; set; }
    [ForeignKey("GoldType")]
    public GoldType GoldTypeFk { get; set; }
    public decimal weight { get; set; }
    public decimal length { get; set; }
    public string brand { get; set; }
    public virtual int? includedItems { get; set; } //getting data from basic code included items
    public decimal value { get; set; }
    public decimal includedItemWeight { get; set; }

}
