using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.ItemListings.Dto
{
    public class CreateOrEditItemListingDto : EntityDto<int?>
    {
        public string code { get; set; }
        public string description { get; set; }
        public string category { get; set; }
        public CreateOrEditItemListingDto() { }
        public CreateOrEditItemListingDto(int? id, string code, string description, string category)
        {
            this.Id = id;
            this.code = code;
            this.description = description;
            this.category = category;
        }
    }
}
