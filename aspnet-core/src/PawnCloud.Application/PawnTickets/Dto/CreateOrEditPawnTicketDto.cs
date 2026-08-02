using Abp.Application.Services.Dto;
using System;

namespace PawnCloud.PawnTickets.Dto;

public class CreateOrEditPawnTicketDto : EntityDto<int>
{
    public string TicketNo { get; set; }

    public int? BranchId { get; set; }

    public int CustomerId { get; set; }

    public string Status { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? MaturityDate { get; set; }

    public DateTime? ExpiryDate { get; set; }

    public string Remarks { get; set; }
}
