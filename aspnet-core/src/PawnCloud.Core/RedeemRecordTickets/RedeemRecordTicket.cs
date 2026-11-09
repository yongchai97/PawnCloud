using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using PawnCloud.PawnTickets;
using PawnCloud.RedeemRecords;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.RedeemRecordTickets
{
    public class RedeemRecordTicket : FullAuditedEntity<int>, IMayHaveTenant
    {
        public virtual int? TenantId { get; set; }
        public virtual int? PawnTicket { get; set; }
        [ForeignKey("PawnTicket")]

        public PawnTicket PawnTicketFk { get; set; }
        public virtual int? RedeemRecord { get; set; }
        [ForeignKey("RedeemRecord")]

        public RedeemRecord RedeemRecordFk { get; set; }

        public RedeemRecordTicket() { }
        public RedeemRecordTicket(int? PawnTicket, int? RedeemRecord) 
        {
            this.PawnTicket = PawnTicket;
            this.RedeemRecord = RedeemRecord;
        }

    }
}
