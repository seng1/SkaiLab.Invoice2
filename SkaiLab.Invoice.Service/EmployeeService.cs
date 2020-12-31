using SkaiLab.Invoice.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SkaiLab.Invoice.Service
{
    public class EmployeeService:Service,IEmployeeService
    {
        public EmployeeService(IDataContext context) : base(context)
        {

        }

        public void Add(Employee employee)
        {
            using var context = Context();
            var newEmployee = new Dal.Models.Employee
            {
                Address=employee.Address,
                DateOfBirth=employee.DateOfBirth,
                DisplayName=employee.DisplayName,
                DocumentUrl=employee.DocumentUrl,
                Email=employee.Email,
                FirstName=employee.FirstName,
                GenderId=employee.GenderId,
                IdorPassportNumber=employee.IDOrPassportNumber,
                IsResidentEmployee=employee.IsResidentEmployee,
                LastName=employee.LastName,
                MaritalStatusId=employee.MaritalStatusId,
                OrganisationId=employee.OrganisationId,
                PhoneNumber=employee.PhoneNumber,
                SalaryTypeId=employee.SalaryTypeId,
                Salary=employee.Salary,
                NumberOfChild=employee.NumberOfChild,
                IsActive=employee.IsActive,
                JobTitle=employee.JobTitle,
                CountryId=employee.CountryId,
                IsConfederationThatHosts=employee.IsConfederationThatHosts
                
            };
            context.Employee.Add(newEmployee);
            context.SaveChanges();
            employee.Id = newEmployee.Id;
        }

        public Employee Get(long id, string organisationId)
        {
            using var context = Context();
            var employee = context.Employee.FirstOrDefault(u => u.Id == id && u.OrganisationId == organisationId);
            if (employee == null)
            {
                throw new Exception("Employee not found");
            }
            return new Employee
            {
                OrganisationId = employee.OrganisationId,
                Address = employee.Address,
                DateOfBirth = employee.DateOfBirth,
                DocumentUrl = employee.DocumentUrl,
                Email = employee.Email,
                FirstName = employee.FirstName,
                Gender = new Gender
                {
                    Id = employee.GenderId,
                    Name = employee.Gender.Name
                },
                GenderId=employee.GenderId,
                Id=employee.Id,
                IDOrPassportNumber=employee.IdorPassportNumber,
                IsResidentEmployee=employee.IsResidentEmployee,
                LastName=employee.LastName,
                MaritalStatus=new MaritalStatus
                {
                    Id=employee.MaritalStatusId,
                    Name=employee.MaritalStatus.Name
                },
                DisplayName=employee.DisplayName,
                MaritalStatusId=employee.MaritalStatusId,
                PhoneNumber=employee.PhoneNumber,
                SalaryType=new SalaryType
                {
                    Name=employee.SalaryType.Name,
                    Id=employee.SalaryTypeId
                },
                SalaryTypeId=employee.SalaryTypeId,
                NumberOfChild=employee.NumberOfChild,
                Salary=employee.Salary,
                CurrencyId=employee.Organisation.OrganisationBaseCurrency.BaseCurrencyId,
                Currency=new Currency
                {
                    Id=employee.Organisation.OrganisationBaseCurrency.BaseCurrencyId,
                    Code= employee.Organisation.OrganisationBaseCurrency.BaseCurrency.Code,
                    Name= employee.Organisation.OrganisationBaseCurrency.BaseCurrency.Name,
                    Symbole= employee.Organisation.OrganisationBaseCurrency.BaseCurrency.Symbole
                },
                IsActive=employee.IsActive,
                JobTitle=employee.JobTitle,
                CountryId = employee.CountryId,
                Country = new Country
                {
                    Alpha2Code = employee.Country.Alpha2Code,
                    Alpha3Code=employee.Country.Alpha3Code,
                    Id=employee.CountryId,
                    Name=employee.Country.Name,
                    Nationality=employee.Country.Nationality
                },
                IsConfederationThatHosts=employee.IsConfederationThatHosts,
            };
        }

        public List<Employee> GetEmployees(string searchText, string organisationId)
        {
            using var context = Context();
            var employees = context.Employee.Where(u => u.OrganisationId == organisationId && (string.IsNullOrEmpty(searchText) || u.DisplayName.Contains(searchText)))
                .Select(employee => new Employee
                {
                    OrganisationId = employee.OrganisationId,
                    Address = employee.Address,
                    DateOfBirth = employee.DateOfBirth,
                    DocumentUrl = employee.DocumentUrl,
                    Email = employee.Email,
                    FirstName = employee.FirstName,
                    Gender = new Gender
                    {
                        Id = employee.GenderId,
                        Name = employee.Gender.Name
                    },
                    DisplayName = employee.DisplayName,
                    GenderId = employee.GenderId,
                    Id = employee.Id,
                    IDOrPassportNumber = employee.IdorPassportNumber,
                    IsResidentEmployee = employee.IsResidentEmployee,
                    LastName = employee.LastName,
                    MaritalStatus = new MaritalStatus
                    {
                        Id = employee.MaritalStatusId,
                        Name = employee.MaritalStatus.Name
                    },
                    MaritalStatusId = employee.MaritalStatusId,
                    PhoneNumber = employee.PhoneNumber,
                    SalaryType = new SalaryType
                    {
                        Name = employee.SalaryType.Name,
                        Id = employee.SalaryTypeId
                    },
                    SalaryTypeId = employee.SalaryTypeId,
                    NumberOfChild = employee.NumberOfChild,
                    Salary = employee.Salary,
                    CurrencyId = employee.Organisation.OrganisationBaseCurrency.BaseCurrency.Id,
                    Currency = new Currency
                    {
                        Id = employee.Organisation.OrganisationBaseCurrency.BaseCurrency.Id,
                        Code = employee.Organisation.OrganisationBaseCurrency.BaseCurrency.Code,
                        Name = employee.Organisation.OrganisationBaseCurrency.BaseCurrency.Name,
                        Symbole = employee.Organisation.OrganisationBaseCurrency.BaseCurrency.Symbole
                    },
                    IsConfederationThatHosts=employee.IsConfederationThatHosts,
                    JobTitle=employee.JobTitle,
                    IsActive=employee.IsActive
                }).ToList();
            return employees;
        }

        public void Update(Employee employee)
        {
            using var context = Context();
            var updateEmployee = context.Employee.FirstOrDefault(u => u.Id == employee.Id && u.OrganisationId == employee.OrganisationId);
            if (updateEmployee == null)
            {
                throw new Exception("Employee not found");
            }
            updateEmployee.PhoneNumber = employee.PhoneNumber;
            updateEmployee.Address = employee.Address;
            updateEmployee.DateOfBirth = employee.DateOfBirth;
            updateEmployee.DisplayName = employee.DisplayName;
            updateEmployee.DocumentUrl = employee.DocumentUrl;
            updateEmployee.Email = employee.Email;
            updateEmployee.FirstName = employee.FirstName;
            updateEmployee.GenderId = employee.GenderId;
            updateEmployee.IdorPassportNumber = employee.IDOrPassportNumber;
            updateEmployee.IsResidentEmployee = employee.IsResidentEmployee;
            updateEmployee.LastName = employee.LastName;
            updateEmployee.MaritalStatusId = employee.MaritalStatusId;
            updateEmployee.NumberOfChild = employee.NumberOfChild;
            updateEmployee.Salary = employee.Salary;
            updateEmployee.SalaryTypeId = employee.SalaryTypeId;
            updateEmployee.IsActive = employee.IsActive;
            updateEmployee.JobTitle = employee.JobTitle;
            updateEmployee.CountryId = employee.CountryId;
            updateEmployee.IsConfederationThatHosts = employee.IsConfederationThatHosts;
            context.SaveChanges();
        }
    }
}
