using Abp.Application.Services.Dto;

namespace PawnCloud.BasicCodes.Dto;

public class PagedBasicCodeResultRequestDto : PagedResultRequestDto
{
    public string Filter { get; set; }
}
