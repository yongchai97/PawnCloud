using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.DailyGoldPrices.Dto
{
    public class DailyGoldPriceDetailDto : EntityDto<int?>
    {
        public virtual int GoldType { get; set; }
        public string purity { get; set; }
        public string description { get; set; }
        public DateTime effectiveDate { get; set; }
        public decimal price { get; set; }
        public decimal memberPrice { get; set; }
        public decimal nonMemberPrice { get; set; }

    }
}
