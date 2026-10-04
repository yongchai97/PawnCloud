using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.GeneralSetups.Dto
{
    public class GeneralSetupTableDto
    {
        public virtual int? GeneralSetup { get; set; }
        public DateTime effectiveDate { get; set; }
        public decimal maximumPercentage { get; set; }
        public decimal firstMonthInterestRate { get; set; }
        public decimal secondMonthInterestRate { get; set; }
        public decimal thirdMonthInterestRate { get; set; }
        public decimal fourthMonthInterestRate { get; set; }
        public decimal fifthMonthInterestRate { get; set; }
        public decimal sixthMonthInterestRate { get; set; }
        public decimal seventhMonthInterestRate { get; set; }
        public decimal eighthMonthInterestRate { get; set; }
        public decimal ninthMonthInterestRate { get; set; }
        public decimal tenthMonthInterestRate { get; set; }
        public decimal eleventhMonthInterestRate { get; set; }
        public decimal twelfthMonthInterestRate { get; set; }
    }
}
