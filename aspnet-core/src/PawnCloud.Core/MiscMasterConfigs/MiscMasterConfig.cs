using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.MiscMasterConfigs
{
    public class MiscMasterConfig : FullAuditedEntity<int>, IMayHaveTenant
    {
        public virtual int? TenantId { get; set; }
        public string category { get; set; }
        public bool availableForUser { get; set; }

        public MiscMasterConfig() { }
        public MiscMasterConfig(string category, bool availableForUser)
        {
            this.category = category;
            this.availableForUser = availableForUser;
        }
    }
}
