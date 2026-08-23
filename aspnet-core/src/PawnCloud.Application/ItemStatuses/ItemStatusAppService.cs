using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using PawnCloud.Authorization;
using PawnCloud.Customers.Dto;
using PawnCloud.GoldTypes;
using PawnCloud.GoldTypes.Dto;
using PawnCloud.ItemStatuses.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.ItemStatuses
{
    public class ItemStatusAppService : ApplicationService
    {
        private readonly IRepository<ItemStatus, int> _repository;

        public ItemStatusAppService(IRepository<ItemStatus, int> repository)
        {
            _repository = repository;
        }
        public async Task<List<ItemStatus>> GetAll()
        {
            var itemStatuses = await _repository.GetAllListAsync();
            return itemStatuses;
        }
    }
}
