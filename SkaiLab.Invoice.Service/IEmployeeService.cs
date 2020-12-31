using SkaiLab.Invoice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Service
{
    public interface IEmployeeService:IService
    {
        void Add(Employee employee);
        void Update(Employee employee);
        Employee Get(long id, string organisationId);
        List<Employee> GetEmployees(string searchText, string organisationId);
    }
}
