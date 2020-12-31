using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class Country
    {
        public Country()
        {
            Employee = new HashSet<Employee>();
        }

        public int Id { get; set; }
        public string Alpha2Code { get; set; }
        public string Alpha3Code { get; set; }
        public string Name { get; set; }
        public string Nationality { get; set; }
        public string NameKh { get; set; }
        public string NationalityKh { get; set; }

        public virtual ICollection<Employee> Employee { get; set; }
    }
}
