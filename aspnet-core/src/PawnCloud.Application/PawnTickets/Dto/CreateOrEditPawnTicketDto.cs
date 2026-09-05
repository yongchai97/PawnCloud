using Abp.Application.Services.Dto;
using System;

namespace PawnCloud.PawnTickets.Dto;

public class CreateOrEditPawnTicketDto : EntityDto<int?>
{
    public string TicketNo { get; set; }
    public int? Customer { get; set; }
    public decimal weight { get; set; }
    public decimal value { get; set; }
    public DateTime pledgedDate { get; set; }
    public DateTime expiryDate { get; set; }
    public decimal amount { get; set; }
    public decimal monthlyCustody { get; set; }
    public decimal serviceCharge { get; set; }
    public string slotNumber { get; set; }
    public virtual int? GeneralSetup { get; set; }
    public CreateOrEditPawnTicketDto() { }
    public CreateOrEditPawnTicketDto(int? id, string ticketNo, int? customer, decimal weight, decimal value,
        DateTime pledgedDate, DateTime expiryDate, decimal amount, decimal monthlyCustody,
        decimal serviceCharge, string slotNumber, int? generalSetup)
    {
        Id = id;
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
