using System;
using System.Collections.Generic;
using System.Linq;

namespace SkaiLab.Invoice.Service
{
    public class TaxService:Service,ITaxService
    {
        public TaxService(IDataContext context) : base(context)
        {

        }

        public void Create(Models.Tax tax)
        {
            using var context = Context();
            if(tax.Components==null || !tax.Components.Any() || tax.Components.Any(t=>string.IsNullOrEmpty(t.Name)) || tax.Components.Any(t => t.Rate < 0))
            {
                throw new Exception("Invalid data to create");
            }
            if(context.Tax.Any(u=>u.Name==tax.Name&& u.OrganisationId == tax.OrganisationId))
            {
                throw new Exception("Tax name already exist");
            }
            foreach(var component in tax.Components)
            {
                if(context.TaxComponent.Any(u=>u.Name==component.Name && u.Tax.OrganisationId == tax.OrganisationId))
                {
                    throw new Exception($"Component name {component.Name} already exist");
                }
            }
            context.Tax.Add(new Dal.Models.Tax
            {
                Name=tax.Name,
                OrganisationId=tax.OrganisationId,
                TaxComponent=tax.Components.Select(u=>new Dal.Models.TaxComponent
                {
                    Name=u.Name,
                    Rate=u.Rate,
                }).ToHashSet()
            });
            context.SaveChanges();
        }

        public Models.Tax GetTax(long id, string organisationId)
        {
            using var context = Context();
            var tax = context.Tax.FirstOrDefault(u => u.Id == id && u.OrganisationId == organisationId);
            if (tax == null)
            {
                throw new Exception("Tax not found");
            }
            return new Models.Tax
            {
                Id = tax.Id,
                Name = tax.Name,
                OrganisationId = tax.OrganisationId,
                TotalRate = tax.TaxComponent.Sum(u => u.Rate),
                Components = tax.TaxComponent.Select(u => new Models.TaxComponent
                {
                    Rate = u.Rate,
                    Id = u.Id,
                    Name = u.Name
                }).ToList()
            };
        }

        public List<Models.Tax> GetTaxes(string organisationId)
        {
            using var context = Context();
            var taxes = context.Tax.Where(u => u.OrganisationId == organisationId)
                .Select(u => new Models.Tax
                {
                    Id=u.Id,
                    Name=u.Name,
                    TotalRate=u.TaxComponent.Sum(u=>u.Rate)
                }).ToList();
            return taxes;
        }

        public List<Models.Tax> GetTaxesIncludeComponent(string organisationId)
        {
            using var context = Context();
            return GetTaxesIncludeComponent(context, organisationId);
        }

        public void Update(Models.Tax tax)
        {
            using var context = Context();
            if(context.Tax.Any(u=>u.Name==tax.Name && u.OrganisationId==tax.OrganisationId && u.Id != tax.Id))
            {
                throw new Exception("Tax name already exist");
            }
            foreach (var component in tax.Components)
            {
                if (context.TaxComponent.Any(u => u.Name == component.Name && u.Tax.OrganisationId == tax.OrganisationId && u.Id!=component.Id))
                {
                    throw new Exception($"Component name {component.Name} already exist");
                }
            }
            var updateTax = context.Tax.FirstOrDefault(u => u.Id == tax.Id && u.OrganisationId == tax.OrganisationId);
            if (updateTax == null)
            {
                throw new Exception("No tax to update");
            }
            updateTax.Name = tax.Name;
            foreach(var component in updateTax.TaxComponent.ToList())
            {
                var c = tax.Components.FirstOrDefault(u => u.Id == component.Id);
                component.Name = c.Name;
            }
            context.SaveChanges();
        }
    }
}
