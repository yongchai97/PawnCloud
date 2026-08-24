using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.GeneralSetups
{
    public class GeneralSetup : FullAuditedEntity<int>, IMayHaveTenant
    {
        public virtual int? TenantId { get; set; }
        // all of these setups are only available one time
        public decimal serviceCharge { get; set; } = (decimal)0.5;
        public decimal maximumAllowedPercentage { get; set; } = (decimal)100;   
        public int monthsBetweenPledgeAndExpiry { get; set; } = 6;
        public virtual int? ticketIdMethod { get; set; } // get option from the enum list
        public string appendedString { get; set; }
        public bool AppendYearMonth { get; set; } = true;
        public virtual int? appendedStringBackMethod { get; set;  } // get option from the enum list
        public GeneralSetup() { }
        public GeneralSetup(decimal serviceCharge, decimal maximumAllowedPercentage,
            int monthsBetweenPledgeAndExpiry, int? ticketIdMethod, string appendedString, bool appendYearMonth, int? appendedStringBackMethod)
        {
            this.serviceCharge = serviceCharge;
            this.maximumAllowedPercentage = maximumAllowedPercentage;
            this.monthsBetweenPledgeAndExpiry = monthsBetweenPledgeAndExpiry;
            this.ticketIdMethod = ticketIdMethod;
            this.appendedString = appendedString;
            this.AppendYearMonth = appendYearMonth;
            this.appendedStringBackMethod = appendedStringBackMethod;
        }
    }
}
