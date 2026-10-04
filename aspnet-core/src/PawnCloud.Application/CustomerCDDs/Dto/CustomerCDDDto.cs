using PawnCloud.Customers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.CustomerCDDs.Dto
{
    public class CustomerCDDDto
    {
        public virtual int? Customer { get; set; }
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

    }
}
