using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Models
{
    public class Permission
    {
        public bool ReadWritePurchaseSale { get; set; }
        public bool ReadPurchaseSale { get; set; }
        public bool ApprovaPayPurchaseSale { get; set; }
        public bool ManageOrganisactionSetting { get; set; }
        public bool ManageAndInviteUser { get; set; }
        public bool Payroll { get; set; }
        public bool Report { get; set; }
    }
}
