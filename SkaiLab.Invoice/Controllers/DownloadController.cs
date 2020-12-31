using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using SkaiLab.Invoice.Models;
using SkaiLab.Invoice.Service;

namespace SkaiLab.Invoice.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class DownloadController : ParentController
    {
        private readonly IOrganisationService organisationService;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IReportService reportService;
        public DownloadController(IOrganisationService organisationService, IWebHostEnvironment webHostEnvironment, IReportService reportService):base(organisationService,(int)MenuFeatureEnum.Report)
        {
            this.organisationService = organisationService;
            this.webHostEnvironment = webHostEnvironment;
            this.reportService = reportService;
        }

        [HttpGet("[action]/{month}")]
        public IActionResult DownloadTax(string month)
        {
            var organisaction = organisationService.Get(organisationService.OrganisationId);
            var url = Request.Scheme + "://" + Request.Host;
            var temPath = Path.Combine(webHostEnvironment.WebRootPath, "Temp");
            if (!Directory.Exists(temPath))
            {
                Directory.CreateDirectory(temPath);
            }
            var files = new DirectoryInfo(temPath).GetFiles().Where(u => u.CreationTime < DateTime.Now.AddHours(-4)).Select(u => u);
            foreach (var file in files)
            {
                file.Delete();
            }
            var folderName = Guid.NewGuid().ToString();
            temPath = Path.Combine(temPath, folderName);
            Directory.CreateDirectory(temPath);
            var templatPath = Path.Combine(webHostEnvironment.WebRootPath, "template", "tax");
            var tax = reportService.GetTaxMonthly(month, reportService.OrganisationId);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(new FileInfo(Path.Combine(templatPath, "Salary.xlsx"))))
            {
                var sheet = package.Workbook.Worksheets[0];
                sheet.Name = "Salary-" + month;
                sheet.Cells[1, 1].Value = "ប្រាក់កាត់ទុកពីប្រាក់បៀវត្ស ប្រចាំខែ "+Utils.ConvertInternationalNumberToKhmer(month.Substring(4))+ " ឆ្នាំ "+ Utils.ConvertInternationalNumberToKhmer(month.Substring(0,4));
                sheet.Cells[2, 1].Value = organisaction.LegalLocalName;
                sheet.Cells[3, 10].Value = "លេខអត្តសញ្ញាណកម្មៈ " + organisaction.TaxNumber;
                int i = 8;
                if (tax.Payroll.Payrolls.Any())
                {
                    if (tax.Payroll.ExchangeRate == 1)
                    {
                        sheet.Cells[4, 13].Value = "";
                        sheet.Cells[4, 14].Value = "";
                    }
                    else
                    {
                        sheet.Cells[4, 14].Value = tax.Payroll.ExchangeRate + " R/$";
                    }

                    foreach (var emp in tax.Payroll.Payrolls)
                    {
                        sheet.Cells[i, 1].Value = i - 7;
                        sheet.Cells[i, 2].Value = emp.Employee.IDOrPassportNumber;
                        sheet.Cells[i, 3].Value = emp.Employee.DisplayName;
                        sheet.Cells[i, 4].Value = emp.Employee.Country.Nationality;
                        sheet.Cells[i, 5].Value = emp.Employee.JobTitle;
                        if (tax.Payroll.ExchangeRate == 1)
                        {
                            sheet.Cells[i, 6].Value = "-";
                        }
                        else
                        {
                            sheet.Cells[i, 6].Value = tax.Payroll.Currency.Symbole + " " + Math.Round(emp.Salary, 0);
                        }
                        sheet.Cells[i, 7].Value = Math.Round(emp.Salary * tax.Payroll.ExchangeRate, 0);
                        if (emp.Employee.IsResidentEmployee)
                        {
                            sheet.Cells[i, 8].Value = emp.Employee.NumberOfChild;
                        }
                        else
                        {
                            sheet.Cells[i, 8].Value = "-";
                        }
                        if (emp.Employee.IsResidentEmployee)
                        {
                            sheet.Cells[i, 9].Value = emp.Employee.IsConfederationThatHosts ? "Yes" : "No";
                        }
                        else
                        {
                            sheet.Cells[i, 9].Value = "-";
                        }
                        if (emp.OtherBenefit == null)
                        {
                            sheet.Cells[i, 10].Value = "-";
                        }
                        else
                        {
                            sheet.Cells[i, 10].Value = Math.Round(emp.OtherBenefit.Value * tax.Payroll.ExchangeRate, 0);
                        }
                        sheet.Cells[i, 11].Value = Math.Round(emp.DeductSalary * tax.Payroll.ExchangeRate, 0);
                        if (emp.OtherBenefitTaxDeduct == null)
                        {
                            sheet.Cells[i, 12].Value = "-";
                        }
                        else
                        {
                            sheet.Cells[i, 12].Value = Math.Round(emp.OtherBenefitTaxDeduct.Value * tax.Payroll.ExchangeRate, 0);
                        }

                        var totalTax = emp.DeductSalary * tax.Payroll.ExchangeRate;
                        if (emp.OtherBenefit != null)
                        {
                            totalTax += emp.OtherBenefitTaxDeduct.Value * tax.Payroll.ExchangeRate;
                        }
                        sheet.Cells[i, 13].Value = Math.Round(totalTax, 0);
                        for (int k = 1; k <= 14; k++)
                        {
                            sheet.Cells[i, k].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        }
                        i++;
                    }
                    sheet.Cells[i, 1, i, 5].Merge = true;
                    sheet.Cells[i, 1].Value = "Total:";
                    sheet.Cells[i, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    if (tax.Payroll.ExchangeRate == 1)
                    {
                        sheet.Cells[i, 6].Value = "-";
                    }
                    else
                    {
                        sheet.Cells[i, 6].Value = tax.Payroll.Currency.Symbole + " " + Math.Round(tax.Payroll.Payrolls.Sum(u => u.Salary), 0);
                    }
                    sheet.Cells[i, 7].Value = Math.Round(tax.Payroll.Payrolls.Sum(u => u.Salary * tax.Payroll.ExchangeRate), 0);
                    sheet.Cells[i, 8].Value = "-";
                    sheet.Cells[i, 9].Value = "-";
                    sheet.Cells[i, 10].Value = Math.Round(tax.Payroll.Payrolls.Where(u => u.OtherBenefit != null).Sum(u => u.OtherBenefit.Value * tax.Payroll.ExchangeRate), 0);
                    sheet.Cells[i, 11].Value = Math.Round(tax.Payroll.Payrolls.Sum(u => u.DeductSalary * tax.Payroll.ExchangeRate), 0);
                    sheet.Cells[i, 12].Value = Math.Round(tax.Payroll.Payrolls.Where(u => u.OtherBenefitTaxDeduct != null).Sum(u => u.OtherBenefitTaxDeduct.Value * tax.Payroll.ExchangeRate), 0);
                    sheet.Cells[i, 13].Value = Math.Round(tax.TotalEmployeeTaxInTaxCurrency, 0);
                    for (int k = 1; k <= 14; k++)
                    {
                        sheet.Cells[i, k].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheet.Cells[i, k].Style.Font.Bold = true;
                    }
                }
              
                i+=2;
                sheet.Cells[i, 1, i, 5].Merge = true;
                sheet.Cells[i, 1].Value = "ធ្វើនៅ ភ្នំពេញ ថ្ងៃទី "+Utils.ConvertInternationalNumberToKhmer(Utils.CurrentCambodiaTime().Value.Day.ToString("00"))+ " ខែ " +Utils.GetKhmerMonthName(Utils.CurrentCambodiaTime().Value)+ " ឆ្នាំ " + Utils.ConvertInternationalNumberToKhmer(Utils.CurrentCambodiaTime().Value.Year.ToString());
                sheet.Cells[i, 1].Style.Font.Name = "Khmer OS Siemreap";
                sheet.Cells[i, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                i++;
                sheet.Cells[i, 1, i, 5].Merge = true;
                sheet.Cells[i, 1].Value = "ហត្ថលេខា";
                sheet.Cells[i, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                sheet.Cells[i, 1].Style.Font.Name = "Khmer OS Siemreap";
                package.SaveAs(new FileInfo(Path.Combine(Path.Combine(temPath,"Salary-"+month+".xlsx"))));
            }
            using (var package = new ExcelPackage(new FileInfo(Path.Combine(templatPath, "Purchase.xlsx"))))
            {
                var sheet = package.Workbook.Worksheets[0];
                sheet.Name = "Purchase " + month;
                sheet.Cells[2, 1].Value = "ខែ "+Utils.ConvertInternationalNumberToKhmer(month.Substring(4))+ "  ឆ្នាំ  "+Utils.ConvertInternationalNumberToKhmer(month.Substring(0,4));
                sheet.Cells[3, 1].Value = "ឈ្មោះសហគ្រាស៖ "+organisaction.LegalLocalName;
                sheet.Cells[4, 1].Value = organisaction.Contact.Address;
                sheet.Cells[4, 8].Value = "លេខអត្តសញ្ញាណកម្មៈ "+organisaction.TaxNumber;
                int i = 10;
                var expenseItems = reportService.GetExpenseItemsTax(tax.Expenses.Select(u => u.Id).ToList());
                decimal totalInBaseCurrency = 0;
                decimal totalInTaxCurrency = 0;
                decimal totalTax = 0;
                foreach(var expense in tax.Expenses)
                {
                    var itemsInExpense = expenseItems.Where(u => u.ExpenseId == expense.Id);
                    sheet.Cells[i, 1].Value = expense.Date;
                    sheet.Cells[i, 2].Value =string.IsNullOrEmpty(expense.RefNo)? expense.OrderNumber:expense.RefNo;
                    sheet.Cells[i, 3].Value = string.IsNullOrEmpty(expense.Vendor.LocalLegalName) && string.IsNullOrEmpty(expense.Vendor.LegalName) 
                        ? expense.Vendor.DisplayName :
                        (string.IsNullOrEmpty(expense.Vendor.LocalLegalName)?expense.Vendor.LegalName:expense.Vendor.LocalLegalName);
                    sheet.Cells[i, 4].Value = expense.Vendor.TaxNumber;
                    sheet.Cells[i, 5].Value = itemsInExpense.First().Description;
                    sheet.Cells[i, 6].Value = "-";
                    if (expense.TaxCurrencyExchangeRate == 1)
                    {
                        sheet.Cells[i, 7].Value = "-";
                    }
                    else
                    {
                        sheet.Cells[i, 7].Value = expense.TaxCurrencyExchangeRate;
                    }
                    if (expense.TaxCurrencyExchangeRate == 1)
                    {
                        sheet.Cells[i, 8].Value = "-";
                    }
                    else
                    {
                        sheet.Cells[i, 8].Value =expense.Currency.Symbole + " "+Math.Round(expense.Total,0);
                    }
                    sheet.Cells[i, 9].Value =Math.Round(expense.Total * expense.TaxCurrencyExchangeRate);

                    Dictionary<int, int> dicRate = new Dictionary<int, int>();
                    foreach(var t in itemsInExpense)
                    {
                        foreach(var r in t.Tax.Components)
                        {
                            if (!dicRate.ContainsKey((int)r.Rate))
                            {
                                dicRate.Add((int)r.Rate, (int)r.Rate);
                            }
                        }
                    }
                    string rateText = "";
                    foreach(var r in dicRate)
                    {
                        if (rateText != "")
                        {
                            rateText += ",";
                        }
                        rateText += r.Key+"%" ;
                    }

                    sheet.Cells[i, 10].Value = rateText;
                    sheet.Cells[i, 11].Value =Math.Round((expense.TotalIncludeTax-expense.Total) * expense.TaxCurrencyExchangeRate,0);
                    sheet.Cells[i, 12].Value = Math.Round(expense.TotalIncludeTax * expense.TaxCurrencyExchangeRate,0);
                    totalInBaseCurrency += expense.Total * expense.BaseCurrencyExchangeRate;
                    totalInTaxCurrency += expense.Total * expense.TaxCurrencyExchangeRate;
                    totalTax += (expense.TotalIncludeTax - expense.Total) *expense.TaxCurrencyExchangeRate;
                    for (int j = 1; j <= 12; j++)
                    {
                        sheet.Cells[i, j].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    }
                    i++;

                }
                sheet.Cells[i, 1, i, 7].Merge = true;
                sheet.Cells[i, 1].Value = "Total:";
                sheet.Cells[i, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                sheet.Cells[i, 8].Value = tax.Currency.Symbole + " " + Math.Round(totalInBaseCurrency, 2);
                sheet.Cells[i, 9].Value =  Math.Round(totalInTaxCurrency, 0);
                sheet.Cells[i, 10].Value = "-";
                sheet.Cells[i, 11].Value = Math.Round(totalTax, 0);
                sheet.Cells[i, 12].Value = Math.Round(totalInTaxCurrency+ totalTax, 0);
                for (int j = 1; j <= 12; j++)
                {
                    sheet.Cells[i, j].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheet.Cells[i, j].Style.Font.Bold = true;
                }
                i += 2;
                sheet.Cells[i, 7, i, 12].Merge = true;
                sheet.Cells[i, 7].Value = "ធ្វើនៅ ភ្នំពេញ ថ្ងៃទី " + Utils.ConvertInternationalNumberToKhmer(Utils.CurrentCambodiaTime().Value.Day.ToString("00")) + " ខែ " + Utils.GetKhmerMonthName(Utils.CurrentCambodiaTime().Value) + " ឆ្នាំ " + Utils.ConvertInternationalNumberToKhmer(Utils.CurrentCambodiaTime().Value.Year.ToString());
                sheet.Cells[i, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                sheet.Cells[i, 7].Style.Font.Name = "Khmer OS Siemreap";
                i++;
                sheet.Cells[i, 7, i, 12].Merge = true;
                sheet.Cells[i, 7].Value = "ហត្ថលេខា";
                sheet.Cells[i, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                sheet.Cells[i, 7].Style.Font.Name = "Khmer OS Siemreap";
                package.SaveAs(new FileInfo(Path.Combine(Path.Combine(temPath, "Purchase-" + month + ".xlsx"))));
            }
            using (var package = new ExcelPackage(new FileInfo(Path.Combine(templatPath, "Sale.xlsx"))))
            {
                var sheet = package.Workbook.Worksheets[0];
                sheet.Name = "Sale " + month;
                sheet.Cells[2, 1].Value = "ខែ " + Utils.ConvertInternationalNumberToKhmer(month.Substring(4)) + "  ឆ្នាំ  " + Utils.ConvertInternationalNumberToKhmer(month.Substring(0, 4));
                sheet.Cells[3, 1].Value = "ឈ្មោះសហគ្រាស៖ " + organisaction.LegalLocalName;
                sheet.Cells[4, 1].Value = organisaction.Contact.Address;
                sheet.Cells[4, 8].Value = "លេខអត្តសញ្ញាណកម្មៈ " + organisaction.TaxNumber;

                var invoiceItems = reportService.GetInvoiceItems(tax.Invoices.Select(u => u.Id).ToList());
                decimal totalInBaseCurrency = 0;
                decimal totalInTaxCurrency = 0;
                decimal totalTax = 0;
                int i = 10;
                foreach (var invoice in tax.Invoices)
                {
                    var itemsInExpense = invoiceItems.Where(u => u.CustomerTransaction == invoice.Id);
                    sheet.Cells[i, 1].Value = invoice.Date;
                    sheet.Cells[i, 2].Value = invoice.Number;
                    sheet.Cells[i, 3].Value = string.IsNullOrEmpty(invoice.Customer.LocalLegalName) && string.IsNullOrEmpty(invoice.Customer.LegalName)
                        ? invoice.Customer.DisplayName :
                        (string.IsNullOrEmpty(invoice.Customer.LocalLegalName) ? invoice.Customer.LegalName : invoice.Customer.LocalLegalName);
                    sheet.Cells[i, 4].Value = invoice.Customer.TaxNumber;
                    sheet.Cells[i, 5].Value = itemsInExpense.First().Description;
                    sheet.Cells[i, 6].Value = "-";
                    if (invoice.TaxCurrencyExchangeRate == 1)
                    {
                        sheet.Cells[i, 7].Value = "-";
                    }
                    else
                    {
                        sheet.Cells[i, 7].Value = invoice.TaxCurrencyExchangeRate;
                    }
                    if (invoice.TaxCurrencyExchangeRate == 1)
                    {
                        sheet.Cells[i, 8].Value = "-";
                    }
                    else
                    {
                        sheet.Cells[i, 8].Value = invoice.Currency.Symbole + " " + Math.Round(invoice.Total, 0);
                    }
                    sheet.Cells[i, 9].Value = Math.Round(invoice.Total * invoice.TaxCurrencyExchangeRate);

                    Dictionary<int, int> dicRate = new Dictionary<int, int>();
                    foreach (var t in itemsInExpense)
                    {
                        foreach (var r in t.Tax.Components)
                        {
                            if (!dicRate.ContainsKey((int)r.Rate))
                            {
                                dicRate.Add((int)r.Rate, (int)r.Rate);
                            }
                        }
                    }
                    string rateText = "";
                    foreach (var r in dicRate)
                    {
                        if (rateText != "")
                        {
                            rateText += ",";
                        }
                        rateText += r.Key + "%";
                    }

                    sheet.Cells[i, 10].Value = rateText;
                    sheet.Cells[i, 11].Value = Math.Round((invoice.TotalIncludeTax - invoice.Total) * invoice.TaxCurrencyExchangeRate, 0);
                    sheet.Cells[i, 12].Value = Math.Round(invoice.TotalIncludeTax * invoice.TaxCurrencyExchangeRate, 0);
                    totalInBaseCurrency += invoice.Total * invoice.BaseCurrencyExchangeRate;
                    totalInTaxCurrency += invoice.Total * invoice.TaxCurrencyExchangeRate;
                    totalTax += (invoice.TotalIncludeTax - invoice.Total) * invoice.TaxCurrencyExchangeRate;
                    for (int j = 1; j <= 12; j++)
                    {
                        sheet.Cells[i, j].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    }
                    i++;

                }
                sheet.Cells[i, 1, i, 7].Merge = true;
                sheet.Cells[i, 1].Value = "Total:";
                sheet.Cells[i, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                sheet.Cells[i, 8].Value = tax.Currency.Symbole + " " + Math.Round(totalInBaseCurrency, 2);
                sheet.Cells[i, 9].Value = Math.Round(totalInTaxCurrency, 0);
                sheet.Cells[i, 10].Value = "-";
                sheet.Cells[i, 11].Value = Math.Round(totalTax, 0);
                sheet.Cells[i, 12].Value = Math.Round(totalInTaxCurrency + totalTax, 0);
                for (int j = 1; j <= 12; j++)
                {
                    sheet.Cells[i, j].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheet.Cells[i, j].Style.Font.Bold = true;
                }
                i += 2;
                sheet.Cells[i, 7, i, 12].Merge = true;
                sheet.Cells[i, 7].Value = "ធ្វើនៅ ភ្នំពេញ ថ្ងៃទី " + Utils.ConvertInternationalNumberToKhmer(Utils.CurrentCambodiaTime().Value.Day.ToString("00")) + " ខែ " + Utils.GetKhmerMonthName(Utils.CurrentCambodiaTime().Value) + " ឆ្នាំ " + Utils.ConvertInternationalNumberToKhmer(Utils.CurrentCambodiaTime().Value.Year.ToString());
                sheet.Cells[i, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                sheet.Cells[i, 7].Style.Font.Name = "Khmer OS Siemreap";
                i++;
                sheet.Cells[i, 7, i, 12].Merge = true;
                sheet.Cells[i, 7].Value = "ហត្ថលេខា";
                sheet.Cells[i, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                sheet.Cells[i, 7].Style.Font.Name = "Khmer OS Siemreap";
                package.SaveAs(new FileInfo(Path.Combine(Path.Combine(temPath, "Sale-" + month + ".xlsx"))));
            }
            using (var package = new ExcelPackage(new FileInfo(Path.Combine(templatPath, "Summary.xlsx"))))
            {
                var sheet = package.Workbook.Worksheets[0];
                sheet.Cells[2, 1].Value = organisaction.LegalLocalName;
                sheet.Cells[3, 1].Value = "ខែ " + Utils.ConvertInternationalNumberToKhmer(month.Substring(4)) + "  ឆ្នាំ  " + Utils.ConvertInternationalNumberToKhmer(month.Substring(0, 4));
                sheet.Cells[5, 2].Value = tax.Currency.Symbole + " " + Math.Round(tax.TotalInvoiceTaxInBaseCurrency, 2);
                sheet.Cells[6, 2].Value = tax.Currency.Symbole + " " + Math.Round(tax.TotalExpenseTaxInBaseCurrency, 2);
                sheet.Cells[7, 2].Value = tax.Currency.Symbole + " " + Math.Round(tax.TotalEmployeeTaxInBaseCurrency, 2);
                sheet.Cells[8, 2].Value = tax.Currency.Symbole + " " + Math.Round(tax.TotalPayToTax, 0);
                sheet.Cells[9, 2].Value = Math.Round(tax.TotalPayToTaxInKHR, 2)+ tax.TaxCurrency.Symbole;
                var invoiceSheet = package.Workbook.Worksheets[1];
                invoiceSheet.Cells[2, 1].Value = "ខែ " + Utils.ConvertInternationalNumberToKhmer(month.Substring(4)) + "  ឆ្នាំ  " + Utils.ConvertInternationalNumberToKhmer(month.Substring(0, 4));
                invoiceSheet.Cells[3, 1].Value = organisaction.LegalLocalName;
                int i = 6;
                foreach(var invoice in tax.Invoices)
                {
                    invoiceSheet.Cells[i, 1].Value = invoice.Date;
                    invoiceSheet.Cells[i, 2].Value = invoice.Number;
                    invoiceSheet.Cells[i, 3].Value = tax.Currency.Symbole+ " "+ Math.Round(invoice.TotalIncludeTax*invoice.BaseCurrencyExchangeRate,2);
                    invoiceSheet.Cells[i, 4].Value = tax.Currency.Symbole + " " + Math.Round((invoice.TotalIncludeTax -invoice.Total) * invoice.BaseCurrencyExchangeRate, 2);
                    if (invoice.TaxCurrencyExchangeRate == 1)
                    {
                        invoiceSheet.Cells[i, 5].Value = "-";
                    }
                    else
                    {
                        invoiceSheet.Cells[i, 5].Value = invoice.TaxCurrencyExchangeRate;
                    }
                    invoiceSheet.Cells[i, 6].Value = Math.Round((invoice.TotalIncludeTax - invoice.Total)*invoice.TaxCurrencyExchangeRate, 0);
                    for(int j = 1; j<= 6; j++)
                    {
                        invoiceSheet.Cells[i, j].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    }
                    i++;
                }
                invoiceSheet.Cells[i, 1, i, 2].Merge = true;
                invoiceSheet.Cells[i, 1].Value = "Total:";
                invoiceSheet.Cells[i, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                invoiceSheet.Cells[i, 3].Value = tax.Currency.Symbole + " "+Math.Round(tax.Invoices.Sum(u=>u.TotalIncludeTax *u.BaseCurrencyExchangeRate),2);
                invoiceSheet.Cells[i, 4].Value = tax.Currency.Symbole + " " + Math.Round(tax.Invoices.Sum(u => (u.TotalIncludeTax-u.Total) * u.BaseCurrencyExchangeRate), 2);
                invoiceSheet.Cells[i, 5].Value = "-";
                invoiceSheet.Cells[i, 6].Value = Math.Round(tax.Invoices.Sum(u => (u.TotalIncludeTax - u.Total) * u.TaxCurrencyExchangeRate), 2)+" " + tax.TaxCurrency.Symbole;
                for (int j = 1; j <= 6; j++)
                {
                    invoiceSheet.Cells[i, j].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    invoiceSheet.Cells[i, j].Style.Font.Bold = true;
                }
                var exepnseSheet = package.Workbook.Worksheets[2];
                exepnseSheet.Cells[2, 1].Value = "ខែ " + Utils.ConvertInternationalNumberToKhmer(month.Substring(4)) + "  ឆ្នាំ  " + Utils.ConvertInternationalNumberToKhmer(month.Substring(0, 4));
                exepnseSheet.Cells[3, 1].Value = organisaction.LegalLocalName;
                i = 6;
                foreach (var expense in tax.Expenses)
                {
                    exepnseSheet.Cells[i, 1].Value = expense.Date;
                    exepnseSheet.Cells[i, 2].Value = expense.OrderNumber;
                    exepnseSheet.Cells[i, 3].Value = tax.Currency.Symbole + " " + Math.Round(expense.TotalIncludeTax * expense.BaseCurrencyExchangeRate, 2);
                    exepnseSheet.Cells[i, 4].Value = tax.Currency.Symbole + " " + Math.Round((expense.TotalIncludeTax - expense.Total) * expense.BaseCurrencyExchangeRate, 2);
                    if (expense.TaxCurrencyExchangeRate == 1)
                    {
                        exepnseSheet.Cells[i, 5].Value = "-";
                    }
                    else
                    {
                        exepnseSheet.Cells[i, 5].Value = expense.TaxCurrencyExchangeRate;
                    }
                    exepnseSheet.Cells[i, 6].Value = Math.Round((expense.TotalIncludeTax - expense.Total) * expense.TaxCurrencyExchangeRate, 0);
                    for (int j = 1; j <= 6; j++)
                    {
                        exepnseSheet.Cells[i, j].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    }
                    i++;
                }
                exepnseSheet.Cells[i, 1, i, 2].Merge = true;
                exepnseSheet.Cells[i, 1].Value = "Total:";
                exepnseSheet.Cells[i, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                exepnseSheet.Cells[i, 3].Value = tax.Currency.Symbole + " " + Math.Round(tax.Expenses.Sum(u => u.TotalIncludeTax * u.BaseCurrencyExchangeRate), 2);
                exepnseSheet.Cells[i, 4].Value = tax.Currency.Symbole + " " + Math.Round(tax.Expenses.Sum(u => (u.TotalIncludeTax - u.Total) * u.BaseCurrencyExchangeRate), 2);
                exepnseSheet.Cells[i, 5].Value = "-";
                exepnseSheet.Cells[i, 6].Value = Math.Round(tax.Expenses.Sum(u => (u.TotalIncludeTax - u.Total) * u.TaxCurrencyExchangeRate), 2) + " " + tax.TaxCurrency.Symbole;
                for (int j = 1; j <= 6; j++)
                {
                    exepnseSheet.Cells[i, j].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    exepnseSheet.Cells[i, j].Style.Font.Bold = true;
                }
                var salarySheet = package.Workbook.Worksheets[3];
                salarySheet.Cells[2, 1].Value = "ខែ " + Utils.ConvertInternationalNumberToKhmer(month.Substring(4)) + "  ឆ្នាំ  " + Utils.ConvertInternationalNumberToKhmer(month.Substring(0, 4));
                salarySheet.Cells[3, 1].Value = organisaction.LegalLocalName;
                i = 6;
                foreach(var emp in tax.Payroll.Payrolls)
                {
                    salarySheet.Cells[i, 1].Value = emp.Employee.DisplayName;
                    salarySheet.Cells[i, 2].Value = tax.Currency.Symbole + " " + Math.Round(emp.Salary,2);
                    if (emp.OtherBenefit == null)
                    {
                        salarySheet.Cells[i, 3].Value = "-";
                    }
                    else
                    {
                        salarySheet.Cells[i, 3].Value = tax.Currency.Symbole + " " + Math.Round(emp.OtherBenefit.Value, 2); 
                    }
                    var totalTax = emp.DeductSalary;
                    if (emp.OtherBenefitTaxDeduct != null)
                    {
                        totalTax += emp.OtherBenefitTaxDeduct.Value;
                    }
                    salarySheet.Cells[i, 4].Value = tax.Currency.Symbole + " "+Math.Round(totalTax,2);
                    if (tax.Payroll.ExchangeRate == 1)
                    {
                        salarySheet.Cells[i, 5].Value = "-";
                    }
                    else
                    {
                        salarySheet.Cells[i, 5].Value = tax.Payroll.ExchangeRate;
                    }
                    salarySheet.Cells[i, 6].Value = Math.Round(totalTax * tax.Payroll.ExchangeRate, 0) + " " + tax.TaxCurrency.Symbole;
                    for(int j = 1; j <= 6; j++)
                    {
                        salarySheet.Cells[i, j].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    }
                    i++;
                }
                salarySheet.Cells[i, 1].Value = "Total:";
                salarySheet.Cells[i, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                salarySheet.Cells[i, 2].Value =tax.Currency.Symbole+ " "+ Math.Round(tax.Payroll.Payrolls.Sum(u=>u.Salary), 2);
                salarySheet.Cells[i, 3].Value = tax.Currency.Symbole + " " + Math.Round(tax.Payroll.Payrolls.Where(u=>u.OtherBenefit!=null).Sum(u => u.OtherBenefit.Value), 2);
                var totalPayTax = tax.Payroll.Payrolls.Sum(u => u.DeductSalary + (u.OtherBenefitTaxDeduct == null ? 0 : u.OtherBenefitTaxDeduct.Value));
                salarySheet.Cells[i, 4].Value = tax.Currency.Symbole + " " + Math.Round(totalPayTax, 2);
                salarySheet.Cells[i, 5].Value= "-";
                salarySheet.Cells[i, 6].Value =  Math.Round(totalPayTax*tax.Payroll.ExchangeRate, 0)+" "+tax.TaxCurrency.Symbole;
                for (int j = 1; j <= 6; j++)
                {
                    salarySheet.Cells[i, j].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    salarySheet.Cells[i, j].Style.Font.Bold = true;
                }
                package.SaveAs(new FileInfo(Path.Combine(Path.Combine(temPath, "Summary-" + month + ".xlsx"))));
            }
            var expenseFileFolder = Path.Combine(temPath, "Purchase");
            Directory.CreateDirectory(expenseFileFolder);
            var expenseAttachments = reportService.GetExpenseOfficalDocuments(tax.Expenses.Select(u => u.Id).ToList());
            foreach(var expenseAttachment in expenseAttachments)
            {
                var fileName = expenseAttachment.RefNo;
                if(expenseAttachments.Any(u=>u!=expenseAttachment && u.RefNo == expenseAttachment.RefNo))
                {
                    fileName += Guid.NewGuid().ToString();
                }
                fileName += "." + expenseAttachment.FileName.Split('.')[expenseAttachment.FileName.Split('.').Length - 1];
                using System.Net.WebClient wc = new System.Net.WebClient();
                wc.DownloadFile(expenseAttachment.FileUrl, Path.Combine(expenseFileFolder, fileName));
            }
            var invoiceFolder = Path.Combine(temPath, "Sale");
            Directory.CreateDirectory(invoiceFolder);
            expenseAttachments = reportService.GetInvoiceOfficalDocuments(tax.Invoices.Select(u => u.Id).ToList());

            foreach (var expenseAttachment in expenseAttachments)
            {
                var fileName = expenseAttachment.RefNo;
                if (expenseAttachments.Any(u => u != expenseAttachment && u.RefNo == expenseAttachment.RefNo))
                {
                    fileName += Guid.NewGuid().ToString();
                }
                fileName += "." + expenseAttachment.FileName.Split('.')[expenseAttachment.FileName.Split('.').Length - 1];
                using System.Net.WebClient wc = new System.Net.WebClient();
                wc.DownloadFile(expenseAttachment.FileUrl, Path.Combine(invoiceFolder, fileName));
            }
            var zipFileName = month+"-" + folderName + ".zip";
            string zipPath = Path.Combine(webHostEnvironment.WebRootPath, "Temp", zipFileName);
            ZipFile.CreateFromDirectory(temPath, zipPath);
            Directory.Delete(temPath, true);
            return Ok(new { result = url + "/Temp/"+ zipFileName });
        }
    }
}
