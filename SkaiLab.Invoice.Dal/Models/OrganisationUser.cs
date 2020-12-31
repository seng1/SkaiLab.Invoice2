using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class OrganisationUser
    {
        public string OrganisationId { get; set; }
        public string UserId { get; set; }
        public bool IsOwner { get; set; }
        public string RoleName { get; set; }

        public virtual Organisation Organisation { get; set; }
        public virtual AspNetUsers User { get; set; }
    }
}
