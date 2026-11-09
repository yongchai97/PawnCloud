using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using PawnCloud.Authorization;
using PawnCloud.Countries;
using PawnCloud.UserOutlets.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.UserOutlets
{
    [AbpAuthorize(PermissionNames.Pages_Users)]
    public class UserOutletAppService : ApplicationService
    {
        private readonly IRepository<UserOutlet> _repository;

        public UserOutletAppService(IRepository<UserOutlet> repository)
        {
            _repository = repository;
        }
        public async Task<List<UserOutlet>> GetAll()
        {
            var useroutlets = await _repository.GetAllListAsync();
            return useroutlets;
        }
        public async Task<List<UserOutlet>> GetAllViaUserId(int? userId)
        {
            var useroutlets = await _repository.GetAll()
                .Include(x => x.GeneralSetupFk)
                .Where(x => x.User == userId)
                .ToListAsync();
            return useroutlets;
        }
        public async Task<UserOutlet> ReturnUserOutletRecordViaId(EntityDto input)
        {
            var useroutlet = await _repository.FirstOrDefaultAsync(x => x.Id == input.Id);
            return useroutlet;
        }
        public async Task CreateUserOutlet(CreateOrEditUserOutletDto input)
        {
            var useroutlet = new UserOutlet
            {
                User = input.User,
                GeneralSetup = input.GeneralSetup
            };
            useroutlet.TenantId = AbpSession.TenantId;
            await _repository.InsertAsync(useroutlet);
        }
        public async Task DeleteUserOutlet(EntityDto input)
        {
            var useroutlet = await _repository.FirstOrDefaultAsync(x => x.Id == input.Id);
            if (useroutlet != null)
            {
                await _repository.DeleteAsync(useroutlet);
            }
        }
    }
}
