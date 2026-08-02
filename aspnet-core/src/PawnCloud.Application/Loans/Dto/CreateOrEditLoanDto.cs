using Abp.Application.Services.Dto;
using System;

namespace PawnCloud.Loans.Dto;

public class CreateOrEditLoanDto : EntityDto<int>
{
    public int PawnTicketId { get; set; }

    public decimal Principal { get; set; }

    public decimal InterestRate { get; set; }

    public string InterestType { get; set; }

    public int LoanPeriod { get; set; }

    public DateTime InterestStartDate { get; set; }

    public DateTime MaturityDate { get; set; }

    public decimal OutstandingPrincipal { get; set; }

    public decimal OutstandingInterest { get; set; }

    public string Status { get; set; }
}
