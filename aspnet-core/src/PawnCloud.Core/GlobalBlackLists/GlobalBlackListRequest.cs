using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.GlobalBlackLists
{
    public class GlobalBlackListRequest : FullAuditedEntity<int>, IMayHaveTenant
    {
        public virtual int? TenantId { get; set; }
        public string FullName { get; set; }
        public string banReason { get; set; }
        public string status { get; set; } //pending, approved, rejected

        public GlobalBlackListRequest() { }
        public GlobalBlackListRequest(string fullName, string banReason, string status)
        {
            FullName = fullName;
            this.banReason = banReason;
            this.status = status;
        }
    }
}
