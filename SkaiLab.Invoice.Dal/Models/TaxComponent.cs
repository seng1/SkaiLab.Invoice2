using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class TaxComponent
    {
        public long Id { get; set; }
        public long TaxId { get; set; }
        public string Name { get; set; }
        public decimal Rate { get; set; }

        public virtual Tax Tax { get; set; }
    }
}
