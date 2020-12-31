using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class Employee
    {
        public Employee()
        {
            PayrollEmployee = new HashSet<PayrollEmployee>();
        }

        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int GenderId { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public int MaritalStatusId { get; set; }
        public int SalaryTypeId { get; set; }
        public bool IsResidentEmployee { get; set; }
        public string IdorPassportNumber { get; set; }
        public string DocumentUrl { get; set; }
        public string OrganisationId { get; set; }
        public decimal Salary { get; set; }
        public int NumberOfChild { get; set; }
        public bool IsActive { get; set; }
        public string JobTitle { get; set; }
        public int CountryId { get; set; }
        public bool IsConfederationThatHosts { get; set; }

        public virtual Country Country { get; set; }
        public virtual Gender Gender { get; set; }
        public virtual MaritalStatus MaritalStatus { get; set; }
        public virtual Organisation Organisation { get; set; }
        public virtual SalaryType SalaryType { get; set; }
        public virtual ICollection<PayrollEmployee> PayrollEmployee { get; set; }
    }
}
