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
        public DateTime bandarayaLicenseExpiryDate { get; set; }
        public DateTime kpktLicenseExpiryDate { get; set; }
        public DateTime kpktPermitIklanExpiryDate { get; set; }
        public DateTime insuranceExpiryDate { get; set; }
        public DateTime pdpaExpiryDate { get; set; }
        public virtual string outletAddress { get; set; }
        public virtual string outletCity { get; set; }

        public virtual string outletState { get; set; }
        public virtual string outletPostcode { get; set; }
        public virtual int? outletCountry { get; set; } //getting data from country table
        public string insurancePolicyNumber { get; set; }
        public DateTime bandarayaLicenseLastUpdate { get; set; }
        public DateTime kpktLicenseLastUpdate { get; set; }
        public DateTime kpktPermitIklanLastUpdate { get; set; }
        public DateTime insuranceLastUpdate { get; set; }
        public DateTime pdpaLastUpdate { get; set; }
    }
}
