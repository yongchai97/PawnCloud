using Abp.Application.Services.Dto;

namespace PawnCloud.BasicCodes.Dto;

public class CreateOrEditBasicCodeDto : EntityDto<int>
{
    public string CodeName { get; set; }
    public string CodeDescription { get; set; }
    public bool SystemProvidedValue { get; set; }
    public int? MiscMasterConfig { get; set; }
}
