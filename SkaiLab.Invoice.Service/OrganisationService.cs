using System;
using SkaiLab.Invoice.Models;
using System.Linq;
using System.Collections.Generic;
namespace SkaiLab.Invoice.Service
{
    public class OrganisationService:Service,IOrganisationService
    {
        public OrganisationService(IDataContext context):base(context)
        {
        }

        public void ChangeWorkingOrganisation(string organisationId, string userId)
        {
            using var context = Context();
            if(!context.OrganisationUser.Any(t=>t.OrganisationId==organisationId && t.UserId == userId))
            {
                throw new Exception("Access deny");
            }
            var workingOrganisation = context.WorkingOrganisation.FirstOrDefault(u => u.UserId == userId);
            if (workingOrganisation == null)
            {
                workingOrganisation = new Dal.Models.WorkingOrganisation
                {
                    UserId = userId
                };
                context.WorkingOrganisation.Add(workingOrganisation);
            }
            workingOrganisation.OrganisationId = organisationId;
            context.SaveChanges();
        }

        public void Create(string userId, string companyName)
        {
            using var context = Context();
            var organisation = new Dal.Models.Organisation
            {
                Id=Guid.NewGuid().ToString(),
                DisplayName=companyName,
                OrganisationTypeId=1,
                DeclareTax=true
            };
            organisation.OrganisationUser.Add(new Dal.Models.OrganisationUser
            {
                UserId = userId,
                
            });
            context.Organisation.Add(organisation);
            context.SaveChanges();
        }

