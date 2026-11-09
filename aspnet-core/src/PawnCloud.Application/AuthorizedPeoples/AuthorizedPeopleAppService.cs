using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using PawnCloud.Authorization;
using PawnCloud.AuthorizedPeoples.Dto;
using PawnCloud.BasicCodes;
using PawnCloud.PrivateBlackLists;
using PawnCloud.PrivateBlackLists.Dto;
using PawnCloud.RedeemRecords;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.AuthorizedPeoples
{
    public class AuthorizedPeopleAppService : ApplicationService
    {
        private readonly IRepository<AuthorizedPeople> _repository;
        private readonly IRepository<RedeemRecord> _RedeemRecord;

        public AuthorizedPeopleAppService(IRepository<AuthorizedPeople> repository, IRepository<RedeemRecord> RedeemRecord)
        {
            _repository = repository;
            _RedeemRecord = RedeemRecord;
        }

        public async Task<List<AuthorizedPeople>> GetAll()
        {
            var listings = await _repository.GetAllListAsync();
            return listings;
        }
        public async Task<AuthorizedPeople> GetById(int id)
        {
            var listing = await _repository.FirstOrDefaultAsync(id);
            return listing;
        }
        public async Task CreateOrEdit(CreateOrEditAuthorizedPeopleDto input)
        {

            if (input.Id == null)
            {
                await Create(input);
            }
            else
            {
                await Edit(input);
            }
        }
        public async Task Delete(EntityDto input)
        {
            var listing = await _repository.FirstOrDefaultAsync(input.Id);
            if(listing == null)
            {
                throw new UserFriendlyException("Record not found");
            }
            var redeemCheck = await _RedeemRecord.FirstOrDefaultAsync(x=>x.redeemedBy == listing.NRIC);
            if(redeemCheck != null)
            {
                throw new UserFriendlyException("Cannot Delete record that has been used for redeem operation");
            }
            await _repository.DeleteAsync(input.Id);
        }
        private async Task Create(CreateOrEditAuthorizedPeopleDto input)
        {
            //prevent duplicate
            var listingChecker = await _repository.FirstOrDefaultAsync(x => (x.NRIC == input.NRIC || x.fullName == input.fullName)
            &&x.Customer == input.Customer);
            if (listingChecker != null)
            {
                return;
            }
            var listing = new AuthorizedPeople(input.Customer,input.NRIC,input.fullName,input.address,input.town,input.postcode,
                input.state,input.Country,input.telephoneNumber,input.remark,input.relationship,input.Race,
                input.Gender,input.member,input.Nationality,input.birthDate,input.age,input.blacklisted,input.oldIC);
            listing.TenantId = AbpSession.TenantId;
            await _repository.InsertAsync(listing);
        }
        private async Task Edit(CreateOrEditAuthorizedPeopleDto input)
        {
            var allAuthorizedPeople = await _repository.GetAllListAsync();
            var listing = allAuthorizedPeople.FirstOrDefault(x=>x.Id == (int)input.Id);
            if (listing != null)
            {
                var listingChecker = allAuthorizedPeople.Where(x=>x.Id != input.Id && x.Customer == input.Customer
                && (x.NRIC == input.NRIC || x.fullName == input.fullName)).FirstOrDefault();
                if(listingChecker != null)
                {
                    return;
                }
                listing.Customer = input.Customer;
                listing.NRIC = input.NRIC;
                listing.fullName = input.fullName;
                listing.address = input.address;
                listing.town = input.town;
                listing.postcode = input.postcode;
                listing.state = input.state;
                listing.Country = input.Country;
                listing.telephoneNumber = input.telephoneNumber;
                listing.remark = input.remark;
                listing.relationship = input.relationship;
                listing.Race = input.Race;

                listing.Gender = input.Gender;
                listing.member = input.member;
                listing.Nationality = input.Nationality;
                listing.birthDate = input.birthDate;
                listing.age = input.age;
                listing.blacklisted = input.blacklisted;
                listing.oldIC = input.oldIC;

                await _repository.UpdateAsync(listing);
            }
        }
    }
}
