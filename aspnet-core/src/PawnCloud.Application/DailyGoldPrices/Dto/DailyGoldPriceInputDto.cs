using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.DailyGoldPrices.Dto
{
    public class DailyGoldPriceInputDto
    {
        public DateTime todayDate { get; set; }
        public decimal price { get; set; } = 0;
    }
}
