using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.RedeemRecords.Dto
{
    public class CreateOrEditRedeemRecordTicketDto : EntityDto<int?>
    {
        public virtual int? PawnTicket { get; set; }
        public virtual int? RedeemRecord { get; set; }
        public CreateOrEditRedeemRecordTicketDto() { }
        public CreateOrEditRedeemRecordTicketDto(int? id, int? PawnTicket, int? RedeemRecord)
        {
            this.Id = id;
            this.PawnTicket = PawnTicket;
            this.RedeemRecord = RedeemRecord;
        }

    }
}
