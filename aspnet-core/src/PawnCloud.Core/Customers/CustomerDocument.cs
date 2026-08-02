using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;
namespace PawnCloud.Customers;

public class CustomerDocument : FullAuditedEntity<int>, IMayHaveTenant
{
    // keep TenantId for tenant-awareness; do not implement IMayHaveTenant to avoid interface resolution issues
    public virtual int? TenantId { get; set; }

    public virtual int Customer { get; set; }

    // optional navigation property
    [ForeignKey("Customer")]
    public virtual Customer CustomerFk { get; set; }

    public virtual string DocumentType { get; set; }

    public virtual string FileName { get; set; }

    // store binary content
    public virtual byte[] DocumentContents { get; set; }
}
