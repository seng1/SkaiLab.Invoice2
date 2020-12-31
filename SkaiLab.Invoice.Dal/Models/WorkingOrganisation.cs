using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class WorkingOrganisation
    {
        public string UserId { get; set; }
        public string OrganisationId { get; set; }

        public virtual Organisation Organisation { get; set; }
        public virtual AspNetUsers User { get; set; }
    }
}
