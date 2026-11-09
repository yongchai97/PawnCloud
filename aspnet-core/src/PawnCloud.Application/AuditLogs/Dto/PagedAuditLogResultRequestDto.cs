using Abp.Application.Services.Dto;

namespace PawnCloud.AuditLogs.Dto;

public class PagedAuditLogResultRequestDto : PagedResultRequestDto
{
    public string Filter { get; set; }
}