        public int Create(Organsation organsation, string userId)
        {
            using var context = Context();
            var userPlan = context.UserPlan.FirstOrDefault(u => u.UserId == userId);
            if (userPlan == null)
            {
                return (int)CreateCompanyResultEnum.UserNoLicense;
            }
            if (userPlan.Expire == null || userPlan.Expire< CurrentCambodiaTime)
            {
                return (int)CreateCompanyResultEnum.LicenseExpireOrIncomple;
            }
            var totalOrganisation = context.OrganisationUser.Where(u => u.UserId == userId && u.IsOwner).Count();
            int maxOrganisation =userPlan.PlanId==1?2:(userPlan.PlanId==2?4:100000000);
            if (totalOrganisation >= maxOrganisation)
            {
                return (int)CreateCompanyResultEnum.LimitCreateNumberOfOrganisation;
            }
            if (!organsation.DeclareTax)
            {
                organsation.OrganisationBaseCurrency.TaxCurrencyId = organsation.OrganisationBaseCurrency.BaseCurrencyId;
            }
            if (!string.IsNullOrEmpty(organsation.LogoUrl)){
                organsation.LogoUrl = UploadFile(organsation.LogoUrl);
            }
            var newOrganisation = new Dal.Models.Organisation
            {
                BussinessRegistrationNumber=organsation.DeclareTax?organsation.BussinessRegistrationNumber:"",
                DeclareTax=organsation.DeclareTax,
                Description=organsation.Description,
                DisplayName=organsation.DisplayName,
                LegalLocalName=organsation.DeclareTax?organsation.LegalLocalName:"",
                LegalName=organsation.DeclareTax?organsation.LegalName:"",
                LineBusiness=organsation.LineBusiness,
                OrganisationTypeId=organsation.OrganisationTypeId,
                TaxNumber=organsation.DeclareTax?organsation.TaxNumber:"",
                OrganisationBaseCurrency=new Dal.Models.OrganisationBaseCurrency
                {
                    BaseCurrencyId=organsation.OrganisationBaseCurrency.BaseCurrencyId,
                    TaxCurrencyId=organsation.OrganisationBaseCurrency.TaxCurrencyId
                },
                Contact=new Dal.Models.Contact
                {
                    Address=organsation.Contact.Address,
                    ContactName=organsation.Contact.ContactName,
                    Email=organsation.Contact.Email,
                    PhoneNumber=organsation.Contact.PhoneNumber,
                    Website=organsation.Contact.Website,
                    Id=Guid.NewGuid().ToString()
                },
                LogoUrl=organsation.LogoUrl,
                Id=Guid.NewGuid().ToString()
            };
            var workingOrganisation = context.WorkingOrganisation.FirstOrDefault(u => u.UserId == UserId);
            if (workingOrganisation == null)
            {
                workingOrganisation = new Dal.Models.WorkingOrganisation
                {
                    UserId = userId,
                    Organisation=newOrganisation
                };
                context.WorkingOrganisation.Add(workingOrganisation);
               
            }
            else
            {
                workingOrganisation.Organisation = newOrganisation;
            }
            newOrganisation.OrganisationUser.Add(new Dal.Models.OrganisationUser
            {
                UserId=userId,
                IsOwner=true,
                RoleName = "Owner",
            });
           
            if (organsation.OrganisationBaseCurrency.BaseCurrencyId != organsation.OrganisationBaseCurrency.TaxCurrencyId)
            {
                var baseCurrency = new Dal.Models.OrganisationCurrency
                {
                    CurrencyId = organsation.OrganisationBaseCurrency.BaseCurrencyId
                };
                var taxCurrency = new Dal.Models.OrganisationCurrency
                {
                    CurrencyId=organsation.OrganisationBaseCurrency.TaxCurrencyId
                };
                newOrganisation.OrganisationCurrency.Add(baseCurrency);
                newOrganisation.OrganisationCurrency.Add(taxCurrency);
                context.ExchangeRate.Add(new Dal.Models.ExchangeRate
                {
                    FromCurrencyId = baseCurrency.CurrencyId,
                    ToCurrencyId = taxCurrency.CurrencyId,
                    IsAuto = false,
                    ExchangeRate1 = organsation.OrganisationBaseCurrency.TaxExchangeRate
                });
                context.ExchangeRate.Add(new Dal.Models.ExchangeRate
                {
                    FromCurrencyId = taxCurrency.CurrencyId,
                    ToCurrencyId = baseCurrency.CurrencyId,
                    IsAuto = false,
                    ExchangeRate1 =1/ organsation.OrganisationBaseCurrency.TaxExchangeRate
                });
            }
            else
            {
                newOrganisation.OrganisationCurrency.Add(new Dal.Models.OrganisationCurrency
                {
                    CurrencyId = organsation.OrganisationBaseCurrency.BaseCurrencyId,
                });
            }
            context.Organisation.Add(newOrganisation);
            var features = context.MenuFeature.Select(u => u.Id).ToList();
            foreach (var feature in features)
            {
                context.OrganisationUserMenuFeature.Add(new Dal.Models.OrganisationUserMenuFeature
                {
                    MenuFeatureId = feature,
                   Organisation=newOrganisation,
                    UserId = userId
                });
            }
            if (organsation.DeclareTax)
            {
                context.Tax.Add(new Dal.Models.Tax
                {
                    Name = "អាករលើតម្លៃបន្ថែម​(១០%)/Tax (10%)",
                    TaxComponent = new List<Dal.Models.TaxComponent>
                    {
                        new Dal.Models.TaxComponent
                        {
                            Rate=10,
                            Name=$"អាករលើតម្លៃបន្ថែម​(១០%){Environment.NewLine}Tax (10%)"
                        }
                    },
                    Organisation=newOrganisation
                });
                context.Tax.Add(new Dal.Models.Tax
                {
                    Name = "ពន្ធកាត់ទុក (១៤%)/Withholding Tax(14%)",
                    TaxComponent = new List<Dal.Models.TaxComponent>
                    {
                        new Dal.Models.TaxComponent
                        {
                            Rate=14,
                            Name=$"ពន្ធកាត់ទុក (១៤%){Environment.NewLine}Withholding Tax(14%)"
                        }
                    },
                    Organisation = newOrganisation
                });
                context.Tax.Add(new Dal.Models.Tax
                {
                    Name = "ពន្ធកាត់ទុក (១៥%)/Withholding Tax(15%)",
                    TaxComponent = new List<Dal.Models.TaxComponent>
                    {
                        new Dal.Models.TaxComponent
                        {
                            Rate=15,
                            Name=$"ពន្ធកាត់ទុក (១៥%){Environment.NewLine}Withholding Tax(15%)"
                        }
                    },
                    Organisation = newOrganisation
                });
            }
            context.SaveChanges();
            organsation.Id = newOrganisation.Id;
            return (int) CreateCompanyResultEnum.Success;
        }

        public Organsation Get(string id)
        {
            using var context = Context();
            var organsiation = context.Organisation.FirstOrDefault(u => u.Id == id);
            return new Organsation
            {
                Id = organsiation.Id,
                BussinessRegistrationNumber = organsiation.BussinessRegistrationNumber,
                Contact = organsiation.Contact == null ? null : new Contact
                {
                    Id = organsiation.Contact.Id,
                    Address = organsiation.Contact.Address,
                    ContactName = organsiation.Contact.ContactName,
                    Email = organsiation.Contact.Email,
                    PhoneNumber = organsiation.Contact.PhoneNumber,
                    Website = organsiation.Contact.Website
                },
                ContactId = organsiation.ContactId,
                Description = organsiation.Description,
                DisplayName = organsiation.DisplayName,
                LegalName = organsiation.LegalName,
                LineBusiness = organsiation.LineBusiness,
                LogoUrl = organsiation.LogoUrl,
                OrganisationTypeId = organsiation.OrganisationTypeId,
                LegalLocalName = organsiation.LegalLocalName,
                TaxNumber = organsiation.TaxNumber,
                OrganisationType = organsiation.OrganisationType == null ? null : new OrganisationType
                {
                    Id = organsiation.OrganisationType.Id,
                    Name = organsiation.OrganisationType.Name
                },
                DeclareTax=organsiation.DeclareTax
            };
        }

