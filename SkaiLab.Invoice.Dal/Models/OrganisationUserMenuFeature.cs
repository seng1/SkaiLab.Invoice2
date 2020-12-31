using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class OrganisationUserMenuFeature
    {
        public string OrganisationId { get; set; }
        public string UserId { get; set; }
        public int MenuFeatureId { get; set; }

        public virtual MenuFeature MenuFeature { get; set; }
        public virtual Organisation Organisation { get; set; }
        public virtual AspNetUsers User { get; set; }
    }
}
