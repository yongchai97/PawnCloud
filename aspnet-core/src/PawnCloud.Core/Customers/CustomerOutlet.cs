using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using PawnCloud.GeneralSetups;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.Customers
{
    public class CustomerOutlet : FullAuditedEntity<int>, IMayHaveTenant
    {
        public virtual int? TenantId { get; set; }
        public virtual int? Customer { get; set; }
        // optional navigation property
        [ForeignKey("Customer")]
        public virtual Customer CustomerFk { get; set; }
        public virtual int? GeneralSetup { get; set; }
        // optional navigation property
        [ForeignKey("GeneralSetup")]
        public virtual GeneralSetup GeneralSetupFk { get; set; }


    }
}
