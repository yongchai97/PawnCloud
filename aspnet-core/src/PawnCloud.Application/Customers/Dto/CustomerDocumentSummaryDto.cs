using System;

namespace PawnCloud.Customers.Dto;

public class CustomerDocumentSummaryDto
{
    public int Id { get; set; }
    public int Customer { get; set; }
    public string DocumentType { get; set; }
    public string FileName { get; set; }
    public DateTime CreationTime { get; set; }
}