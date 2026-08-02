using Abp.Application.Services.Dto;

namespace PawnCloud.Customers.Dto;

public class CreateOrEditCustomerDocumentDto : EntityDto<int>
{
    public int Customer { get; set; }

    public string DocumentType { get; set; }

    public string FileName { get; set; }

    public byte[] DocumentContents { get; set; }
}
