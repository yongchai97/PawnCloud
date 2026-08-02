using Abp.Application.Services.Dto;
using System;

namespace PawnCloud.Customers.Dto;

public class CustomerDocumentDto : EntityDto<int>
{
    public int? TenantId { get; set; }

    public int Customer { get; set; }

    public string DocumentType { get; set; }

    public string FileName { get; set; }

    public byte[] DocumentContents { get; set; }

    public DateTime CreationTime { get; set; }
}
