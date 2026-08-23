using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using PawnCloud.Authorization;
using PawnCloud.Customers.Dto;
using PawnCloud.GoldTypes.Dto;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PawnCloud.GoldTypes
{
    public class GoldTypeAppService : ApplicationService
    {
        private readonly IRepository<GoldType, int> _repository;

        public GoldTypeAppService(IRepository<GoldType, int> repository)
        {
            _repository = repository;
        }
        public async Task<List<GoldType>> GetAll()
        {
            var goldTypes = await _repository.GetAllListAsync();
            return goldTypes;
        }
    }
}
