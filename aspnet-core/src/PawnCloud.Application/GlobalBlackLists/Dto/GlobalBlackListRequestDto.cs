using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.GlobalBlackLists.Dto
{
    public class GlobalBlackListRequestDto
    {
        public string FullName { get; set; }
        public string banReason { get; set; }
        public string status { get; set; } //pending, approved, rejected

    }
}
