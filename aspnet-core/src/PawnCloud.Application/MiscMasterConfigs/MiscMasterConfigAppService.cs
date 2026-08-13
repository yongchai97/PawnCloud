using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PawnCloud.MiscMasterConfigs.Dto;

namespace PawnCloud.MiscMasterConfigs;

public class MiscMasterConfigAppService : ApplicationService, IMiscMasterConfigAppService
{
    private readonly IRepository<MiscMasterConfig, int> _repository;

    public MiscMasterConfigAppService(IRepository<MiscMasterConfig, int> repository)
    {
        _repository = repository;
    }

    public async Task<ListResultDto<MiscMasterConfigLookupDto>> GetForLookup()
    {
        var configs = await _repository.GetAllListAsync();
        var lookup = configs
            .Where(c => c.availableForUser)
            .OrderBy(c => c.category)
            .Select(c => new MiscMasterConfigLookupDto
            {
                Id = c.Id,
                DisplayName = c.category
            })
            .ToList();
        return new ListResultDto<MiscMasterConfigLookupDto>(lookup);
    }
}
