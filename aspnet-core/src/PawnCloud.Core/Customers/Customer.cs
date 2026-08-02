using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.MultiTenancy;

namespace PawnCloud.Customers;

public class Customer : FullAuditedEntity<int>, IMayHaveTenant
{
    public virtual int? TenantId { get; set; }

    public virtual string CustomerNo { get; set; }

    public virtual string FullName { get; set; }

    public virtual string? NRIC { get; set; }

    public virtual string? PassportNo { get; set; }

    public virtual string? PhoneNo { get; set; }

    public virtual string? Email { get; set; }

    public virtual string? Address1 { get; set; }

    public virtual string? Address2 { get; set; }

    public virtual string? Postcode { get; set; }

    public virtual string? City { get; set; }

    public virtual string? State { get; set; }

    public virtual string? Nationality { get; set; }

    public virtual string? Occupation { get; set; }

    public virtual decimal? MonthlyIncome { get; set; }
}
