using SkaiLab.Invoice.Models;
using SkaiLab.Invoice.Models.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Service
{
    public class CustomerService:Service,ICustomerService
    {
        public CustomerService(IDataContext context) : base(context)
        {

        }

        public void Create(Customer customer)
        {
            using var context = Context();
            context.Customer.Add(new Dal.Models.Customer
            {
                BusinessRegistrationNumber = customer.BusinessRegistrationNumber,
                Contact = new Dal.Models.Contact
                {
                    Address = customer.Contact.Address,
                    Website = customer.Contact.Website,
                    PhoneNumber = customer.Contact.PhoneNumber,
                    Id = Guid.NewGuid().ToString(),
                    Email = customer.Contact.Email,
                    ContactName = customer.Contact.ContactName
                },
                CurrencyId = customer.CurrencyId,
                DisplayName = customer.DisplayName,
                LegalName = customer.LegalName,
                OrganisationId = customer.OrganisationId,
                TaxNumber = customer.TaxNumber,
                LocalLegalName=customer.LocalLegalName
            });
            context.SaveChanges();
        }

        public Customer GetCustomer(long id, string organisationId)
        {
            using var context = Context();
            var customer = context.Customer.FirstOrDefault(u => u.Id == id && u.OrganisationId == organisationId);
            if (customer == null)
            {
                throw new Exception("Customer not found");
            }
            return new Customer
            {
                BusinessRegistrationNumber = customer.BusinessRegistrationNumber,
                Contact = new Contact
                {
                    Address = customer.Contact.Address,
                    ContactName = customer.Contact.ContactName,
                    Email = customer.Contact.Email,
                    Id = customer.ContactId,
                    PhoneNumber = customer.Contact.PhoneNumber,
                    Website = customer.Contact.Website
                },
                ContactId = customer.ContactId,
                Id = customer.Id,
                Currency = new Currency
                {
                    Id = customer.CurrencyId,
                    Code = customer.Currency.Code,
                    Name = customer.Currency.Name,
                    Symbole = customer.Currency.Symbole
                },
                CurrencyId = customer.CurrencyId,
                DisplayName = customer.DisplayName,
                LegalName = customer.LegalName,
                OrganisationId = customer.OrganisationId,
                TaxNumber = customer.TaxNumber,
                LocalLegalName=customer.LocalLegalName
            };
        }

        public List<Customer> GetCustomers(CustomerFilter filter)
        {
            using var context = Context();
            var customers = context.Customer.Where(u => u.OrganisationId == filter.OrganisationId &&
               (string.IsNullOrEmpty(filter.SearchText) || u.DisplayName.Contains(filter.SearchText) ||
               u.LegalName.Contains(filter.SearchText)))
                .OrderBy(u => u.DisplayName)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(u => new Customer
                {
                    BusinessRegistrationNumber = u.BusinessRegistrationNumber,
                    DisplayName = u.DisplayName,
                    LegalName = u.LegalName,
                    TaxNumber = u.TaxNumber,
                    Id = u.Id,
                    ContactId = u.ContactId,
                    Contact = new Contact
                    {
                        Id = u.ContactId,
                        Address = u.Contact.Address,
                        ContactName = u.Contact.ContactName,
                        Email = u.Contact.Email,
                        PhoneNumber = u.Contact.PhoneNumber,
                        Website = u.Contact.Website
                    },
                    LocalLegalName=u.LocalLegalName
                }).ToList();
            return customers;
        }

        public List<Customer> GetCustomers(string organisationId)
        {
            using var context = Context();
            var customers = context.Customer.Where(u => u.OrganisationId == organisationId)
                .OrderBy(u => u.DisplayName)
                .Select(u => new Customer
                {
                    Id=u.Id,
                    DisplayName=u.DisplayName,
                    BusinessRegistrationNumber=u.BusinessRegistrationNumber,
                    LegalName=u.LegalName,
                    TaxNumber=u.TaxNumber,
                    CurrencyId=u.CurrencyId,
                    LocalLegalName=u.LocalLegalName
                }).ToList();
            return customers;
        }

        public void GetTotalPages(CustomerFilter filter)
        {
            using var context = Context();
            filter.TotalRow = context.Customer.Where(u => u.OrganisationId == filter.OrganisationId &&
              (string.IsNullOrEmpty(filter.SearchText) || u.DisplayName.Contains(filter.SearchText) ||
              u.LegalName.Contains(filter.SearchText))).Count();
        }

        public void Update(Customer customer)
        {
            using var context = Context();
            var customerDb = context.Customer.FirstOrDefault(u => u.Id == customer.Id && u.OrganisationId == customer.OrganisationId);
            if (customerDb == null)
            {
                throw new Exception("Customer not found");
            }
            customerDb.LegalName = customer.LegalName;
            customerDb.TaxNumber = customer.TaxNumber;
            customerDb.BusinessRegistrationNumber = customer.BusinessRegistrationNumber;
            customerDb.Contact.Address = customer.Contact.Address;
            customerDb.Contact.ContactName = customer.Contact.ContactName;
            customerDb.Contact.Email = customer.Contact.Email;
            customerDb.Contact.PhoneNumber = customer.Contact.PhoneNumber;
            customerDb.Contact.Website = customer.Contact.Website;
            customerDb.CurrencyId = customer.CurrencyId;
            customerDb.DisplayName = customer.DisplayName;
            customerDb.LocalLegalName = customer.LocalLegalName;
            context.SaveChanges();
        }
    }
}
