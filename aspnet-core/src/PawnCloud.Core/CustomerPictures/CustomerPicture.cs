using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using PawnCloud.Customers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.CustomerPictures
{
    public class CustomerPicture : FullAuditedEntity<int>, IMayHaveTenant
    {

        public virtual int? TenantId { get; set; }
        [Required]
        [StringLength(500)]
        public string BlobName { get; set; }

        [Required]
        [StringLength(255)]
        public string OriginalFileName { get; set; }

        [Required]
        [StringLength(100)]
        public string ContentType { get; set; }

        public long FileSize { get; set; }

        public virtual int? Customer { get; set; }
        [ForeignKey("Customer")]
        public Customer CustomerFk { get; set; }


    }
}
