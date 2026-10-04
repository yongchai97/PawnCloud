using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.GeneralSetups
{
    public class GeneralSetupTable : FullAuditedEntity<int>, IMayHaveTenant
    {
        public virtual int? TenantId { get; set; }
        public virtual int? GeneralSetup { get; set; }
        [ForeignKey("GeneralSetup")]
        public virtual GeneralSetup GeneralSetupFk { get; set; }
        public DateTime effectiveDate { get; set; }
        public decimal maximumPercentage { get; set; }
        public decimal firstMonthInterestRate { get; set; }
        public decimal secondMonthInterestRate { get;set; }
        public decimal thirdMonthInterestRate { get;  set; }
        public decimal fourthMonthInterestRate { get; set; }
        public decimal fifthMonthInterestRate { get; set; }
        public decimal sixthMonthInterestRate { get; set; }
        public decimal seventhMonthInterestRate { get; set; }
        public decimal eighthMonthInterestRate { get; set; }
        public decimal ninthMonthInterestRate { get; set; }
        public decimal tenthMonthInterestRate { get; set; }
        public decimal eleventhMonthInterestRate { get; set; }
        public decimal twelfthMonthInterestRate { get; set; }
        public GeneralSetupTable() { }
        public GeneralSetupTable(int? generalSetup, DateTime effectiveDate, decimal maximumPercentage, decimal firstMonthInterestRate,
            decimal secondMonthInterestRate, decimal thirdMonthInterestRate, decimal fourthMonthInterestRate,
            decimal fifthMonthInterestRate, decimal sixthMonthInterestRate, decimal seventhMonthInterestRate,
            decimal eighthMonthInterestRate, decimal ninthMonthInterestRate, decimal tenthMonthInterestRate,
            decimal eleventhMonthInterestRate, decimal twelfthMonthInterestRate)
        {
            this.GeneralSetup = generalSetup;
            this.effectiveDate = effectiveDate;
            this.maximumPercentage = maximumPercentage;
            this.firstMonthInterestRate = firstMonthInterestRate;
            this.secondMonthInterestRate = secondMonthInterestRate;
            this.thirdMonthInterestRate = thirdMonthInterestRate;
            this.fourthMonthInterestRate = fourthMonthInterestRate;
            this.fifthMonthInterestRate = fifthMonthInterestRate;
            this.sixthMonthInterestRate = sixthMonthInterestRate;
            this.seventhMonthInterestRate = seventhMonthInterestRate;
            this.eighthMonthInterestRate = eighthMonthInterestRate;
            this.ninthMonthInterestRate = ninthMonthInterestRate;
            this.tenthMonthInterestRate = tenthMonthInterestRate;
            this.eleventhMonthInterestRate = eleventhMonthInterestRate;
            this.twelfthMonthInterestRate = twelfthMonthInterestRate;
        }
    }
}
