using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.GlobalBlackLists.Dto
{
    public class CreateOrEditGlobalBlackListRequestDto : Entity<int?>
    {
        public string FullName { get; set; }
        public string banReason { get; set; }
        public string status { get; set; } //pending, approved, rejected

        public CreateOrEditGlobalBlackListRequestDto() { }
        public CreateOrEditGlobalBlackListRequestDto(int? id, string fullName, string banReason, string status)
        {
            Id = id;
            FullName = fullName;
            this.banReason = banReason;
            this.status = status;
        }
    }
}
