using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using PawnCloud.Authorization;
using PawnCloud.CustomerCDDs;
using PawnCloud.DailyGoldPrices;
using PawnCloud.GeneralSetups;
using PawnCloud.GoldTypes;
using PawnCloud.PawnItems;
using PawnCloud.PawnRequestPermissions.Dto;
using PawnCloud.PawnTicketPayments;
using PawnCloud.PawnTickets;
using PawnCloud.RedeemRecords;
using PawnCloud.RedeemRecordTickets;
using PawnCloud.SharedFunctions;
using PawnCloud.UserOutlets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.PawnRequestPermissions
{
    [AbpAuthorize(PermissionNames.Pages_PawnRequestPermissions)]
    public class PawnRequestPermissionAppService : ApplicationService
    {
        private readonly IRepository<PawnRequestPermission> _PawnRequestPermission;
        private readonly IRepository<PawnTicket> _PawnTicket;
        private readonly IRepository<PawnItem> _PawnItem;
        private readonly IRepository<PawnTicketPayment> _PawnTicketPayment;
        private readonly IRepository<PawnTicketDocument> _PawnTicketDocument;

        public PawnRequestPermissionAppService(
            IRepository<PawnRequestPermission> pawnRequestPermissionRepository, IRepository<PawnTicket> pawnTicket, IRepository<PawnItem> pawnItem, IRepository<PawnTicketPayment> pawnTicketPayment, IRepository<PawnTicketDocument> pawnTicketDocument)
        {
            _PawnRequestPermission = pawnRequestPermissionRepository;
            _PawnTicket = pawnTicket;
            _PawnItem = pawnItem;
            _PawnTicketPayment = pawnTicketPayment;
            _PawnTicketDocument = pawnTicketDocument;
        }

        public async Task<List<PawnRequestPermission>> GetAllPawnRequestPermissions()
        {
            var pawnRequestPermissions = await _PawnRequestPermission.GetAllListAsync();
            return pawnRequestPermissions;
        }
        public async Task<PawnRequestPermission> GetPawnRequestPermissionById(int id)
        {
            var pawnRequestPermission = await _PawnRequestPermission.FirstOrDefaultAsync(id);
            return pawnRequestPermission;
        }

        public async Task ApproveOrRejectRequest(CreateOrEditPawnRequestPermissionDto input)
        {
            var pawnRequestPermission = await _PawnRequestPermission.FirstOrDefaultAsync(x=>x.Id == (int)input.Id && (x.approvalStatus == "PENDING"));
            if(pawnRequestPermission == null)
            {
                return;
            }
            pawnRequestPermission.approvalStatus = input.approvalStatus;
            pawnRequestPermission.approvedBy = (int)AbpSession.UserId;
            await _PawnRequestPermission.UpdateAsync(pawnRequestPermission);
            if(pawnRequestPermission.approvalStatus == "APPROVED")
            {
                var designatedPawnTicket = await _PawnTicket.FirstOrDefaultAsync(x => x.TicketNo == pawnRequestPermission.TicketNo);
                if(designatedPawnTicket == null)
                {
                    return;
                }
                switch (pawnRequestPermission.request)
                {
                    case "DELETE":
                        await _PawnTicketDocument.DeleteAsync(x => x.PawnTicket == designatedPawnTicket.Id);
                        await _PawnTicketPayment.DeleteAsync(x => x.PawnTicket == designatedPawnTicket.Id);
                        await _PawnItem.DeleteAsync(x => x.PawnTicket == designatedPawnTicket.Id);
                        await _PawnTicket.DeleteAsync(x => x.Id == designatedPawnTicket.Id);
                        break;
                    case "CHANGENUMBER":
                        designatedPawnTicket.TicketNo = input.newTicketNo;
                        await _PawnTicket.UpdateAsync(designatedPawnTicket);
                        break;
                    default:
                        break;
                }
            }
        }
        public async Task DeletePawnRequestPermission(int id)
        {
            var pawnRequestPermission = await _PawnRequestPermission.FirstOrDefaultAsync(id);
            if (pawnRequestPermission == null ||
                string.Equals(pawnRequestPermission.approvalStatus, "APPROVED", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(pawnRequestPermission.approvalStatus, "REJECTED", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            await _PawnRequestPermission.DeleteAsync(pawnRequestPermission);
        }

    }
}