        public Currency GetBaseCurrency(string organisationId)
        {
            using var context = Context();
            var currency = context.OrganisationBaseCurrency.FirstOrDefault(u => u.OrganisationId == organisationId).BaseCurrency;
            return new Currency
            {
                Code=currency.Code,
                Name=currency.Name,
                Symbole=currency.Symbole,
                Id=currency.Id
            };
        }

        public int GetMaximumCreateOrganisationByUser(string userId)
        {
            using var context = Context();
            var userPlan = context.UserPlan.FirstOrDefault(u => u.UserId == userId);
            if (userPlan == null)
            {
                return 0;
            }
            if (userPlan.PlanId == 1)
            {
                return 2;
            }
            if (userPlan.PlanId == 2)
            {
                return 4;
            }
            return 100000000;
        }

        public string GetOrganisationIdByUserId(string userId)
        {
            using var context = Context();
            return context.OrganisationUser.FirstOrDefault(u => u.UserId == userId).OrganisationId;
        }

        public List<Organsation> GetOrganisations(string userId)
        {
            using var context = Context();
            var organisation = context.OrganisationUser.Where(u => u.UserId == userId)
                .Select(u => new Organsation
                {
                    Id=u.OrganisationId,
                    DisplayName=u.Organisation.DisplayName,
                    DeclareTax=u.Organisation.DeclareTax
                }).ToList();
            return organisation;
        }

        public List<Organsation> GetOrganisationsWithSameBaseCurrency(string organisationId, string userId)
        {
            using var context = Context();
            var currencyId = context.OrganisationBaseCurrency.FirstOrDefault(u => u.OrganisationId == organisationId).BaseCurrencyId;
            var organisations = context.OrganisationUser.Where(u => u.UserId == userId && u.Organisation.OrganisationBaseCurrency.BaseCurrencyId == currencyId)
                .Select(u => new Organsation
                {
                    Id=u.OrganisationId,
                    DisplayName=u.Organisation.DisplayName

                }).ToList();
            return organisations;
        }

        public Organsation GetWorkingOrganisation(string userId)
        {
            using var context = Context();
            var organisation = context.WorkingOrganisation.FirstOrDefault(u => u.UserId == userId);
            if (organisation != null)
            {
                return new Organsation
                {
                    Id = organisation.OrganisationId,
                    DisplayName = organisation.Organisation.DisplayName,
                    DeclareTax=organisation.Organisation.DeclareTax
                };
            }
            var organisation2 = context.OrganisationUser.FirstOrDefault(u => u.UserId == userId);
            if (organisation2 != null)
            {
                return new Organsation
                {
                    Id = organisation2.OrganisationId,
                    DisplayName = organisation2.Organisation.DisplayName,
                    DeclareTax=organisation2.Organisation.DeclareTax
                };
            }
            return null;
        }

        public void Update(Organsation org)
        {
            using var context = Context();
            var organisation = context.Organisation.FirstOrDefault(u => u.Id == org.Id);
            if(!string.IsNullOrEmpty(org.LogoUrl) && !org.LogoUrl.Contains("http"))
            {
                org.LogoUrl = UploadFile(org.LogoUrl);
            }
            organisation.LogoUrl = org.LogoUrl;
            organisation.DisplayName = org.DisplayName;
            organisation.Description = org.Description;
            organisation.BussinessRegistrationNumber = org.BussinessRegistrationNumber;
            organisation.LegalName = org.LegalName;
            organisation.LineBusiness = org.LineBusiness;
            organisation.OrganisationTypeId = org.OrganisationTypeId;
            organisation.LegalLocalName = org.LegalLocalName;
            organisation.TaxNumber = org.TaxNumber;
            if (organisation.Contact == null)
            {
                organisation.Contact = new Dal.Models.Contact();
            }
            organisation.Contact.Address = org.Contact.Address;
            organisation.Contact.ContactName = org.Contact.ContactName;
            organisation.Contact.Email = org.Contact.Email;
            organisation.Contact.PhoneNumber = org.Contact.PhoneNumber;
            organisation.Contact.Website = org.Contact.Website;

            context.SaveChanges();

        }
    }
}
