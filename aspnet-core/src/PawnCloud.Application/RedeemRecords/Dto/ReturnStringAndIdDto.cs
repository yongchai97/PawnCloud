using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.RedeemRecords.Dto
{
    public class ReturnStringAndIdDto
    {
        public virtual int? Id { get; set; }
        public string message { get; set; }
    }
}
