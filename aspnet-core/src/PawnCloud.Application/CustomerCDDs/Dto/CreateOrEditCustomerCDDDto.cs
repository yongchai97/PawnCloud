using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.CustomerCDDs.Dto
{
    public class CreateOrEditCustomerCDDDto : EntityDto<int?>
    {
        public virtual int? Customer { get; set; }
        public bool highNetWorth { get; set; } = false;
        public string businessSize { get; set; } = string.Empty; // three options, small, medium, large
        public string businessType { get; set; } = string.Empty; //three options, lower risk, medium risk, high risk
        public bool publicResearchCompany { get; set; } = false;
        public string researchDescription { get; set; } = string.Empty;
        public bool allowAnomaly { get; set; } = false;
        public bool measureCustomer { get; set; } = false;
        public bool offerUnusualTransaction { get; set; } = false;
        public bool nomineeService { get; set; } = false;
        public bool nomineeCustomer { get; set; } = false;
        public bool crossBorderCustomer { get; set; } = false;
        public virtual int? PaymentMode { get; set; } //getting data from basic code payment mode
        public virtual int? DeliveryChannel { get; set; } //getting data from basic code delivery channel
        public bool UNSCRMatching { get; set; } = false;
        public bool MOHAMatching { get; set; } = false;
        public bool otherMatching { get; set; } = false;
        public string matchingID { get; set; } = string.Empty;
        public bool approval { get; set; } = false;
        public virtual int? approvedBy { get; set; } //getting data from user table
        public string matchingDescription { get; set; } = string.Empty;// Description of the matching process to AMLA list

        public CreateOrEditCustomerCDDDto()
        {
            // Initialize default values if needed
        }
        public CreateOrEditCustomerCDDDto(int? id, int? Customer, bool highNetWorth, string businessSize, string businessType, 
            bool publicResearchCompany, string researchDescription, bool allowAnomaly, bool measureCustomer, bool offerUnusualTransaction,
            bool nomineeService, bool nomineeCustomer, bool crossBorderCustomer, int? PaymentMode, int? DeliveryChannel, 
            bool UNSCRMatching, bool MOHAMatching, bool otherMatching, string matchingID, bool approval, int? approvedBy, string matchingDescription)
        {
            this.Id = id;
            this.Customer = Customer;
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
            this.PaymentMode = PaymentMode;
            this.DeliveryChannel = DeliveryChannel;
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
