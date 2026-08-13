using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.ItemStatuses
{
    public class ItemStatus : FullAuditedEntity<int>, IMayHaveTenant
    {
        public virtual int? TenantId { get; set; }
        public string code { get; set; }
        public string description { get; set; }
        public ItemStatus() { }
        public ItemStatus(string code, string description)
        {
            this.code = code;
            this.description = description;
        }
    }
}
