using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.GoldTypes.Dto
{
    public class GoldTypeDto
    {
        public string purity { get; set; }
        public string description { get; set; }
        public decimal defaultPercentage { get; set; }
        public bool active { get; set; }

    }
}
