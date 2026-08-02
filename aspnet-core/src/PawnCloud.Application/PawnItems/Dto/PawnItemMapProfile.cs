using AutoMapper;
using PawnCloud.PawnItems;

namespace PawnCloud.PawnItems.Dto;

public class PawnItemMapProfile : Profile
{
    public PawnItemMapProfile()
    {
        CreateMap<PawnItem, PawnItemDto>();
        CreateMap<CreateOrEditPawnItemDto, PawnItem>();
        CreateMap<PawnItem, CreateOrEditPawnItemDto>();
    }
}
