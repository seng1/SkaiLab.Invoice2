using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class MenuFeature
    {
        public MenuFeature()
        {
            OrganisationInvitingUserMenuFeature = new HashSet<OrganisationInvitingUserMenuFeature>();
            OrganisationUserMenuFeature = new HashSet<OrganisationUserMenuFeature>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string NameKh { get; set; }

        public virtual ICollection<OrganisationInvitingUserMenuFeature> OrganisationInvitingUserMenuFeature { get; set; }
        public virtual ICollection<OrganisationUserMenuFeature> OrganisationUserMenuFeature { get; set; }
    }
}
