using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.PawnRequestPermissions.Dto
{
    public class PawnRequestPermissionDto
    {
        public virtual string TicketNo { get; set; }
        public virtual int? Customer { get; set; }
        public string request { get; set; } //can be "DELETE", "CHANGENUMBER", "EXCEEDLIMIT"
        public string approvalStatus { get; set; } //can be "PENDING", "APPROVED", "REJECTED"
        public virtual int? requestedBy { get; set; }
        public virtual int? approvedBy { get; set; }
        public string newTicketNo { get; set; } //only used when request is "CHANGENUMBER"

    }
}
