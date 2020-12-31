using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class Contact
    {
        public Contact()
        {
            Customer = new HashSet<Customer>();
            Organisation = new HashSet<Organisation>();
            Vendor = new HashSet<Vendor>();
        }

        public string Id { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Website { get; set; }
        public string ContactName { get; set; }

        public virtual ICollection<Customer> Customer { get; set; }
        public virtual ICollection<Organisation> Organisation { get; set; }
        public virtual ICollection<Vendor> Vendor { get; set; }
    }
}
