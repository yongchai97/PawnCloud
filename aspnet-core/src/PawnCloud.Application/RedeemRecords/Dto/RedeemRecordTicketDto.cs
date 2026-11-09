using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.RedeemRecords.Dto
{
    public class RedeemRecordTicketDto
    {
        public virtual int? PawnTicket { get; set; }
        public virtual int? RedeemRecord { get; set; }

    }
}
