using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace PawnCloud.PawnTickets;

public class PawnTicket : FullAuditedEntity<int>
{
    public virtual int? TenantId { get; set; }

    public virtual string TicketNo { get; set; }

    public virtual int? BranchId { get; set; }

    [ForeignKey("Customer")]
    public virtual int CustomerId { get; set; }

    public virtual PawnCloud.Customers.Customer Customer { get; set; }

    public virtual string Status { get; set; }

    public virtual DateTime CreatedDate { get; set; }

    public virtual DateTime? MaturityDate { get; set; }

    public virtual DateTime? ExpiryDate { get; set; }

    public virtual string Remarks { get; set; }
}
