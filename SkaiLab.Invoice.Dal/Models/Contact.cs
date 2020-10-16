using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class Contact
    {
        public Contact()
        {
            OrganisationContactId1Navigation = new HashSet<Organisation>();
            OrganisationContactId2Navigation = new HashSet<Organisation>();
        }

        public string Id { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Website { get; set; }
        public string ContactName { get; set; }

        public virtual ICollection<Organisation> OrganisationContactId1Navigation { get; set; }
        public virtual ICollection<Organisation> OrganisationContactId2Navigation { get; set; }
    }
}
