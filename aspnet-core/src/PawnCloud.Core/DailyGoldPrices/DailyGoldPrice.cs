using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using PawnCloud.Customers;
using PawnCloud.GoldTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.DailyGoldPrices
{
    public class DailyGoldPrice : FullAuditedEntity<int>, IMayHaveTenant
    {
        public virtual int? TenantId { get; set; }
        public DateTime effectiveDate { get; set; }
        public decimal price { get; set; }
        public virtual int GoldType { get; set; }

        // optional navigation property
        [ForeignKey("GoldType")]
        public virtual GoldType GoldTypeFk { get; set; }
        public decimal memberPrice { get; set; }
        public decimal nonMemberPrice { get; set; }
    }
}
