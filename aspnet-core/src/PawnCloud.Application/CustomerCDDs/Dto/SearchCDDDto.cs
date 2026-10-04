using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.CustomerCDDs.Dto
{
    public class SearchCDDDto
    {
        public string customerName { get; set; }
        public CreateOrEditCustomerCDDDto CreateOrEditCustomerCDDDto { get; set; }
    }
}
