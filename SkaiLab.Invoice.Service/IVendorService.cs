using SkaiLab.Invoice.Models;
using SkaiLab.Invoice.Models.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Service
{
    public interface IVendorService:IService
    {
        void GetTotalPage(VendorFilter filter);
        List<Vendor> GetVendors(VendorFilter filter);
        void Create(Vendor vendor);
        Vendor GetVendor(long id, string organisationId);
        void Update(Vendor vendor);
        List<Vendor> GetVendors(string organisationId);
    }
}
