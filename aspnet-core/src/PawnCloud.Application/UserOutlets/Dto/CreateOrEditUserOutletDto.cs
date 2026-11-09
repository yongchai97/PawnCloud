using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.UserOutlets.Dto
{
    public class CreateOrEditUserOutletDto : EntityDto<int?>
    {
        public virtual int? User { get; set; }
        public virtual int? GeneralSetup { get; set; }

        public CreateOrEditUserOutletDto() { }
        public CreateOrEditUserOutletDto(int? id, int? user, int? generalSetup)
        {
            Id = id;
            User = user;
            GeneralSetup = generalSetup;
        }
    }
}
