using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using PawnCloud.GoldTypes;
using PawnCloud.MiscMasterConfigs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.BasicCodes
{
    public class BasicCode : FullAuditedEntity<int>, IMayHaveTenant
    {
        public virtual int? TenantId { get; set; }
        public virtual string codeName { get; set; }
        public virtual string codeDescription { get; set; }
        public bool systemProvidedValue { get; set; }
        public virtual int? MiscMasterConfig { get; set; }
        // optional navigation property
        [ForeignKey("MiscMasterConfig")]
        public virtual MiscMasterConfig MiscMasterConfigFk { get; set; }

        public BasicCode() { }
        public BasicCode(string codeName, string codeDescription, bool systemProvidedValue, int? MiscMasterConfig)
        {
            this.codeName = codeName;
            this.codeDescription = codeDescription;
            this.systemProvidedValue = systemProvidedValue;
            this.MiscMasterConfig = MiscMasterConfig;
        }
    }
}
