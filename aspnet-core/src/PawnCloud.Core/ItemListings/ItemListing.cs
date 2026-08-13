using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.ItemListings
{
    public class ItemListing : FullAuditedEntity<int>, IMayHaveTenant
    {
        public virtual int? TenantId { get; set; }
        public string code { get; set; }
        public string description { get; set; }
        public string category { get; set; }

        public ItemListing() { }
        public ItemListing(string code, string description, string category)
        {
            this.code = code;
            this.description = description;
            this.category = category;
        }
    }
}
