using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.GeneralSetups.Dto
{
    public class GeneralSetupDto
    {
        public decimal serviceCharge { get; set; }
        public decimal maximumAllowedPercentage { get; set; }
        public int monthsBetweenPledgeAndExpiry { get; set; }
        public int? ticketIdMethod { get; set; }
        public string appendedString { get; set; }
        public bool AppendYearMonth { get; set; }
        public virtual int? appendedStringBackMethod { get; set; } // get option from the enum list
        public string outletName { get; set; }
        public string outletRegistrationNumber { get; set; }

    }
}
