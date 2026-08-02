using AutoMapper;
using PawnCloud.PawnTickets;

namespace PawnCloud.PawnTickets.Dto;

public class PawnTicketMapProfile : Profile
{
    public PawnTicketMapProfile()
    {
        CreateMap<PawnTicket, PawnTicketDto>();
        CreateMap<CreateOrEditPawnTicketDto, PawnTicket>();
        CreateMap<PawnTicket, CreateOrEditPawnTicketDto>();
    }
}
