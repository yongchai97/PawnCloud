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
        public GeneralSetup() { }
        public GeneralSetup(decimal serviceCharge, decimal maximumAllowedPercentage,
            int monthsBetweenPledgeAndExpiry, int? ticketIdMethod, string appendedString, bool appendYearMonth,
            int? appendedStringBackMethod, string outletName, string outletRegistrationNumber, DateTime bandarayaLicenseExpiryDate,
            DateTime kpktLicenseExpiryDate, DateTime kpktPermitIklanExpiryDate, DateTime insuranceExpiryDate, DateTime pdpaExpiryDate,
            string outletAddress, string outletCity, string outletState, string outletPostcode, int? outletCountry,
            string insurancePolicyNumber,
            DateTime bandarayaLicenseLastUpdate, DateTime kpktLicenseLastUpdate, 
            DateTime kpktPermitIklanLastUpdate, DateTime insuranceLastUpdate, DateTime pdpaLastUpdate)
        {
            this.serviceCharge = serviceCharge;
            this.maximumAllowedPercentage = maximumAllowedPercentage;
            this.monthsBetweenPledgeAndExpiry = monthsBetweenPledgeAndExpiry;
            this.ticketIdMethod = ticketIdMethod;
            this.appendedString = appendedString;
            this.AppendYearMonth = appendYearMonth;
            this.appendedStringBackMethod = appendedStringBackMethod;
            this.outletName = outletName;
            this.outletRegistrationNumber = outletRegistrationNumber;
            this.bandarayaLicenseExpiryDate = bandarayaLicenseExpiryDate;
            this.kpktLicenseExpiryDate = kpktLicenseExpiryDate;
            this.kpktPermitIklanExpiryDate = kpktPermitIklanExpiryDate;
            this.insuranceExpiryDate = insuranceExpiryDate;
            this.pdpaExpiryDate = pdpaExpiryDate;
            this.outletAddress = outletAddress;
            this.outletCity = outletCity;
            this.outletState = outletState;
            this.outletPostcode = outletPostcode;
            this.outletCountry = outletCountry;
            this.insurancePolicyNumber = insurancePolicyNumber;
            this.bandarayaLicenseLastUpdate = bandarayaLicenseLastUpdate;
            this.kpktLicenseLastUpdate = kpktLicenseLastUpdate;
            this.kpktPermitIklanLastUpdate = kpktPermitIklanLastUpdate;
            this.insuranceLastUpdate = insuranceLastUpdate;
            this.pdpaLastUpdate = pdpaLastUpdate;

        }
    }
}
