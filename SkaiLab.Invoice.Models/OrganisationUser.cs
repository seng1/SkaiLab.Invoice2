using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Models
{
    public class OrganisationUser
    {
        public string OrganisationId { get; set; }
        public User User { get; set; }
        public string RoleName { get; set; }
        public bool IsInviting { get; set; }
        public bool IsInvitingExpired { get; set; }
        public List<MenuFeature> MenuFeatures { get; set; }
        public bool IsOwner { get; set; }
        public bool IsAdministrator { get; set; }
    }
}
