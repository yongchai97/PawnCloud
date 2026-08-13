using Abp.Application.Services.Dto;

namespace PawnCloud.BasicCodes.Dto;

public class BasicCodeDto : EntityDto<int>
{
    public int? TenantId { get; set; }
    public string CodeName { get; set; }
    public string CodeDescription { get; set; }
    public bool SystemProvidedValue { get; set; }
    public int? MiscMasterConfig { get; set; }
}
