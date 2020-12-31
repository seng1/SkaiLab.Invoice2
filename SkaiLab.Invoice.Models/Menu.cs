using System.Collections.Generic;

namespace SkaiLab.Invoice.Models
{
    public class Menu
    {
        public string Name { get; set; }
        public string IconClass { get; set; }
        public bool IsActive { get; set; }
        public List<Menu> MenuItems { get; set; }
        public string RouteLink { get; set; }
    }
}
