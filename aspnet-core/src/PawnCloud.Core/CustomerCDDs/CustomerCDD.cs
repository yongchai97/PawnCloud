using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using PawnCloud.Customers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.CustomerCDDs
{
    public class CustomerCDD : FullAuditedEntity<int>, IMayHaveTenant
    {
        public virtual int? TenantId { get; set; }
        public virtual int? Customer { get; set; }
        [ForeignKey("Customer")]
        public virtual Customer CustomerFk { get; set; }
        public bool highNetWorth { get; set; }
        public string businessSize { get; set; } // three options, small, medium, large
        public string businessType { get; set; } //three options, lower risk, medium risk, high risk
        public bool publicResearchCompany { get; set; }
        public string researchDescription { get; set; }
        public bool allowAnomaly { get; set; }
        public bool measureCustomer { get; set; }
        public bool offerUnusualTransaction { get; set; }
        public bool nomineeService { get; set; }
        public bool nomineeCustomer { get; set; }
        public bool crossBorderCustomer { get; set; }
        public virtual int? PaymentMode { get; set; } //getting data from basic code payment mode
        public virtual int? DeliveryChannel { get; set; } //getting data from basic code delivery channel
        public bool UNSCRMatching { get; set; }
        public bool MOHAMatching { get; set; }  
        public bool otherMatching { get; set; }
        public string matchingID { get; set; }
        public bool approval { get; set; }
        public virtual int? approvedBy { get; set; } //getting data from user table
        public string matchingDescription { get; set; } // Description of the matching process to AMLA list
        public CustomerCDD() { }
        public CustomerCDD(int? customer, bool highNetWorth, string businessSize, string businessType,
            bool publicResearchCompany, string researchDescription, bool allowAnomaly, bool measureCustomer, bool offerUnusualTransaction,
            bool nomineeService, bool nomineeCustomer, bool crossBorderCustomer, int? paymentMode, int? deliveryChannel,
            bool UNSCRMatching, bool MOHAMatching, bool otherMatching, string matchingID, bool approval, int? approvedBy, string matchingDescription)
        {
            Customer = customer;
            this.highNetWorth = highNetWorth;
            this.businessSize = businessSize;
            this.businessType = businessType;
            this.publicResearchCompany = publicResearchCompany;
            this.researchDescription = researchDescription;
            this.allowAnomaly = allowAnomaly;
            this.measureCustomer = measureCustomer;
            this.offerUnusualTransaction = offerUnusualTransaction;
            this.nomineeService = nomineeService;
            this.nomineeCustomer = nomineeCustomer;
            this.crossBorderCustomer = crossBorderCustomer;
            PaymentMode = paymentMode;
            DeliveryChannel = deliveryChannel;
            this.UNSCRMatching = UNSCRMatching;
            this.MOHAMatching = MOHAMatching;
            this.otherMatching = otherMatching;
            this.matchingID = matchingID;
            this.approval = approval;
            this.approvedBy = approvedBy;
            this.matchingDescription = matchingDescription;
        }
    }
}
