using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using PawnCloud.Customers;
using PawnCloud.PawnTickets;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.PawnTicketPayments
{
    public class PawnTicketPayment : FullAuditedEntity<int>, IMayHaveTenant
    {
        public virtual int? TenantId { get; set; }
        public virtual int? PawnTicket { get; set; }  
        [ForeignKey("PawnTicket")]

        public PawnTicket PawnTicketFk { get; set; }


        public decimal amount { get; set; }
        public virtual int? PaymentMethod { get; set; } // Get option from Basic code with type Payment Method
        public PawnTicketPayment() { }
        public PawnTicketPayment(int? pawnTicket, decimal amount, int? paymentMethod)
        {
            PawnTicket = pawnTicket;
            this.amount = amount;
            PaymentMethod = paymentMethod;
        }
    }
}
