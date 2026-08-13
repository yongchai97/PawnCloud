using Abp.Application.Services.Dto;

namespace PawnCloud.MiscMasterConfigs.Dto;

public class MiscMasterConfigDto : EntityDto<int>
{
    public int? TenantId { get; set; }
    public string Category { get; set; }
    public bool AvailableForUser { get; set; }
}
