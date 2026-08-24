using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using PawnCloud.GoldTypes;
using PawnCloud.ItemListings;
using PawnCloud.ItemStatuses;
using PawnCloud.PawnTickets;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.PawnItems
{
    public class PawnItem : FullAuditedEntity<int>, IMayHaveTenant
    {
        public virtual int? TenantId { get; set; }
        public string pawnItemNumber { get; set; }
        public int quantity { get; set; }
        public virtual int? PawnTicket { get; set; }
        [ForeignKey("PawnTicket")]
        public PawnTicket PawnTicketFk { get; set; }
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
        public virtual int? IncludedItem { get; set; } //get option from basic code included item
        public decimal includedItemWeight { get; set; }
        public decimal includedItemValue { get; set; }

        public PawnItem() { }
        public PawnItem(string pawnItemNumber, int quantity, int? pawnTicket, 
            int? itemListing, int? itemStatus, string description, int? goldType, 
            decimal weight, decimal length, string brand, int? includedItem, decimal includedItemWeight, decimal includedItemValue)
        {
            this.pawnItemNumber = pawnItemNumber;
            this.quantity = quantity;
            PawnTicket = pawnTicket;
            ItemListing = itemListing;
            ItemStatus = itemStatus;
            this.description = description;
            GoldType = goldType;
            this.weight = weight;
            this.length = length;
            this.brand = brand;
            IncludedItem = includedItem;
            this.includedItemWeight = includedItemWeight;
            this.includedItemValue = includedItemValue;
        }
    }
}
