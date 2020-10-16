using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class OrganisationType
    {
        public OrganisationType()
        {
            Organisation = new HashSet<Organisation>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Organisation> Organisation { get; set; }
    }
}
