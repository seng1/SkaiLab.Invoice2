using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class AspNetUserTokensCode
    {
        public string UserId { get; set; }
        public string Code { get; set; }
        public DateTime? Expire { get; set; }

        public virtual AspNetUsers User { get; set; }
    }
}
