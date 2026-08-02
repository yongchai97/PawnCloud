using AutoMapper;
using PawnCloud.Customers;

namespace PawnCloud.Customers.Dto;

public class CustomerMapProfile : Profile
{
    public CustomerMapProfile()
    {
        CreateMap<Customer, CustomerDto>();
        CreateMap<CreateOrEditCustomerDto, Customer>();
        CreateMap<CustomerDto, Customer>();
        CreateMap<Customer, CreateOrEditCustomerDto>();
    }
}
