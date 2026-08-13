using AutoMapper;
using PawnCloud.MiscMasterConfigs;

namespace PawnCloud.MiscMasterConfigs.Dto;

public class MiscMasterConfigMapProfile : Profile
{
    public MiscMasterConfigMapProfile()
    {
        CreateMap<MiscMasterConfig, MiscMasterConfigDto>()
            .ForMember(d => d.Category, opt => opt.MapFrom(s => s.category))
            .ForMember(d => d.AvailableForUser, opt => opt.MapFrom(s => s.availableForUser));
        
        CreateMap<MiscMasterConfigDto, MiscMasterConfig>()
            .ForMember(d => d.category, opt => opt.MapFrom(s => s.Category))
            .ForMember(d => d.availableForUser, opt => opt.MapFrom(s => s.AvailableForUser));
    }
}
