using Abp.Application.Services.Dto;
using System;

namespace PawnCloud.PawnTickets.Dto;

public class CreateOrEditPawnTicketDto : EntityDto<int>
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

}
