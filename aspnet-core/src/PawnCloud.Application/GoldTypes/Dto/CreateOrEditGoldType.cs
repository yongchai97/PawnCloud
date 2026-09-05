using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.GoldTypes.Dto
{
    public class CreateOrEditGoldType : EntityDto<int?>
    {
        public string purity { get; set; }
        public string description { get; set; }
        public decimal defaultPercentage { get; set; }
        public bool active { get; set; }

        public CreateOrEditGoldType() { }
        public CreateOrEditGoldType(int? id, string purity, string description, decimal defaultPercentage, bool active)
        {
            Id = id;
            this.purity = purity;
            this.description = description;
            this.defaultPercentage = defaultPercentage;
            this.active = active;
        }
    }
}
