using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.RedeemRecords.Dto
{
    public class RedeemRecordDto
    {
        public string redeemedBy { get; set; }
        public DateTime redeemedDate { get; set; }
        public int numberOfTickets { get; set; }
        public decimal totalAmount { get; set; }
        public decimal totalCustody { get; set; }
        public decimal totalService { get; set; }
        public decimal grandTotal { get; set; }

    }
}
