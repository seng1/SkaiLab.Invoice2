using System;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Models
{
    public class PayrollMonth
    {
        public long Id { get; set; }
        public string Month { get; set; }
        public decimal Total { get; set; }
        public decimal ExchangeRate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string OrganisationId { get; set; }
        public Currency Currency { get; set; }
    }
    public class PayrollMonthTaxSalary
    {

    }
    public class PayrollMonthTax: PayrollMonth
    {
        public decimal ChildOrSpouseAmount { get; set; }
        public decimal NoneResidentRate { get; set; }
        public decimal AdditionalBenefitsRate { get; set; }
        public List<TaxSalaryRange> TaxSalaryRanges { get; set; }
        public List<PayrollTax> Payrolls { get; set; }
       
        public Currency TaxCurrency { get; set; }
    }
    public class PayrollMonthNoneTax : PayrollMonth
    {
        public List<PayrollNoneTax> Payrolls { get; set; }
    }
    public class Payroll
    {
        public long Id { get; set; }
        public long EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public DateTime Date { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal Total { get; set; }
        public Organsation Organsation { get; set; }
    }
    public class PayrollTax : Payroll
    {
        public decimal Salary { get; set; }
        public int? NumberOfChilds { get; set; }

        public decimal DeductSalary { get; set; }
        public decimal? OtherBenefit { get; set; }
        public decimal? OtherBenefitTaxDeduct { get; set; }

    }
    public class PayrollNoneTax : Payroll
    {
        public decimal Salary { get; set; }
        public decimal? OtherBenefit { get; set; }
    }
    public class TaxSalaryRange
    {
        public long Id { get; set; }
        public decimal FromAmount { get; set; }
        public decimal? ToAmount { get; set; }
        public double Rate { get; set; }
    }
}
