using SkaiLab.Invoice.Models;
using SkaiLab.Invoice.Models.Filter;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Service
{
    public interface ICustomerService:IService
    {
        void GetTotalPages(CustomerFilter filter);
        List<Customer> GetCustomers(CustomerFilter filter);
        void Create(Customer customer);
        Customer GetCustomer(long id, string organisationId);
        void Update(Customer customer);
        List<Customer> GetCustomers(string organisationId);
    }
}
