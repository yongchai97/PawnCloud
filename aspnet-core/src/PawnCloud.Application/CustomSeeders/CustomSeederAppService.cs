using Abp.Application.Services;
using Abp.Domain.Repositories;
using PawnCloud.BasicCodes;
using PawnCloud.GoldTypes;
using PawnCloud.ItemStatuses;
using PawnCloud.MiscMasterConfigs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.CustomSeeders
{
    public class CustomSeederAppService : ApplicationService
    {
        private readonly IRepository<BasicCode, int> _BasicCode;
        private readonly IRepository<MiscMasterConfig, int> _MiscMasterConfig;
        private readonly IRepository<GoldType, int> _GoldType;
        private readonly IRepository<ItemStatus, int> _ItemStatus;

        public CustomSeederAppService(IRepository<BasicCode, int> BasicCode, IRepository<MiscMasterConfig, int> MiscMasterConfig, 
            IRepository<GoldType, int> GoldType, IRepository<ItemStatus, int> ItemStatus)
        {
            _BasicCode = BasicCode;
            _MiscMasterConfig = MiscMasterConfig;
            _GoldType = GoldType;
            _ItemStatus = ItemStatus;
        }
        public async Task SeedRootDb()
        {
            await SeedMiscMasterConfig();
            await SeedGoldType();
            await SeedItemStatus();
        }
        private async Task SeedItemStatus()
        {
            await AddItemStatusIfNotExist("3", "(ROSAK PATAH KURANG)");
            await AddItemStatusIfNotExist("4", "(ROSAK PATAH KURANG BENGKOK)");
            await AddItemStatusIfNotExist("5", "(ROSAK PATAH KURANG BENGKOK BASAH)");
            await AddItemStatusIfNotExist("B", "(BENGKOK)");
            await AddItemStatusIfNotExist("BBS", "(BENGKOK BASAH)");
            await AddItemStatusIfNotExist("BS", "(BASAH)");
            await AddItemStatusIfNotExist("CAL", "(CALAR)");
            await AddItemStatusIfNotExist("K", "(KURANG)");
            await AddItemStatusIfNotExist("KB", "(KURANG BENGKOK)");
            await AddItemStatusIfNotExist("KBS", "(KURANG BASAH)");
            await AddItemStatusIfNotExist("P", "(PATAH)");
            await AddItemStatusIfNotExist("PBS", "(PATAH BASAH)");
            await AddItemStatusIfNotExist("PK", "(PATAH KURANG)");
            await AddItemStatusIfNotExist("R", "(ROSAK)");
            await AddItemStatusIfNotExist("RB", "(ROSAK BENGKOK)");
            await AddItemStatusIfNotExist("RBS", "(ROSAK BASAH)");
            await AddItemStatusIfNotExist("RC", "(ROSAK CALAR)");
            await AddItemStatusIfNotExist("RK", "(ROSAK KURANG)");
            await AddItemStatusIfNotExist("RP", "(ROSAK PATAH)");
        }
        private async Task AddItemStatusIfNotExist(string code, string description)
        {
            var itemStatus = await _ItemStatus.FirstOrDefaultAsync(x => x.code == code);
            if (itemStatus == null) 
            {
                var newItemStatus = new ItemStatus
                {
                    code = code,
                    description = description
                };
                newItemStatus.TenantId = AbpSession.TenantId;
                await _ItemStatus.InsertAsync(newItemStatus);
            }
        }
        private async Task SeedGoldType()
        {
            await AddGoldTypeIfNotExist("375", "375", (decimal)37.5);
            await AddGoldTypeIfNotExist("585", "585", (decimal)58.5);
            await AddGoldTypeIfNotExist("750", "750", (decimal)75.0);
            await AddGoldTypeIfNotExist("800", "800", (decimal)80.0);
            await AddGoldTypeIfNotExist("835", "835", (decimal)83.5);
            await AddGoldTypeIfNotExist("875", "875", (decimal)87.5);
            await AddGoldTypeIfNotExist("900", "900", (decimal)90.0);
            await AddGoldTypeIfNotExist("916", "916", (decimal)91.6);
            await AddGoldTypeIfNotExist("950", "950", (decimal)95.0);
            await AddGoldTypeIfNotExist("965", "965", (decimal)96.5);
            await AddGoldTypeIfNotExist("999", "999", (decimal)99.9);
            await AddGoldTypeIfNotExist("G", "STAINLESS STEEL", 0);
            await AddGoldTypeIfNotExist("H", "YELLOW GOLD", 0);
            await AddGoldTypeIfNotExist("I", "WHITE GOLD", 0);
            await AddGoldTypeIfNotExist("J", "EVEROSE GOLD", 0);
            await AddGoldTypeIfNotExist("K", "PLATINUM", 0);
            await AddGoldTypeIfNotExist("L", "DIAMOND", 0);
            await AddGoldTypeIfNotExist("M", "SS DIAMOND", 0);
            await AddGoldTypeIfNotExist("N", "YG DIAMOND", 0);
            await AddGoldTypeIfNotExist("O", "WG DIAMOND", 0);
            await AddGoldTypeIfNotExist("P", "PT DIAMOND", 0);
            await AddGoldTypeIfNotExist("PP", "PAPER", 0);

        }
        private async Task AddGoldTypeIfNotExist(string purity, string description, decimal defaultPercentage)
        {
            var goldType = await _GoldType.FirstOrDefaultAsync(x => x.purity == purity);
            if (goldType == null) 
            {
                var newGoldType = new GoldType
                {
                    purity = purity,
                    description = description,
                    defaultPercentage = defaultPercentage,
                    active = true
                };
                newGoldType.TenantId = AbpSession.TenantId;
                await _GoldType.InsertAsync(newGoldType);
            }
        }
        private async Task SeedMiscMasterConfig()
        {
            //await AddMiscMasterConfigIfNotExist("NATIONALITY", false);
            //await AddMiscMasterConfigIfNotExist("COUNTRY", false);
            await AddMiscMasterConfigIfNotExist("RACE", false);
            await AddMiscMasterConfigIfNotExist("GENDER", false);

        }
        private async Task AddMiscMasterConfigIfNotExist(string category, bool availableForUser)
        {
            var miscMasterConfig = await _MiscMasterConfig.FirstOrDefaultAsync(x => x.category == category);
            if (miscMasterConfig == null) 
            {
                var newMiscMasterConfig = new MiscMasterConfig
                {
                    category = category,
                    availableForUser = availableForUser
                };
                newMiscMasterConfig.TenantId = AbpSession.TenantId;
                await _MiscMasterConfig.InsertAsync(newMiscMasterConfig);
            }
        }
    }
}
