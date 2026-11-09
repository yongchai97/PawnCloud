using PawnCloud.PawnTickets.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.RedeemRecords.Dto
{
    public class RedeemRecordConsolidated
    {
        public CreateOrEditRedeemRecordDto createOrEditRedeemRecordDto { get; set; } = new CreateOrEditRedeemRecordDto();
        public List<CreateOrEditRedeemRecordTicketDto> createOrEditRedeemRecordTicketDtos { get; set; } = new List<CreateOrEditRedeemRecordTicketDto>();
        public List<CreateOrEditPawnTicketDto> createOrEditPawnTicketDtos { get; set; } = new List<CreateOrEditPawnTicketDto>();
    }
}
