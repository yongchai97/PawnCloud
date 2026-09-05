using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using PawnCloud.Customers;
using PawnCloud.Customers;
using PawnCloud.GeneralSetups;
using PawnCloud.GoldTypes;
using PawnCloud.ItemListings;  // Add this line
using PawnCloud.ItemStatuses;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PawnCloud.PawnTickets;

public class PawnTicket : FullAuditedEntity<int>, IMayHaveTenant
{
    public virtual int? TenantId { get; set; }

    public virtual string TicketNo { get; set; }

    public virtual int? Customer { get; set; }
    [ForeignKey("Customer")]

    public Customer CustomerFk { get; set; }

    public decimal weight { get; set; }
    public decimal value { get; set; }
    public DateTime pledgedDate { get; set; }
    public DateTime expiryDate { get; set; }
    public decimal amount { get; set; }
    public decimal monthlyCustody { get; set; }
    public decimal serviceCharge { get; set; }
    public string slotNumber { get; set; }
    public virtual int? GeneralSetup { get; set; }
    [ForeignKey("GeneralSetup")]

    public GeneralSetup GeneralSetupFk { get; set; }

    public PawnTicket() { }
    public PawnTicket(string ticketNo, int? customer, decimal weight, decimal value, 
        DateTime pledgedDate, DateTime expiryDate, decimal amount, decimal monthlyCustody, 
        decimal serviceCharge, string slotNumber, int? generalSetup)
    {
        TicketNo = ticketNo;
        Customer = customer;
        this.weight = weight;
        this.value = value;
        this.pledgedDate = pledgedDate;
        this.expiryDate = expiryDate;
        this.amount = amount;
        this.monthlyCustody = monthlyCustody;
        this.serviceCharge = serviceCharge;
        this.slotNumber = slotNumber;
        GeneralSetup = generalSetup;
    }
}
