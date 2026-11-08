using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.GlobalBlackLists.Dto
{
    public class CreateOrEditGlobalBlackListDto : Entity<int?>
    {
        public string FullName { get; set; }
        public string banReason { get; set; }
        public CreateOrEditGlobalBlackListDto() { }
        public CreateOrEditGlobalBlackListDto(int? id, string fullName, string banReason)
        {
            Id = id;
            FullName = fullName;
            this.banReason = banReason;
        }

    }
}
