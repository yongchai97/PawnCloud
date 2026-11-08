using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.PrivateBlackLists.Dto
{
    public class CreateOrEditPrivateBlackListDto : EntityDto<int?>
    {
        public string FullName { get; set; }
        public string banReason { get; set; }

        public CreateOrEditPrivateBlackListDto() { }
        public CreateOrEditPrivateBlackListDto(int? id, string fullName, string banReason)
        {
            Id = id;
            FullName = fullName;
            this.banReason = banReason;
        }
    }
}
