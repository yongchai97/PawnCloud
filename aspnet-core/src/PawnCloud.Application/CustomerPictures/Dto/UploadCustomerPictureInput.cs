using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.CustomerPictures.Dto
{
    public class UploadCustomerPictureInput
    {
        [Required]
        public int Customer { get; set; }

        [Required]
        public IFormFile File { get; set; }

    }
}
