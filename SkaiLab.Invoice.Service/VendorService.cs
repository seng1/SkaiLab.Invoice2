using Microsoft.Extensions.Azure;
using SkaiLab.Invoice.Models;
using SkaiLab.Invoice.Models.Filter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SkaiLab.Invoice.Service
{
    public class VendorService : Service, IVendorService
    {
        public VendorService(IDataContext context) : base(context)
        {

        }

        public void Create(Vendor vendor)
        {
            using var context = Context();
            context.Vendor.Add(new Dal.Models.Vendor
            {
                BusinessRegistrationNumber=vendor.BusinessRegistrationNumber,
                Contact=new Dal.Models.Contact
                {
                    Address=vendor.Contact.Address,
                    Website=vendor.Contact.Website,
                    PhoneNumber=vendor.Contact.PhoneNumber,
                    Id=Guid.NewGuid().ToString(),
                    Email=vendor.Contact.Email,
                    ContactName=vendor.Contact.ContactName
                },
                CurrencyId=vendor.CurrencyId,
                DisplayName=vendor.DisplayName,
                LegalName=vendor.LegalName,
                OrganisationId=vendor.OrganisationId,
                TaxNumber=vendor.TaxNumber,
                LocalLegalName=vendor.LocalLegalName
                
            });
            context.SaveChanges();
        }

        public void GetTotalPage(VendorFilter filter)
        {
            using var context = Context();
            filter.TotalRow = context.Vendor.Where(u => u.OrganisationId == filter.OrganisationId &&
              (string.IsNullOrEmpty(filter.SearchText) || u.DisplayName.Contains(filter.SearchText) ||
              u.LegalName.Contains(filter.SearchText))).Count();
        }

        public Vendor GetVendor(long id, string organisationId)
        {
            using var context = Context();
            var vendor = context.Vendor.FirstOrDefault(u => u.Id == id && u.OrganisationId == organisationId);
            if (vendor == null)
            {
                throw new Exception("Vendor not found");
            }
            return new Vendor
            {
                BusinessRegistrationNumber=vendor.BusinessRegistrationNumber,
                Contact=new Contact
                {
                    Address=vendor.Contact.Address,
                    ContactName=vendor.Contact.ContactName,
                    Email=vendor.Contact.Email,
                    Id=vendor.ContactId,
                    PhoneNumber=vendor.Contact.PhoneNumber,
                    Website=vendor.Contact.Website
                },
                ContactId=vendor.ContactId,
                Id=vendor.Id,
                Currency=new Currency
                {
                    Id=vendor.CurrencyId,
                    Code=vendor.Currency.Code,
                    Name=vendor.Currency.Name,
                    Symbole=vendor.Currency.Symbole
                },
                 CurrencyId=vendor.CurrencyId,
                 DisplayName=vendor.DisplayName,
                 LegalName=vendor.LegalName,
                 OrganisationId=vendor.OrganisationId,
                 TaxNumber=vendor.TaxNumber,
                 LocalLegalName=vendor.LocalLegalName
            };
        }

        public List<Vendor> GetVendors(VendorFilter filter)
        {
            using var context = Context();
            var vendors = context.Vendor.Where(u => u.OrganisationId == filter.OrganisationId &&
               (string.IsNullOrEmpty(filter.SearchText) || u.DisplayName.Contains(filter.SearchText) ||
               u.LegalName.Contains(filter.SearchText)))
                .OrderBy(u => u.DisplayName)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(u => new Vendor
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
                    }
                }).ToList();
            return vendors;
        }

        public List<Vendor> GetVendors(string organisationId)
        {
            using var context = Context();
            var vendors = context.Vendor.Where(u => u.OrganisationId == organisationId)
                .OrderBy(u => u.DisplayName)
                .Select(u => new Vendor
                {
                    Id=u.Id,
                    DisplayName=u.DisplayName,
                    CurrencyId=u.CurrencyId,
                    ContactId=u.ContactId,
                    Contact=new Contact
                    {
                        Id=u.ContactId,
                        Address=u.Contact.Address,
                        ContactName=u.Contact.ContactName,
                        Email=u.Contact.Email,
                        PhoneNumber=u.Contact.PhoneNumber,
                        Website=u.Contact.Website
                    },
                    LocalLegalName=u.LocalLegalName
                }).ToList();
            return vendors;
        }

        public void Update(Vendor vendor)
        {
            using var context = Context();
            var vendorDb = context.Vendor.FirstOrDefault(u => u.Id == vendor.Id && u.OrganisationId == vendor.OrganisationId);
            if (vendorDb == null)
            {
                throw new Exception("Vendor not found");
            }
            vendorDb.LegalName = vendor.LegalName;
            vendorDb.TaxNumber = vendor.TaxNumber;
            vendorDb.BusinessRegistrationNumber = vendor.BusinessRegistrationNumber;
            vendorDb.Contact.Address = vendor.Contact.Address;
            vendorDb.Contact.ContactName = vendor.Contact.ContactName;
            vendorDb.Contact.Email = vendor.Contact.Email;
            vendorDb.Contact.PhoneNumber = vendor.Contact.PhoneNumber;
            vendorDb.Contact.Website = vendor.Contact.Website;
            vendorDb.CurrencyId = vendor.CurrencyId;
            vendorDb.DisplayName = vendor.DisplayName;
            vendorDb.LocalLegalName = vendor.LocalLegalName;
            context.SaveChanges();
        }
    }
}
