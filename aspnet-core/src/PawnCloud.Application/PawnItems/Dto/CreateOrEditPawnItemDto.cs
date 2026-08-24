using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.PawnItems.Dto
{
    public class CreateOrEditPawnItemDto : EntityDto<int?>
    {
        public string pawnItemNumber { get; set; }
        public int quantity { get; set; }
        public virtual int? PawnTicket { get; set; }
        public virtual int? ItemListing { get; set; }
        public virtual int? ItemStatus { get; set; }
        public string description { get; set; }
        public virtual int? GoldType { get; set; }
        public decimal weight { get; set; }
        public decimal length { get; set; }
        public string brand { get; set; }
        public virtual int? IncludedItem { get; set; } //get option from basic code included item
        public decimal includedItemWeight { get; set; }
        public decimal includedItemValue { get; set; }
    }
}
