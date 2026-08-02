using AutoMapper;
using PawnCloud.Customers;

namespace PawnCloud.Customers.Dto;

public class CustomerDocumentMapProfile : Profile
{
    public CustomerDocumentMapProfile()
    {
        CreateMap<CustomerDocument, CustomerDocumentDto>();
        CreateMap<CreateOrEditCustomerDocumentDto, CustomerDocument>();
        CreateMap<CustomerDocument, CreateOrEditCustomerDocumentDto>();
    }
}
