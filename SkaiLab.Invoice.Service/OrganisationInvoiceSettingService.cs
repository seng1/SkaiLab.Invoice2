using SkaiLab.Invoice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Service
{
    public class OrganisationInvoiceSettingService:Service,IOrganisationInvoiceSettingService
    {
        public OrganisationInvoiceSettingService(IDataContext context) : base(context)
        {

        }

        public OrganisationInvoiceSetting GetOrganisationInvoiceSetting(string id)
        {
            using var context = Context();
            var organisation = context.OrganisationInvoiceSetting.FirstOrDefault(u => u.Id == id);
            if (organisation == null)
            {
                return new OrganisationInvoiceSetting
                {
                    Id=id,
                    TermAndConditionForInvoice="",
                    TermAndConditionForQuote=""
                };
            }
            return new OrganisationInvoiceSetting
            {
                Id = organisation.Id,
                TermAndConditionForQuote = organisation.TermAndConditionForQuote,
                TermAndConditionForInvoice = organisation.TermAndConditionForInvoice,
                TermAndConditionForPurchaseOrder=organisation.TermAndConditionForPurchaseOrder
            };
        }

        public void Save(OrganisationInvoiceSetting organisationInvoiceSetting)
        {
            using var context = Context();
            var organisation = context.OrganisationInvoiceSetting.FirstOrDefault(u => u.Id == organisationInvoiceSetting.Id);
            if (organisation == null)
            {
                organisation = new Dal.Models.OrganisationInvoiceSetting
                {
                    Id = organisationInvoiceSetting.Id
                };
                context.OrganisationInvoiceSetting.Add(organisation);
            }
            organisation.TermAndConditionForInvoice = organisationInvoiceSetting.TermAndConditionForInvoice;
            organisation.TermAndConditionForQuote = organisationInvoiceSetting.TermAndConditionForQuote;
            organisation.TermAndConditionForPurchaseOrder = organisationInvoiceSetting.TermAndConditionForPurchaseOrder;
            context.SaveChanges();
        }
    }
}
