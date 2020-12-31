using SkaiLab.Invoice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Service
{
    public interface IPayrollService : IService
    {
        PayrollMonthTax GetMonthTax(string organisationId, string month);
        PayrollMonthNoneTax GetPayrollMonth(string organisationId, string month);
        void CreatePayroll(PayrollMonthTax payrollMonthTax, string organisationId, string month);
        void CreatePayroll(PayrollMonthNoneTax payrollNoneTax, string organisationId, string month);
    }
}
