using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.PawnTicketDocuments.Dto
{
    public class UploadPawnTicketDocumentInput
    {
        [Required]
        public int PawnTicket { get; set; }

        [Required]
        public IFormFile File { get; set; }

    }
}
