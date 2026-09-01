using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.Customers.Dto
{
    public class CreateOrEditCustomerOutletDto : EntityDto<int?>
    {
        public virtual int? Customer { get; set; }
        public virtual int? GeneralSetup { get; set; }
    }
}
