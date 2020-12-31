using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class ProjectPlan
    {
        public ProjectPlan()
        {
            Plan = new HashSet<Plan>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Plan> Plan { get; set; }
    }
}
