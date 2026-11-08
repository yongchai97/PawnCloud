using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.GlobalBlackLists
{
    public class GlobalBlackList : FullAuditedEntity<int>, IMayHaveTenant
    {
        public virtual int? TenantId { get; set; }
        public string FullName { get; set; }
        public string banReason { get; set; }

        public GlobalBlackList() { }
        public GlobalBlackList(string fullName, string banReason)
        {
            FullName = fullName;
            this.banReason = banReason;
        }
    }
}
