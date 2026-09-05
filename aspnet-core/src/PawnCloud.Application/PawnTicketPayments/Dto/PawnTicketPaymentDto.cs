using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.PawnTicketPayments.Dto
{
    public class PawnTicketPaymentDto : EntityDto<int?>
    {
        public virtual int? PawnTicket { get; set; }
        public decimal amount { get; set; }
        public virtual int? PaymentMethod { get; set; } // Get option from Basic code with type Payment Method

        public PawnTicketPaymentDto() { }
        public PawnTicketPaymentDto(int? id, int? pawnTicket, decimal amount, int? paymentMethod)
        {
            this.Id = id;
            PawnTicket = pawnTicket;
            this.amount = amount;
            PaymentMethod = paymentMethod;
        }
    }
}
