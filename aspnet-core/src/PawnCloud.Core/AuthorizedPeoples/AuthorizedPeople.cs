using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using PawnCloud.Customers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.AuthorizedPeoples
{
    public class AuthorizedPeople : FullAuditedEntity<int>, IMayHaveTenant
    {
        public virtual int? TenantId { get; set; }
        public virtual int? Customer { get; set; }
        // optional navigation property
        [ForeignKey("Customer")]
        public virtual Customer CustomerFk { get; set; }
        public string NRIC { get; set; }
        public string fullName { get; set; }
        public string address { get; set; }
        public string town { get; set; }
        public string postcode { get; set; }
        public string state { get; set; }
        public virtual int? Country { get; set; } //getting data from country table
        public string telephoneNumber { get; set; }
        public string remark { get; set; }
        public string relationship { get; set; }
        public virtual int? Race { get; set; }//getting data from basic code race
        public virtual int? Gender { get; set; }//getting data from basic code gender
        public bool member { get; set; }
        public virtual int? Nationality { get; set; }//getting data from country table
        public DateTime birthDate { get; set; }
        public decimal age { get; set; } //automatic calculated based on birthdate until current date
        public bool blacklisted { get; set; } 
        public string oldIC { get; set; }

        public AuthorizedPeople() { }
        public AuthorizedPeople(int? customer, string nric, string fullName, string address, string town, string postcode, 
            string state, int? country, string telephoneNumber, string remark, string relationship, int? race, int? gender, 
            bool member, int? nationality, DateTime birthDate, decimal age, bool blacklisted, string oldIC)
        {
            Customer = customer;
            NRIC = nric;
            this.fullName = fullName;
            this.address = address;
            this.town = town;
            this.postcode = postcode;
            this.state = state;
            Country = country;
            this.telephoneNumber = telephoneNumber;
            this.remark = remark;
            this.relationship = relationship;
            Race = race;
            Gender = gender;
            this.member = member;
            Nationality = nationality;
            this.birthDate = birthDate;
            this.age = age;
            this.blacklisted = blacklisted;
            this.oldIC = oldIC;
        }
    }
}
