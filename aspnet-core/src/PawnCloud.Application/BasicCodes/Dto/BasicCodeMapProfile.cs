using AutoMapper;
using PawnCloud.BasicCodes;

namespace PawnCloud.BasicCodes.Dto;

public class BasicCodeMapProfile : Profile
{
    public BasicCodeMapProfile()
    {
        CreateMap<BasicCode, BasicCodeDto>()
            .ForMember(d => d.CodeName, opt => opt.MapFrom(s => s.codeName))
            .ForMember(d => d.CodeDescription, opt => opt.MapFrom(s => s.codeDescription))
            .ForMember(d => d.SystemProvidedValue, opt => opt.MapFrom(s => s.systemProvidedValue));
        
        CreateMap<CreateOrEditBasicCodeDto, BasicCode>()
            .ForMember(d => d.codeName, opt => opt.MapFrom(s => s.CodeName))
            .ForMember(d => d.codeDescription, opt => opt.MapFrom(s => s.CodeDescription))
            .ForMember(d => d.systemProvidedValue, opt => opt.MapFrom(s => s.SystemProvidedValue));
        
        CreateMap<BasicCodeDto, BasicCode>()
            .ForMember(d => d.codeName, opt => opt.MapFrom(s => s.CodeName))
            .ForMember(d => d.codeDescription, opt => opt.MapFrom(s => s.CodeDescription))
            .ForMember(d => d.systemProvidedValue, opt => opt.MapFrom(s => s.SystemProvidedValue));
        
        CreateMap<BasicCode, CreateOrEditBasicCodeDto>()
            .ForMember(d => d.CodeName, opt => opt.MapFrom(s => s.codeName))
            .ForMember(d => d.CodeDescription, opt => opt.MapFrom(s => s.codeDescription))
            .ForMember(d => d.SystemProvidedValue, opt => opt.MapFrom(s => s.systemProvidedValue));
    }
}
