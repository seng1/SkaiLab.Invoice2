using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class OrganisationInvitingUser
    {
        public string OrganisationId { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string RoleName { get; set; }
        public DateTime ExpireToken { get; set; }
        public string Token { get; set; }

        public virtual Organisation Organisation { get; set; }
    }
}
