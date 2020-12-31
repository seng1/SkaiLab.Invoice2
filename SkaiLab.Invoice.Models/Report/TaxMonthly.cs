using System.Collections.Generic;
namespace SkaiLab.Invoice.Models.Report
{
    public class TaxMonthly
    {
        public decimal TotalInvoice { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal TotalEmployeeSalary { get; set; }
        public decimal TotalPayToTax { get; set; }
        public decimal TotalPayToTaxInKHR { get; set; }
        public bool IsPayrollRun { get; set; }
        public List<Invoice> Invoices { get; set; }
        public List<Expense> Expenses { get; set; }
        public PayrollMonthTax Payroll { get; set; }
        public Currency Currency { get; set; }
        public Currency TaxCurrency { get; set; }
        public decimal TotalInvoiceTaxInBaseCurrency { get; set; }
        public decimal TotalInvoiceTaxInTaxCurrency { get; set; }
        public decimal TotalExpenseTaxInBaseCurrency { get; set; }
        public decimal TotalExpenseTaxInTaxCurrency { get; set; }
        public decimal TotalEmployeeTaxInBaseCurrency { get; set; }
        public decimal TotalEmployeeTaxInTaxCurrency { get; set; }
    }
}
