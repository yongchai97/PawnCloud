using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using PawnCloud.GeneralSetups;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.UserOutlets
{
    public class UserOutlet : FullAuditedEntity<int>, IMayHaveTenant
    {
        public virtual int? TenantId { get; set; }
        public virtual int? User { get; set; }
        public virtual int? GeneralSetup { get; set; }
        [ForeignKey("GeneralSetup")]
        public GeneralSetup GeneralSetupFk { get; set; }

        public UserOutlet() { }
        public UserOutlet(int? user, int? generalSetup)
        {
            User = user;
            GeneralSetup = generalSetup;
        }
    }
}
