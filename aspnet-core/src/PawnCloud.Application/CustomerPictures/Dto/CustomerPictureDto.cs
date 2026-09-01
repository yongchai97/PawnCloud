using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.CustomerPictures.Dto
{
    public class CustomerPictureDto : EntityDto<int?>
    {
        public string OriginalFileName { get; set; }

        public string ContentType { get; set; }

        public long FileSize { get; set; }

        public string BlobName { get; set; }
        public virtual int? Customer { get; set; }

    }
}
