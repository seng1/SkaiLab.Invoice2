using SkaiLab.Invoice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Service
{
    public interface IMenuService:IService
    {
        List<Menu> GetMenus(string userId, string organisationId);
        Permission GetPermission(string userId, string organisationId);
    }
}
