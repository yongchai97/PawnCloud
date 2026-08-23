using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.Countries
{
    public class Country : FullAuditedEntity<int>, IMayHaveTenant
    {
        public virtual int? TenantId { get; set; }
        public string countryName { get; set; }
        public string countryCodeTwoAlphabet { get; set; }
        public string countryCodeThreeAlphabet { get; set; }
        public string sequenceNumber { get; set; }
        public string riskLevel { get; set; }
        public bool help {  get; set; }
        public int effectiveYear { get; set; }

        public Country() { }
        public Country(string countryName, string countryCodeTwoAlphabet, string countryCodeThreeAlphabet, string sequenceNumber, string riskLevel, bool help, int effectiveYear)
        {
            this.countryName = countryName;
            this.countryCodeTwoAlphabet = countryCodeTwoAlphabet;
            this.countryCodeThreeAlphabet = countryCodeThreeAlphabet;
            this.sequenceNumber = sequenceNumber;
            this.riskLevel = riskLevel;
            this.help = help;
            this.effectiveYear = effectiveYear;
        }
    }
}
