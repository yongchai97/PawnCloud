using System;

namespace PawnCloud.AuditLogs.Dto;

public class AuditLogDto
{
    public long Id { get; set; }
    public int? TenantId { get; set; }
    public long? UserId { get; set; }
    public string UserName { get; set; }
    public string ServiceName { get; set; }
    public string MethodName { get; set; }
    public string Parameters { get; set; }
    public string ReturnValue { get; set; }
    public DateTime ExecutionTime { get; set; }
    public int ExecutionDuration { get; set; }
    public string ClientIpAddress { get; set; }
    public string ClientName { get; set; }
    public string BrowserInfo { get; set; }
    public string ExceptionMessage { get; set; }
}
