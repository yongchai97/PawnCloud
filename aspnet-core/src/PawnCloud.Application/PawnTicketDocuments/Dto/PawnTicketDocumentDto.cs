using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.PawnTicketDocuments.Dto
{
    public class PawnTicketDocumentDto
    {
        public string OriginalFileName { get; set; }
        public string ContentType { get; set; }
        public long FileSize { get; set; }
        public string BlobName { get; set; }
        public virtual int? PawnTicket { get; set; }
    }
}
