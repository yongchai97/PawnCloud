using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.RedeemRecords
{
    public class RedeemRecord : FullAuditedEntity<int>, IMayHaveTenant
    {
        public virtual int? TenantId { get; set; }
        public string redeemedBy { get; set; }
        public DateTime redeemedDate { get; set; }
        public int numberOfTickets { get; set; }
        public decimal totalAmount { get; set; }
        public decimal totalCustody { get; set; }
        public decimal totalService { get; set; }
        public decimal grandTotal { get; set; }
        public decimal totalDiscount { get; set; }
        public RedeemRecord() { }
        public RedeemRecord(string redeemedBy, DateTime redeemedDate, int numberOfTickets, decimal totalAmount, decimal totalCustody,
            decimal totalService, decimal grandTotal, decimal totalDiscount)
        {
            this.redeemedBy = redeemedBy;
            this.redeemedDate = redeemedDate;
            this.numberOfTickets = numberOfTickets;
            this.totalAmount = totalAmount;
            this.totalCustody = totalCustody;
            this.totalService = totalService;
            this.grandTotal = grandTotal;
            this.totalDiscount = totalDiscount;
        }
    }
}
