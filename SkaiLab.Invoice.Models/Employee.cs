using System;
namespace SkaiLab.Invoice.Models
{
    public class Employee
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int GenderId { get; set; }
        public Gender Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public int MaritalStatusId { get; set; }
        public MaritalStatus MaritalStatus { get; set; }
        public int SalaryTypeId { get; set; }
        public SalaryType SalaryType { get; set; }
        public bool IsResidentEmployee { get; set; }
        public string IDOrPassportNumber { get; set; }
        public string DocumentUrl { get; set; }
        public string OrganisationId { get; set; }
        public decimal Salary { get; set; }
        public int NumberOfChild { get; set; }
        public int CurrencyId { get; set; }
        public Currency Currency { get; set; }
        public bool IsActive { get; set; }
        public string JobTitle { get; set; }
        public int CountryId { get; set; }
        public Country Country { get; set; }
        public bool IsConfederationThatHosts { get; set; }
    }
}
