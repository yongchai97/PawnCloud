using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PawnCloud.Misc;

public class MiscFunctionAppService : ApplicationService, IMiscFunctionAppService
{
    public Task<ListResultDto<StatusDto>> GetStatuses()
    {
        var list = new List<StatusDto>
        {
            new StatusDto { Name = "Pending", Value = "Pending" },
            new StatusDto { Name = "Active", Value = "Active" },
            new StatusDto { Name = "Redeemed", Value = "Redeemed" },
            new StatusDto { Name = "Renewed", Value = "Renewed" },
            new StatusDto { Name = "Auction", Value = "Auction" },
            new StatusDto { Name = "Closed", Value = "Closed" },
            new StatusDto { Name = "Cancelled", Value = "Cancelled" },
        };

        return Task.FromResult(new ListResultDto<StatusDto>(list));
    }
}
