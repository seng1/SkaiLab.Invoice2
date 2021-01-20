using SkaiLab.Invoice.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SkaiLab.Invoice.Service
{
    public class PayrollService : Service, IPayrollService
    {
        public PayrollService(IDataContext context) : base(context)
        {

        }

        public PayrollMonthTax GetMonthTax(string organisationId, string month)
        {
            using var context = Context();
            var organisation = context.Organisation.FirstOrDefault(u => u.Id == organisationId);
            if (!context.PayrollMonth.Any(u => u.Month == month && u.OrganisationId == organisationId))
            {
              
                var taxSalary = context.TaxSalary.FirstOrDefault();
               
                var exchangeRate = organisation.OrganisationBaseCurrency.TaxCurrencyId == organisation.OrganisationBaseCurrency.BaseCurrencyId ? 1 : context.ExchangeRate.FirstOrDefault(u => u.FromCurrencyId == organisation.OrganisationBaseCurrency.BaseCurrencyId && u.ToCurrencyId == organisation.OrganisationBaseCurrency.TaxCurrencyId).ExchangeRate1;
                var payroll= new PayrollMonthTax
                {
                    AdditionalBenefitsRate=taxSalary.AdditionalBenefits,
                    NoneResidentRate=taxSalary.NoneResidentRate,
                    ChildOrSpouseAmount=taxSalary.ChildOrSpouseAmount,
                    TaxSalaryRanges=context.TaxSalaryRange.OrderBy(u=>u.Id).Select(u=>new TaxSalaryRange
                    {
                        Rate=u.TaxRate,
                        FromAmount=u.FromAmount,
                        ToAmount=u.ToAmount
                    }).ToList(),
                    Month=month,
                    OrganisationId=organisationId,
                    Currency=new Currency
                    {
                        Id=organisation.OrganisationBaseCurrency.BaseCurrencyId,
                        Code=organisation.OrganisationBaseCurrency.BaseCurrency.Code,
                        Name=organisation.OrganisationBaseCurrency.BaseCurrency.Name,
                        Symbole=organisation.OrganisationBaseCurrency.BaseCurrency.Symbole
                    },
                    TaxCurrency=new Currency
                    {
                        Id = organisation.OrganisationBaseCurrency.TaxCurrencyId,
                        Code = organisation.OrganisationBaseCurrency.TaxCurrency.Code,
                        Name = organisation.OrganisationBaseCurrency.TaxCurrency.Name,
                        Symbole = organisation.OrganisationBaseCurrency.TaxCurrency.Symbole
                    },
                    ExchangeRate=exchangeRate
                };
                var employees = (from u in context.Employee.Where(u => u.IsActive && u.OrganisationId == organisationId).ToList()
                                 let numberOfChildAndSpouse= (u.IsConfederationThatHosts?1:0) +u.NumberOfChild
                                 let salary =Math.Round(u.SalaryTypeId == (int)SalaryTypeEnum.Cross ? u.Salary : (!u.IsResidentEmployee ? GetCrossSalaryFromNet(u.Salary,payroll.NoneResidentRate) : GetCrossSalaryFromNet(u.Salary,payroll.ExchangeRate,payroll.TaxSalaryRanges)),0)
                                select new PayrollTax
                                {
                                    Employee = new Employee
                                    {
                                        Id = u.Id,
                                        DisplayName = u.DisplayName,
                                        JobTitle = u.JobTitle,
                                        Country = new Country
                                        {
                                            Id = u.CountryId,
                                            Alpha2Code = u.Country.Alpha2Code,
                                            Alpha3Code = u.Country.Alpha3Code,
                                            Name = u.Country.Name,
                                            Nationality = u.Country.Nationality
                                        },
                                        IsConfederationThatHosts=u.IsConfederationThatHosts,
                                        NumberOfChild=u.NumberOfChild,
                                        IsResidentEmployee=u.IsResidentEmployee
                                    },
                                    EmployeeId = u.Id,
                                    NumberOfChilds = u.NumberOfChild,
                                    OtherBenefit = null,
                                    OtherBenefitTaxDeduct = null,
                                    Date = DateTime.Now,
                                    TransactionDate = DateTime.Now,
                                    Salary = salary,
                                    DeductSalary=!u.IsResidentEmployee? GetNoneResidentDeductTax(salary,payroll.NoneResidentRate): GetDeductAmountWithChildrend(salary,payroll.TaxSalaryRanges, numberOfChildAndSpouse,payroll.ChildOrSpouseAmount,payroll.ExchangeRate)
                                    
                                }).ToList();

                payroll.Payrolls = employees;
                return payroll;
            }
            var payrollMonth = context.PayrollMonth.FirstOrDefault(u => u.Month == month && u.OrganisationId == organisationId);
            var result = new PayrollMonthTax
            {
                AdditionalBenefitsRate = payrollMonth.PayrollMonthTaxSalary.AdditionalBenefits,
                NoneResidentRate = payrollMonth.PayrollMonthTaxSalary.NoneResidentRate,
                ChildOrSpouseAmount = payrollMonth.PayrollMonthTaxSalary.ChildOrSpouseAmount,
                TaxSalaryRanges = payrollMonth.PayrollMonthTaxSalary.PayrollMonthTaxSalaryRange.OrderBy(u => u.TaxRate).Select(u => new TaxSalaryRange
                {
                    Rate = u.TaxRate,
                    FromAmount = u.FromAmount,
                    ToAmount = u.ToAmount
                }).ToList(),
                Month = month,
                OrganisationId = organisationId,
                Id=payrollMonth.Id,
                EndDate=payrollMonth.EndDate,
                StartDate=payrollMonth.StartDate,
                Total=payrollMonth.Total,
                Currency = new Currency
                {
                    Id = organisation.OrganisationBaseCurrency.BaseCurrencyId,
                    Code = organisation.OrganisationBaseCurrency.BaseCurrency.Code,
                    Name = organisation.OrganisationBaseCurrency.BaseCurrency.Name,
                    Symbole = organisation.OrganisationBaseCurrency.BaseCurrency.Symbole
                },
                TaxCurrency = new Currency
                {
                    Id = organisation.OrganisationBaseCurrency.TaxCurrencyId,
                    Code = organisation.OrganisationBaseCurrency.TaxCurrency.Code,
                    Name = organisation.OrganisationBaseCurrency.TaxCurrency.Name,
                    Symbole = organisation.OrganisationBaseCurrency.TaxCurrency.Symbole
                },
                ExchangeRate = (decimal)payrollMonth.ExchangeRate,
                Payrolls=payrollMonth.PayrollEmployee.Select(u=>new PayrollTax
                {
                    Date=u.Date,
                    DeductSalary=u.PayrollEmployeeTax.SalaryTax,
                    Employee = new Employee
                    {
                        Id = u.Id,
                        DisplayName = u.Employee.DisplayName,
                        JobTitle = u.Employee.JobTitle,
                        Country = new Country
                        {
                            Id = u.Employee.CountryId,
                            Alpha2Code = u.Employee.Country.Alpha2Code,
                            Alpha3Code = u.Employee.Country.Alpha3Code,
                            Name = u.Employee.Country.Name,
                            Nationality = u.Employee.Country.Nationality
                        },
                        IsConfederationThatHosts = u.PayrollEmployeeTax.ConfederationThatHosts,
                        NumberOfChild = u.PayrollEmployeeTax.NumberOfChilds,
                        IsResidentEmployee = u.PayrollEmployeeTax.IsResidentEmployee,
                        
                    },
                    EmployeeId=u.EmployeeId,
                    NumberOfChilds=u.PayrollEmployeeTax.NumberOfChilds,
                    OtherBenefit=u.PayrollEmployeeTax.OtherBenefit,
                    OtherBenefitTaxDeduct=u.PayrollEmployeeTax.OtherBenefitTaxDeduct,
                    Salary=u.PayrollEmployeeTax.Salary,
                    TransactionDate=u.TransactionDate,
                    Total=u.Total,
                    Id=u.Id

                }).ToList()
            };
            return result;
        }

        public PayrollMonthNoneTax GetPayrollMonth(string organisationId, string month)
        {
            using var context = Context();
            var organisation = context.Organisation.FirstOrDefault(u => u.Id == organisationId);
            if (!context.PayrollMonth.Any(u => u.Month == month && u.OrganisationId == organisationId))
            {

               
                var payroll = new PayrollMonthNoneTax
                {
                    Month = month,
                    OrganisationId = organisationId,
                    Currency = new Currency
                    {
                        Id = organisation.OrganisationBaseCurrency.BaseCurrencyId,
                        Code = organisation.OrganisationBaseCurrency.BaseCurrency.Code,
                        Name = organisation.OrganisationBaseCurrency.BaseCurrency.Name,
                        Symbole = organisation.OrganisationBaseCurrency.BaseCurrency.Symbole
                    },
                };
                var employees = (from u in context.Employee.Where(u => u.IsActive && u.OrganisationId == organisationId).ToList()
                                 let numberOfChildAndSpouse = (u.IsConfederationThatHosts ? 1 : 0) + u.NumberOfChild
                                 select new PayrollNoneTax
                                 {
                                     Employee = new Employee
                                     {
                                         Id = u.Id,
                                         DisplayName = u.DisplayName,
                                         JobTitle = u.JobTitle,
                                         Country = new Country
                                         {
                                             Id = u.CountryId,
                                             Alpha2Code = u.Country.Alpha2Code,
                                             Alpha3Code = u.Country.Alpha3Code,
                                             Name = u.Country.Name,
                                             Nationality = u.Country.Nationality
                                         },
                                         IsConfederationThatHosts = u.IsConfederationThatHosts,
                                         NumberOfChild = u.NumberOfChild,
                                         IsResidentEmployee = u.IsResidentEmployee
                                     },
                                     EmployeeId = u.Id,
                                     OtherBenefit = null,
                                     Date = DateTime.Now,
                                     TransactionDate = DateTime.Now,
                                     Salary = u.Salary,

                                 }).ToList();

                payroll.Payrolls = employees;
                return payroll;
            }
            var payrollMonth = context.PayrollMonth.FirstOrDefault(u => u.Month == month && u.OrganisationId == organisationId);
            var result = new PayrollMonthNoneTax
            {
                Month = month,
                OrganisationId = organisationId,
                Id = payrollMonth.Id,
                EndDate = payrollMonth.EndDate,
                StartDate = payrollMonth.StartDate,
                Total = payrollMonth.Total,
                Currency = new Currency
                {
                    Id = organisation.OrganisationBaseCurrency.BaseCurrencyId,
                    Code = organisation.OrganisationBaseCurrency.BaseCurrency.Code,
                    Name = organisation.OrganisationBaseCurrency.BaseCurrency.Name,
                    Symbole = organisation.OrganisationBaseCurrency.BaseCurrency.Symbole
                },
                ExchangeRate = (decimal)payrollMonth.ExchangeRate,
                Payrolls = payrollMonth.PayrollEmployee.Select(u => new PayrollNoneTax
                {
                    Date = u.Date,
                    Employee = new Employee
                    {
                        Id = u.Id,
                        DisplayName = u.Employee.DisplayName,
                        JobTitle = u.Employee.JobTitle,
                        Country = new Country
                        {
                            Id = u.Employee.CountryId,
                            Alpha2Code = u.Employee.Country.Alpha2Code,
                            Alpha3Code = u.Employee.Country.Alpha3Code,
                            Name = u.Employee.Country.Name,
                            Nationality = u.Employee.Country.Nationality
                        },

                    },
                    EmployeeId = u.EmployeeId,
                    OtherBenefit = u.PayrollEmployeeNoneTax.OtherBenefit,
                    Salary = u.PayrollEmployeeNoneTax.Salary,
                    TransactionDate = u.TransactionDate,
                    Total = u.Total,
                    Id = u.Id

                }).ToList()
            };
            return result;
        }
        decimal GetCrossSalaryFromNet(decimal netSalary, decimal residentRate)
        {
            decimal result = netSalary;
            while (result - (result * residentRate) / 100 < netSalary)
            {
                if (netSalary > 100000)
                {
                    result += 1000;
                }
                else
                {
                    result += 1;
                }

            }
            return result;
        }
        decimal GetCrossSalaryFromNet(decimal netSalary, decimal exchangeRate, List<TaxSalaryRange> taxSalaryRanges)
        {
            decimal crossSalary = netSalary * exchangeRate;
            crossSalary += 1000000;
            var rate = FindingRate(crossSalary, taxSalaryRanges);
            var deductAmount = GetDeductAmount(crossSalary, taxSalaryRanges, rate);
            var calNet = crossSalary - deductAmount;
            netSalary = netSalary * exchangeRate;
            if (calNet > netSalary)
            {
                while (calNet > netSalary)
                {
                    crossSalary -= 500;
                    rate = FindingRate(crossSalary, taxSalaryRanges);
                    deductAmount = GetDeductAmount(crossSalary, taxSalaryRanges, rate);
                    calNet = crossSalary - deductAmount;
                }

            }
            else
            {
                while (calNet < netSalary)
                {
                    crossSalary += 500;
                    rate = FindingRate(crossSalary, taxSalaryRanges);
                    deductAmount = GetDeductAmount(crossSalary, taxSalaryRanges, rate);
                    calNet = crossSalary - deductAmount;
                }
            }
            var amount = (netSalary + deductAmount) / exchangeRate;
            return amount;
        }
        double FindingRate(decimal salary, List<TaxSalaryRange> taxSalaryRanges)
        {
            double result = 0;
            foreach (var range in taxSalaryRanges)
            {
                if (range.ToAmount == null && salary>=range.FromAmount)
                {
                    result = range.Rate;
                    break;
                }
                if (salary >= range.FromAmount && salary <= range.ToAmount)
                {
                    result = range.Rate;
                    break;
                }
            }
            return result;
        }
        decimal GetDeductAmount(decimal salary, List<TaxSalaryRange> taxSalaryRanges, double rate)
        {
            decimal deductAmountFromPreviuseRate = 0;

            for (int i = 0; i < taxSalaryRanges.Count; i++)
            {
                if (taxSalaryRanges[i].Rate >= rate)
                {
                    break;
                }
                if (taxSalaryRanges[i].Rate > 0)
                {
                    var amount = taxSalaryRanges[i].ToAmount.Value - taxSalaryRanges[i - 1].ToAmount.Value;
                    deductAmountFromPreviuseRate += (amount * (decimal)taxSalaryRanges[i].Rate) / 100;
                }
            }
            decimal deductAmount = 0;
            for (int i = 0; i < taxSalaryRanges.Count; i++)
            {
                if (taxSalaryRanges[i].Rate == rate)
                {
                    decimal amount = salary - taxSalaryRanges[i].FromAmount;
                    deductAmount = (amount * (decimal)rate) / 100;
                    break;
                }
            }
            deductAmount += deductAmountFromPreviuseRate;
            return deductAmount;
        }
        decimal GetNoneResidentDeductTax(decimal salary, decimal taxRate)
        {
            return (salary * taxRate) / 100;
        }
        decimal GetDeductAmountWithChildrend(decimal salary, List<TaxSalaryRange> taxSalaryRanges,int numberOfChildAndSpouse, decimal childOrSpouseDeductAmount,decimal exchangeRate)
        {
            salary = salary * exchangeRate;
            if (numberOfChildAndSpouse > 0)
            {
                salary -= numberOfChildAndSpouse * childOrSpouseDeductAmount;
            }
            var rate = FindingRate(salary, taxSalaryRanges);
            return GetDeductAmount(salary, taxSalaryRanges, rate)/exchangeRate;
        }

        public void CreatePayroll(PayrollMonthTax payrollMonthTax, string organisationId, string month)
        {
            using var context = Context();
            if (!context.Organisation.FirstOrDefault(u => u.Id == organisationId).DeclareTax)
            {
                throw new Exception("This organisation is not declare tax");
            }
            var payroll = context.PayrollMonth.FirstOrDefault(u => u.OrganisationId == organisationId && u.Month == month);
            var date = CurrentCambodiaTime;
            if (date.Year.ToString() + date.Month.ToString("00") != month)
            {
                date = new DateTime(int.Parse(month.Substring(0, 4)), int.Parse(month.Substring(4, 2)), 1, 20, 0, 0, 0);
            }
            if (payroll == null)
            {
                payroll = new Dal.Models.PayrollMonth
                {
                    Month = month,
                    EndDate = CurrentCambodiaTime,
                    StartDate = CurrentCambodiaTime,
                    ExchangeRate = (double)payrollMonthTax.ExchangeRate,
                    OrganisationId = organisationId,
                    PayrollMonthTaxSalary = new Dal.Models.PayrollMonthTaxSalary
                    {
                        AdditionalBenefits = payrollMonthTax.AdditionalBenefitsRate,
                        ChildOrSpouseAmount = payrollMonthTax.ChildOrSpouseAmount,
                        NoneResidentRate = payrollMonthTax.NoneResidentRate,
                        PayrollMonthTaxSalaryRange = payrollMonthTax.TaxSalaryRanges.Select(u => new Dal.Models.PayrollMonthTaxSalaryRange
                        {
                            FromAmount=u.FromAmount,
                            TaxRate=u.Rate,
                            ToAmount=u.ToAmount,
                           
                        }).ToHashSet()
                    },
                    Total=payrollMonthTax.Payrolls.Sum(u=>u.Salary+(u.OtherBenefit==null?0:u.OtherBenefit.Value))
                };
                payroll.PayrollEmployee = payrollMonthTax.Payrolls.Select(u => new Dal.Models.PayrollEmployee
                {
                    EmployeeId=u.EmployeeId,
                    Date= date,
                    Total =u.Salary + (u.OtherBenefit==null?0:u.OtherBenefit.Value),
                    TransactionDate=CurrentCambodiaTime,
                    PayrollEmployeeTax=new Dal.Models.PayrollEmployeeTax
                    {
                        OtherBenefit=u.OtherBenefit,
                        SalaryTax=u.DeductSalary,
                        ConfederationThatHosts=u.Employee.IsConfederationThatHosts,
                        IsResidentEmployee=u.Employee.IsResidentEmployee,
                        NumberOfChilds=u.Employee.NumberOfChild,
                        OtherBenefitTaxDeduct=u.OtherBenefitTaxDeduct,
                        Salary=u.Salary
                    }
                }).ToHashSet();
                context.PayrollMonth.Add(payroll);
                context.SaveChanges();
                payrollMonthTax.Id = payroll.Id;
                payrollMonthTax.Payrolls = context.PayrollEmployeeTax.Where(u=>u.IdNavigation.PayrollMonthId==payrollMonthTax.Id).Select(u => new PayrollTax
                {
                    Date=u.IdNavigation.Date,
                    DeductSalary=u.SalaryTax,
                    EmployeeId=u.IdNavigation.EmployeeId,
                    Employee = new Employee
                    {
                        Id = u.IdNavigation.EmployeeId,
                        DisplayName = u.IdNavigation.Employee.DisplayName,
                        JobTitle = u.IdNavigation.Employee.JobTitle,
                        Country = new Country
                        {
                            Id = u.IdNavigation.Employee.CountryId,
                            Alpha2Code = u.IdNavigation.Employee.Country.Alpha2Code,
                            Alpha3Code = u.IdNavigation.Employee.Country.Alpha3Code,
                            Name = u.IdNavigation.Employee.Country.Name,
                            Nationality = u.IdNavigation.Employee.Country.Nationality
                        },
                        IsConfederationThatHosts = u.ConfederationThatHosts,
                        NumberOfChild = u.NumberOfChilds,
                        IsResidentEmployee = u.IsResidentEmployee
                    },
                    NumberOfChilds=u.NumberOfChilds,
                    OtherBenefit=u.OtherBenefit,
                    Id=u.Id,
                    OtherBenefitTaxDeduct=u.OtherBenefitTaxDeduct,
                    Salary=u.Salary,
                    Total=u.IdNavigation.Total,
                    TransactionDate=u.IdNavigation.TransactionDate
                }).ToList();
                return;
            }
            payroll.EndDate = CurrentCambodiaTime;
            payroll.Total = payrollMonthTax.Payrolls.Sum(u => u.Salary  + (u.OtherBenefit == null ? 0 : u.OtherBenefit.Value));
            payroll.ExchangeRate =(double) payrollMonthTax.ExchangeRate;
            foreach(var p in payrollMonthTax.Payrolls)
            {
                var updatePayroll = context.PayrollEmployee.FirstOrDefault(u => u.Id == p.Id);
                updatePayroll.Total = p.Salary  + (p.OtherBenefit == null ? 0 : p.OtherBenefit.Value);
                updatePayroll.TransactionDate = CurrentCambodiaTime;
                updatePayroll.Date = date;
                updatePayroll.PayrollEmployeeTax.ConfederationThatHosts = p.Employee.IsConfederationThatHosts;
                updatePayroll.PayrollEmployeeTax.IsResidentEmployee = p.Employee.IsResidentEmployee;
                updatePayroll.PayrollEmployeeTax.NumberOfChilds = p.Employee.NumberOfChild;
                updatePayroll.PayrollEmployeeTax.OtherBenefit = p.OtherBenefit;
                updatePayroll.PayrollEmployeeTax.Salary = p.Salary;
                updatePayroll.PayrollEmployeeTax.SalaryTax = p.DeductSalary;
                updatePayroll.PayrollEmployeeTax.OtherBenefitTaxDeduct = p.OtherBenefitTaxDeduct;
            }
            context.SaveChanges();
        }

        public void CreatePayroll(PayrollMonthNoneTax payrollNoneTax, string organisationId, string month)
        {
            using var context = Context();
            if (context.Organisation.FirstOrDefault(u => u.Id == organisationId).DeclareTax)
            {
                throw new Exception("This organisation is declare tax");
            }
            var payroll = context.PayrollMonth.FirstOrDefault(u => u.OrganisationId == organisationId && u.Month == month);
            var date = CurrentCambodiaTime;
            if (date.Year.ToString() + date.Month.ToString("00") != month)
            {
                date = new DateTime(int.Parse(month.Substring(0, 4)), int.Parse(month.Substring(4, 2)), 1, 20, 0, 0, 0);
            }
            if (payroll == null)
            {
                payroll = new Dal.Models.PayrollMonth
                {
                    Month = month,
                    EndDate = CurrentCambodiaTime,
                    StartDate = CurrentCambodiaTime,
                    ExchangeRate = 1,
                    OrganisationId = organisationId,
                   
                    Total = payrollNoneTax.Payrolls.Sum(u => u.Salary + (u.OtherBenefit == null ? 0 : u.OtherBenefit.Value))
                };
                payroll.PayrollEmployee = payrollNoneTax.Payrolls.Select(u => new Dal.Models.PayrollEmployee
                {
                    EmployeeId = u.EmployeeId,
                    Date = date,
                    Total = u.Salary + (u.OtherBenefit == null ? 0 : u.OtherBenefit.Value),
                    TransactionDate = CurrentCambodiaTime,
                   PayrollEmployeeNoneTax=new Dal.Models.PayrollEmployeeNoneTax
                   {
                       Salary=u.Salary,
                       OtherBenefit=u.OtherBenefit
                   }
                }).ToHashSet();
                context.PayrollMonth.Add(payroll);
                context.SaveChanges();
                payrollNoneTax.Id = payroll.Id;
                payrollNoneTax.Payrolls = context.PayrollEmployeeNoneTax.Where(u=>u.IdNavigation.PayrollMonthId==payrollNoneTax.Id).Select(u => new PayrollNoneTax
                {
                    Date = u.IdNavigation.Date,
                    EmployeeId = u.IdNavigation.EmployeeId,
                    Employee = new Employee
                    {
                        Id = u.IdNavigation.EmployeeId,
                        DisplayName = u.IdNavigation.Employee.DisplayName,
                        JobTitle = u.IdNavigation.Employee.JobTitle,
                        Country = new Country
                        {
                            Id = u.IdNavigation.Employee.CountryId,
                            Alpha2Code = u.IdNavigation.Employee.Country.Alpha2Code,
                            Alpha3Code = u.IdNavigation.Employee.Country.Alpha3Code,
                            Name = u.IdNavigation.Employee.Country.Name,
                            Nationality = u.IdNavigation.Employee.Country.Nationality
                        }
                    },
                    OtherBenefit = u.OtherBenefit,
                    Id = u.Id,
                    Salary = u.Salary,
                    Total = u.IdNavigation.Total,
                    TransactionDate = u.IdNavigation.TransactionDate
                }).ToList();
                return;
            }
            payroll.EndDate = CurrentCambodiaTime;
            payroll.Total = payrollNoneTax.Payrolls.Sum(u => u.Salary + (u.OtherBenefit == null ? 0 : u.OtherBenefit.Value));
            foreach (var p in payrollNoneTax.Payrolls)
            {
                var updatePayroll = context.PayrollEmployee.FirstOrDefault(u => u.Id == p.Id);
                updatePayroll.Total = p.Salary + (p.OtherBenefit == null ? 0 : p.OtherBenefit.Value);
                updatePayroll.PayrollEmployeeNoneTax.OtherBenefit = p.OtherBenefit;
                updatePayroll.PayrollEmployeeNoneTax.Salary = p.Salary;
            }
            context.SaveChanges();
        }
    }
}
