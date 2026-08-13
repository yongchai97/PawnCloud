using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.GoldTypes
{
    public class GoldType : FullAuditedEntity<int>, IMayHaveTenant
    {
        public virtual int? TenantId { get; set; }
        public string purity { get; set; }
        public string description { get; set; }
        public decimal defaultPercentage { get; set; }
        public bool active { get; set; }

        public GoldType() { }
        public GoldType(string purity, string description, decimal defaultPercentage, bool active)
        {
            this.purity = purity;
            this.description = description;
            this.defaultPercentage = defaultPercentage;
            this.active = active;
        }
    }
}
