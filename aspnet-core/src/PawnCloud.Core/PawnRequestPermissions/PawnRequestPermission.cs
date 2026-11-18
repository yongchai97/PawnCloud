using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using PawnCloud.Customers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.PawnRequestPermissions
{
    public class PawnRequestPermission : FullAuditedEntity<int>, IMayHaveTenant
    {
        public virtual int? TenantId { get; set; }

        public virtual string TicketNo { get; set; }
        public virtual int? Customer { get; set; }
        [ForeignKey("Customer")]

        public Customer CustomerFk { get; set; }
        public string request { get; set; } //can be "DELETE", "CHANGENUMBER", "EXCEEDLIMIT"
        public string approvalStatus { get; set; } //can be "PENDING", "APPROVED", "REJECTED"
        public virtual int? requestedBy { get; set; }
        public virtual int? approvedBy { get; set; }
        public string newTicketNo { get; set; } //only used when request is "CHANGENUMBER"
        public PawnRequestPermission() { }
        public PawnRequestPermission(string ticketNo, int? customer, string request, string approvalStatus,
            int? requestedBy, int? approvedBy, string newTicketNo)
        {
            TicketNo = ticketNo;
            Customer = customer;
            this.request = request;
            this.approvalStatus = approvalStatus;
            this.requestedBy = requestedBy;
            this.approvedBy = approvedBy;
            this.newTicketNo = newTicketNo;
        }
    }
}
