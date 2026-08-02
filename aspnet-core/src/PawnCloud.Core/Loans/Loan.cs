using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace PawnCloud.Loans;

public class Loan : FullAuditedEntity<int>
{
    public virtual int? TenantId { get; set; }

    [ForeignKey("PawnTicket")]
    public virtual int PawnTicketId { get; set; }

    public virtual PawnCloud.PawnTickets.PawnTicket PawnTicket { get; set; }

    public virtual decimal Principal { get; set; }

    public virtual decimal InterestRate { get; set; }

    public virtual string InterestType { get; set; }

    public virtual int LoanPeriod { get; set; }

    public virtual DateTime InterestStartDate { get; set; }

    public virtual DateTime MaturityDate { get; set; }

    public virtual decimal OutstandingPrincipal { get; set; }

    public virtual decimal OutstandingInterest { get; set; }

    public virtual string Status { get; set; }
}
