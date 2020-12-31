using SkaiLab.Invoice.Models;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Service
{
    public interface ITaxService : IService
    {
        List<Tax> GetTaxes(string organisationId);
        void Create(Tax tax);
        void Update(Tax tax);
        Tax GetTax(long id, string organisationId);
        List<Tax> GetTaxesIncludeComponent(string organisationId);
    }
}
