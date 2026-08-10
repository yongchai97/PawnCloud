using Abp.Application.Services.Dto;
using System;

namespace PawnCloud.Customers.Dto;

public class CreateOrEditCustomerDto : EntityDto<int>
{
    public virtual string CustomerNo { get; set; }

    public virtual string FullName { get; set; }

    public virtual string? NRIC { get; set; }

    public virtual string? PassportNo { get; set; }

    public virtual string? PhoneNo { get; set; }

    public virtual string? Email { get; set; }

    public virtual string? Address { get; set; }
    public virtual string? City { get; set; }

    public virtual string? State { get; set; }
    public virtual int? Country { get; set; } //getting data from basic code country
    public virtual string? mailingAddress { get; set; }
    public virtual string? mailingCity { get; set; }

    public virtual string? mailingState { get; set; }
    public virtual int? mailingCountry { get; set; } //getting data from basic code country
    public virtual int? Race { get; set; }//getting data from basic code nationality
    public virtual int? Gender { get; set; }//getting data from basic code nationality
    public bool member { get; set; }

    public virtual int? Nationality { get; set; }//getting data from basic code nationality
    public DateTime birthDate { get; set; }
    public decimal age { get; set; } //automatic calculated based on birthdate until current date
    public virtual string? oldIC { get; set; }
    public string telephoneNumber { get; set; }
    public string? cardID { get; set; }
    public string GSTNumber { get; set; }
    public string remark { get; set; }
    public bool blacklisted { get; set; } = false;

    public virtual string? Occupation { get; set; }
    public virtual string? Employer { get; set; }
    public virtual int? BusinessNature { get; set; } //getting data from basic code business nature
    public virtual int? MaritalStatus { get; set; } //getting data from basic code marital status
    public virtual decimal? MonthlyIncome { get; set; }
}
