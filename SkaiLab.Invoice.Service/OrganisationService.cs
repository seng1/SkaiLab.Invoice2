using System;
using SkaiLab.Invoice.Models;
using System.Linq;

namespace SkaiLab.Invoice.Service
{
    public class OrganisationService:Service,IOrganisationService
    {
        public OrganisationService(IDataContext context):base(context)
        {
        }

        public void Create(string userId, string companyName)
        {
            using var context = Context();
            var organisation = new Dal.Models.Organisation
            {
                Id=Guid.NewGuid().ToString(),
                DisplayName=companyName,
                OrganisationTypeId=1
            };
            organisation.OrganisationUser.Add(new Dal.Models.OrganisationUser
            {
                UserId = userId,
                
            });
            context.Organisation.Add(organisation);
            context.SaveChanges();
        }

        public Organsation Get(string id)
        {
            using var context = Context();
            var organsiation = context.Organisation.FirstOrDefault(u => u.Id == id);
            return new Organsation
            {
                Id = organsiation.Id,
                BussinessRegistrationNumber = organsiation.BussinessRegistrationNumber,
                Contact1 = organsiation.ContactId1Navigation == null ? null : new Contact
                {
                    Id = organsiation.ContactId1Navigation.Id,
                    Address = organsiation.ContactId1Navigation.Address,
                    ContactName = organsiation.ContactId1Navigation.ContactName,
                    Email = organsiation.ContactId1Navigation.Email,
                    PhoneNumber = organsiation.ContactId1Navigation.PhoneNumber,
                    Website = organsiation.ContactId1Navigation.Website
                },
                Contact2 = organsiation.ContactId2Navigation == null ? null : new Contact
                {
                    Website = organsiation.ContactId2Navigation.Website,
                    PhoneNumber = organsiation.ContactId2Navigation.PhoneNumber,
                    Email = organsiation.ContactId2Navigation.Email,
                    Address = organsiation.ContactId2Navigation.Address,
                    ContactName = organsiation.ContactId2Navigation.ContactName,
                    Id = organsiation.ContactId2Navigation.Id
                },
                ContactId1 = organsiation.ContactId1,
                ContactId2 = organsiation.ContactId2,
                Description = organsiation.Description,
                DisplayName = organsiation.DisplayName,
                LegalName = organsiation.LegalName,
                LineBusiness = organsiation.LineBusiness,
                LogoUrl = organsiation.LogoUrl,
                OrganisationTypeId = organsiation.OrganisationTypeId,
                TaxDisplayName = organsiation.TaxDisplayName,
                TaxNumber = organsiation.TaxNumber,
                OrganisationType = organsiation.OrganisationType == null ? null : new OrganisationType
                {
                    Id = organsiation.OrganisationType.Id,
                    Name = organsiation.OrganisationType.Name
                }
            };
        }

        public string GetOrganisationIdByUserId(string userId)
        {
            using var context = Context();
            return context.OrganisationUser.FirstOrDefault(u => u.UserId == userId).OrganisationId;
        }

        public void Update(Organsation org)
        {
            using var context = Context();
            var organisation = context.Organisation.FirstOrDefault(u => u.Id == org.Id);
            organisation.DisplayName = org.DisplayName;
            organisation.Description = org.Description;
            organisation.BussinessRegistrationNumber = org.BussinessRegistrationNumber;
            organisation.LegalName = org.LegalName;
            organisation.LineBusiness = org.LineBusiness;
            organisation.OrganisationTypeId = org.OrganisationTypeId;
            organisation.TaxDisplayName = org.TaxDisplayName;
            organisation.TaxNumber = org.TaxNumber;
            if (org.Contact1 != null)
            {
                if (organisation.ContactId1Navigation == null)
                {
                    organisation.ContactId1Navigation = new Dal.Models.Contact
                    {
                        Id = Guid.NewGuid().ToString()
                    };
                    organisation.ContactId1Navigation.Address = org.Contact1.Address;
                    organisation.ContactId1Navigation.ContactName = org.Contact1.ContactName;
                    organisation.ContactId1Navigation.Email = org.Contact1.Email;
                    organisation.ContactId1Navigation.PhoneNumber = org.Contact1.PhoneNumber;
                    organisation.ContactId1Navigation.Website = org.Contact1.Website;
                }
            }
            if (org.Contact2 != null)
            {
                if (organisation.ContactId2Navigation == null)
                {
                    organisation.ContactId2Navigation = new Dal.Models.Contact
                    {
                        Id = Guid.NewGuid().ToString()
                    };
                    organisation.ContactId2Navigation.Address = org.Contact2.Address;
                    organisation.ContactId2Navigation.ContactName = org.Contact2.ContactName;
                    organisation.ContactId2Navigation.Email = org.Contact2.Email;
                    organisation.ContactId2Navigation.PhoneNumber = org.Contact2.PhoneNumber;
                    organisation.ContactId2Navigation.Website = org.Contact2.Website;
                }
            }
            context.SaveChanges();

        }
    }
}
