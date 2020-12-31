using System.Collections.Generic;
namespace SkaiLab.Invoice.Models
{
    public class Tax
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string OrganisationId { get; set; }
        public decimal TotalRate { get; set; }
        public List<TaxComponent> Components { get; set; }

    }
    public class TaxComponent
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public decimal Rate { get; set; }
    }
}
