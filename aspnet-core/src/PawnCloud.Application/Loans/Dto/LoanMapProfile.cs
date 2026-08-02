using AutoMapper;
using PawnCloud.Loans;

namespace PawnCloud.Loans.Dto;

public class LoanMapProfile : Profile
{
    public LoanMapProfile()
    {
        CreateMap<Loan, LoanDto>();
        CreateMap<CreateOrEditLoanDto, Loan>();
        CreateMap<Loan, CreateOrEditLoanDto>();
    }
}
