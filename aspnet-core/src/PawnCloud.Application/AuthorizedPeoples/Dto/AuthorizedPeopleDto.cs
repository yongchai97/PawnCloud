using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnCloud.AuthorizedPeoples.Dto
{
    public class AuthorizedPeopleDto
    {
        public virtual int? Customer { get; set; }
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
    }
}
