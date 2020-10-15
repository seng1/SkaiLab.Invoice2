using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Models
{
    public class Token
    {
        public string AccessToken { get; set; }
        public string Name { get; set; }
        public int ValidHour { get; set; }
    }
}
