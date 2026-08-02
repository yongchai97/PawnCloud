using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;

namespace PawnCloud.PawnItems;

public class PawnItem : FullAuditedEntity<int>
{
    public virtual int? TenantId { get; set; }

    [ForeignKey("PawnTicket")]
    public virtual int PawnTicketId { get; set; }

    public virtual PawnCloud.PawnTickets.PawnTicket PawnTicket { get; set; }

    public virtual string Category { get; set; }

    public virtual string Description { get; set; }

    public virtual decimal? Weight { get; set; }

    public virtual decimal? Purity { get; set; }

    public virtual string SerialNumber { get; set; }

    public virtual decimal? EstimatedValue { get; set; }

    public virtual decimal? MarketValue { get; set; }

    public virtual decimal? LoanValue { get; set; }

    public virtual string Condition { get; set; }
}
