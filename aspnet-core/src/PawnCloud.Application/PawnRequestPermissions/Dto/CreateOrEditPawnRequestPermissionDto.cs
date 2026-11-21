using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.PawnRequestPermissions.Dto
{
    public class CreateOrEditPawnRequestPermissionDto : EntityDto<int?>
    {
        public virtual string TicketNo { get; set; }
        public virtual int? Customer { get; set; }
        public string request { get; set; } //can be "DELETE", "CHANGENUMBER", "EXCEEDLIMIT"
        public string approvalStatus { get; set; } //can be "PENDING", "APPROVED", "REJECTED"
        public virtual int? requestedBy { get; set; }
        public virtual int? approvedBy { get; set; }
        public string newTicketNo { get; set; } //only used when request is "CHANGENUMBER"
        public string reason { get; set; } //reason for the request

        public CreateOrEditPawnRequestPermissionDto() { }
        public CreateOrEditPawnRequestPermissionDto(int? id, string ticketNo, int? customer, string request, string approvalStatus,
            int? requestedBy, int? approvedBy, string newTicketNo, string reason)
        {
            Id = id;
            TicketNo = ticketNo;
            Customer = customer;
            this.request = request;
            this.approvalStatus = approvalStatus;
            this.requestedBy = requestedBy;
            this.approvedBy = approvedBy;
            this.newTicketNo = newTicketNo;
            this.reason = reason;
        }
    }
}
