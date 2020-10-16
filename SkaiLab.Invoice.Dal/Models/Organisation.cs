using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class Organisation
    {
        public Organisation()
        {
            OrganisationUser = new HashSet<OrganisationUser>();
        }

        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string LegalName { get; set; }
        public string LogoUrl { get; set; }
        public int? OrganisationTypeId { get; set; }
        public string LineBusiness { get; set; }
        public string BussinessRegistrationNumber { get; set; }
        public string Description { get; set; }
        public string TaxNumber { get; set; }
        public string TaxDisplayName { get; set; }
        public string ContactId1 { get; set; }
        public string ContactId2 { get; set; }

        public virtual Contact ContactId1Navigation { get; set; }
        public virtual Contact ContactId2Navigation { get; set; }
        public virtual OrganisationType OrganisationType { get; set; }
        public virtual ICollection<OrganisationUser> OrganisationUser { get; set; }
    }
}
