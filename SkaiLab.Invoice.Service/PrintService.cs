using SkaiLab.Invoice.Models;
using System;
using System.Linq;

namespace SkaiLab.Invoice.Service
{
    public class PrintService:Service,IPrintService
    {
        public PrintService(IDataContext context):base(context)
        {

        }

        public Expense GetBill(long id)
        {
            using var context = Context();
            var order = context.Bill.FirstOrDefault(u => u.Id == id).IdNavigation;
            return new Expense
            {
                BaseCurrencyExchangeRate = order.BaseCurrencyExchangeRate.Value,
                Id = order.Id,
                Currency = new Currency
                {
                    Id = order.CurrencyId,
                    Code = order.Currency.Code,
                    Name = order.Currency.Name,
                    Symbole = order.Currency.Symbole
                },
                CurrencyId = order.CurrencyId,
                Date = order.Date,
                DeliveryDate = order.DeliveryDate,
                Note = order.Note,
                OrderNumber = order.Number,
                RefNo = order.RefNo,
                TaxCurrencyExchangeRate = order.TaxCurrencyExchangeRate.Value,
                VendorId = order.VendorId,
                TermAndCondition = order.TermAndCondition,
                Vendor = new Vendor
                {
                    BusinessRegistrationNumber = order.Vendor.BusinessRegistrationNumber,
                    Contact = new Contact
                    {
                        Address = order.Vendor.Contact.Address,
                        ContactName = order.Vendor.Contact.ContactName,
                        Email = order.Vendor.Contact.Email,
                        PhoneNumber = order.Vendor.Contact.PhoneNumber,
                        Website = order.Vendor.Contact.Website,
                        Id = order.Vendor.ContactId
                    },
                    DisplayName = order.Vendor.DisplayName,
                    Id = order.VendorId,
                    LegalName = order.Vendor.LegalName,
                    LocalLegalName = order.Vendor.LocalLegalName,
                    TaxNumber = order.Vendor.TaxNumber,

                },
                Total = order.Total,
                TotalIncludeTax = order.TotalIncludeTax,
                Organisation = new Organsation
                {
                    TaxNumber = order.Organisation.TaxNumber,
                    BussinessRegistrationNumber = order.Organisation.BussinessRegistrationNumber,
                    Contact = new Contact
                    {
                        Address = order.Organisation.Contact.Address,
                        ContactName = order.Organisation.Contact.ContactName,
                        Email = order.Organisation.Contact.Email,
                        Id = order.Organisation.Contact.Id,
                        PhoneNumber = order.Organisation.Contact.PhoneNumber,
                        Website = order.Organisation.Contact.Website
                    },
                    Id = order.OrganisationId,
                    Description = order.Organisation.Description,
                    DisplayName = order.Organisation.DisplayName,
                    LegalLocalName = order.Organisation.LegalLocalName,
                    LegalName = order.Organisation.LegalName,
                    LogoUrl = order.Organisation.LogoUrl,
                    LineBusiness = order.Organisation.LineBusiness,
                    TaxCurrency = new Currency
                    {
                        Code = order.Organisation.OrganisationBaseCurrency.TaxCurrency.Code,
                        Id = order.Organisation.OrganisationBaseCurrency.TaxCurrency.Id,
                        Name = order.Organisation.OrganisationBaseCurrency.TaxCurrency.Name,
                        Symbole = order.Organisation.OrganisationBaseCurrency.TaxCurrency.Symbole
                    },

                },
                ExpenseItems = order.ExpenseItem.Select(u => new PurchaseOrderItem
                {
                    Description = u.Description,
                    DiscountRate = u.DiscountRate,
                    Id = u.Id,
                    LineTotal = u.LineTotal,
                    LineTotalIncludeTax = u.LineTotalIncludeTax,
                    Quantity = u.Quantity,
                    UnitPrice = u.UnitPrice,
                    Tax = u.Tax == null ? null : new Tax
                    {
                        Id = u.Tax.Id,
                        Name = u.Tax.Name,
                        Components = u.Tax.TaxComponent.Select(t => new TaxComponent
                        {
                            Id = t.Id,
                            Name = t.Name,
                            Rate = t.Rate
                        }).ToList(),
                        TotalRate = u.Tax.TaxComponent.Sum(u => u.Rate)
                    }
                }).ToList()

            };
        }
        public Expense GetExpense(long id)
        {
            using var context = Context();
            var order = context.VendorExpense.FirstOrDefault(u => u.Id == id).IdNavigation;
            return new Expense
            {
                BaseCurrencyExchangeRate = order.BaseCurrencyExchangeRate.Value,
                Id = order.Id,
                Currency = new Currency
                {
                    Id = order.CurrencyId,
                    Code = order.Currency.Code,
                    Name = order.Currency.Name,
                    Symbole = order.Currency.Symbole
                },
                CurrencyId = order.CurrencyId,
                Date = order.Date,
                DeliveryDate = order.DeliveryDate,
                Note = order.Note,
                OrderNumber = order.Number,
                RefNo = order.RefNo,
                TaxCurrencyExchangeRate = order.TaxCurrencyExchangeRate.Value,
                VendorId = order.VendorId,
                TermAndCondition = order.TermAndCondition,
                Vendor = new Vendor
                {
                    BusinessRegistrationNumber = order.Vendor.BusinessRegistrationNumber,
                    Contact = new Contact
                    {
                        Address = order.Vendor.Contact.Address,
                        ContactName = order.Vendor.Contact.ContactName,
                        Email = order.Vendor.Contact.Email,
                        PhoneNumber = order.Vendor.Contact.PhoneNumber,
                        Website = order.Vendor.Contact.Website,
                        Id = order.Vendor.ContactId
                    },
                    DisplayName = order.Vendor.DisplayName,
                    Id = order.VendorId,
                    LegalName = order.Vendor.LegalName,
                    LocalLegalName = order.Vendor.LocalLegalName,
                    TaxNumber = order.Vendor.TaxNumber,

                },
                Total = order.Total,
                TotalIncludeTax = order.TotalIncludeTax,
                Organisation = new Organsation
                {
                    TaxNumber = order.Organisation.TaxNumber,
                    BussinessRegistrationNumber = order.Organisation.BussinessRegistrationNumber,
                    Contact = new Contact
                    {
                        Address = order.Organisation.Contact.Address,
                        ContactName = order.Organisation.Contact.ContactName,
                        Email = order.Organisation.Contact.Email,
                        Id = order.Organisation.Contact.Id,
                        PhoneNumber = order.Organisation.Contact.PhoneNumber,
                        Website = order.Organisation.Contact.Website
                    },
                    Id = order.OrganisationId,
                    Description = order.Organisation.Description,
                    DisplayName = order.Organisation.DisplayName,
                    LegalLocalName = order.Organisation.LegalLocalName,
                    LegalName = order.Organisation.LegalName,
                    LogoUrl = order.Organisation.LogoUrl,
                    LineBusiness = order.Organisation.LineBusiness,
                    TaxCurrency = new Currency
                    {
                        Code = order.Organisation.OrganisationBaseCurrency.TaxCurrency.Code,
                        Id = order.Organisation.OrganisationBaseCurrency.TaxCurrency.Id,
                        Name = order.Organisation.OrganisationBaseCurrency.TaxCurrency.Name,
                        Symbole = order.Organisation.OrganisationBaseCurrency.TaxCurrency.Symbole
                    },

                },
                ExpenseItems = order.ExpenseItem.Select(u => new PurchaseOrderItem
                {
                    Description = u.Description,
                    DiscountRate = u.DiscountRate,
                    Id = u.Id,
                    LineTotal = u.LineTotal,
                    LineTotalIncludeTax = u.LineTotalIncludeTax,
                    Quantity = u.Quantity,
                    UnitPrice = u.UnitPrice,
                    Tax = u.Tax == null ? null : new Tax
                    {
                        Id = u.Tax.Id,
                        Name = u.Tax.Name,
                        Components = u.Tax.TaxComponent.Select(t => new TaxComponent
                        {
                            Id = t.Id,
                            Name = t.Name,
                            Rate = t.Rate
                        }).ToList(),
                        TotalRate = u.Tax.TaxComponent.Sum(u => u.Rate)
                    }
                }).ToList()

            };
        }

        public Models.Invoice GetInvoice(long id)
        {
            using var context = Context();
            var invoice = context.Invoice.FirstOrDefault(u => u.Id == id).IdNavigation;
            return new Models.Invoice
            {
                Id = invoice.Id,
                BaseCurrencyExchangeRate = invoice.BaseCurrencyExchangeRate,
                Date = invoice.Date,
                CurrencyId = invoice.CurrencyId,
                Currency = new Currency
                {
                    Id = invoice.CurrencyId,
                    Code = invoice.Currency.Code,
                    Name = invoice.Currency.Name,
                    Symbole = invoice.Currency.Symbole
                },
                Customer = new Customer
                {
                    BusinessRegistrationNumber = invoice.Customer.BusinessRegistrationNumber,
                    DisplayName = invoice.Customer.DisplayName,
                    Id = invoice.CustomerId,
                    LegalName = invoice.Customer.LegalName,
                    TaxNumber = invoice.Customer.TaxNumber,
                    Contact = new Contact
                    {
                        Address = invoice.Customer.Contact.Address,
                        Email = invoice.Customer.Contact.Email,
                        Id = invoice.Customer.ContactId,
                        ContactName = invoice.Customer.Contact.ContactName,
                        PhoneNumber = invoice.Customer.Contact.PhoneNumber,
                        Website = invoice.Customer.Contact.Website,

                    },
                    LocalLegalName = invoice.Customer.LocalLegalName
                },
                TermAndCondition = invoice.TermAndCondition,
                CustomerId = invoice.CustomerId,
                Note = invoice.Note,
                Number = invoice.Number,
                OrganisationId = invoice.OrganisationId,
                Organisation = new Organsation
                {
                    BussinessRegistrationNumber = invoice.Organisation.BussinessRegistrationNumber,
                    Description = invoice.Organisation.Description,
                    DisplayName = invoice.Organisation.DisplayName,
                    Id = invoice.OrganisationId,
                    LegalName = invoice.Organisation.LegalName,
                    LineBusiness = invoice.Organisation.LineBusiness,
                    LegalLocalName = invoice.Organisation.LegalLocalName,
                    LogoUrl = invoice.Organisation.LogoUrl,
                    TaxNumber = invoice.Organisation.TaxNumber,
                    Contact = new Contact
                    {
                        Address = invoice.Organisation.Contact.Address,
                        ContactName = invoice.Organisation.Contact.ContactName,
                        Email = invoice.Organisation.Contact.Email,
                        Id = invoice.Organisation.ContactId,
                        PhoneNumber = invoice.Organisation.Contact.PhoneNumber,
                        Website = invoice.Organisation.Contact.Website,

                    },
                    TaxCurrency = new Currency
                    {
                        Id = invoice.Organisation.OrganisationBaseCurrency.TaxCurrencyId,
                        Code = invoice.Organisation.OrganisationBaseCurrency.TaxCurrency.Code,
                        Name = invoice.Organisation.OrganisationBaseCurrency.TaxCurrency.Name,
                        Symbole = invoice.Organisation.OrganisationBaseCurrency.TaxCurrency.Symbole
                    }
                },
                RefNo = invoice.RefNo,
                TaxCurrencyExchangeRate = invoice.TaxCurrencyExchangeRate,
                TotalIncludeTax = invoice.TotalIncludeTax,
                Total = invoice.Total,
                CustomerTransactionItems = invoice.CustomerTransactionItem.Select(u => new CustomerTransactionItem
                {
                    Description = u.Description,
                    DiscountRate = u.DiscountRate,
                    Id = u.Id,
                    LineTotal = u.LineTotal,
                    LineTotalIncludeTax = u.LineTotalIncludeTax,
                    ProductId = u.CustomerTransactionItemProduct.ProductId,
                    Quantity = u.Quantity,
                    UnitPrice = u.UnitPrice,
                    Product = new Product
                    {
                        Id = u.CustomerTransactionItemProduct.ProductId,
                        Code = u.CustomerTransactionItemProduct.Product.Code,
                        ImageUrl = u.CustomerTransactionItemProduct.Product.ImageUrl,
                        Name = u.CustomerTransactionItemProduct.Product.Name,
                        ProductSaleInformation = new ProductSalePurchaseDetail
                        {
                            Title = u.CustomerTransactionItemProduct.Product.ProductSaleInformation.Title,
                            Description = u.CustomerTransactionItemProduct.Product.ProductSaleInformation.Description
                        }
                    },
                    TaxId = u.TaxId,
                    Tax = u.Tax == null ? null : new Tax
                    {
                        Id = u.Tax.Id,
                        Name = u.Tax.Name,
                        Components = u.Tax.TaxComponent.Select(t => new TaxComponent
                        {
                            Id = t.Id,
                            Name = t.Name,
                            Rate = t.Rate
                        }).ToList(),
                        TotalRate = u.Tax.TaxComponent.Sum(u => u.Rate)
                    },


                }).ToList()

            };
        }

        public PayrollNoneTax GetPayrollNoneTax(long id)
        {
            using var context = Context();
            var payroll = context.PayrollEmployee.FirstOrDefault(u => u.Id == id);
            return new PayrollNoneTax
            {
                Date = payroll.Date,
                Salary = payroll.PayrollEmployeeNoneTax.Salary,
                EmployeeId = payroll.EmployeeId,
                Employee = new Employee
                {
                    DisplayName = payroll.Employee.DisplayName,
                    JobTitle = payroll.Employee.JobTitle,
                    Id = payroll.EmployeeId
                },
                Id = payroll.Id,
                Total = payroll.Total,
                OtherBenefit = payroll.PayrollEmployeeNoneTax.OtherBenefit,
                TransactionDate = payroll.TransactionDate,
                Organsation = new Organsation
                {
                    TaxNumber = payroll.Employee.Organisation.TaxNumber,
                    BussinessRegistrationNumber = payroll.Employee.Organisation.BussinessRegistrationNumber,
                    Contact = new Contact
                    {
                        Address = payroll.Employee.Organisation.Contact.Address,
                        ContactName = payroll.Employee.Organisation.Contact.ContactName,
                        Email = payroll.Employee.Organisation.Contact.Email,
                        Id = payroll.Employee.Organisation.Contact.Id,
                        PhoneNumber = payroll.Employee.Organisation.Contact.PhoneNumber,
                        Website = payroll.Employee.Organisation.Contact.Website
                    },
                    Id = payroll.Employee.OrganisationId,
                    Description = payroll.Employee.Organisation.Description,
                    DisplayName = payroll.Employee.Organisation.DisplayName,
                    LegalLocalName = payroll.Employee.Organisation.LegalLocalName,
                    LegalName = payroll.Employee.Organisation.LegalName,
                    LogoUrl = payroll.Employee.Organisation.LogoUrl,
                    LineBusiness = payroll.Employee.Organisation.LineBusiness,
                    TaxCurrency = new Currency
                    {
                        Code = payroll.Employee.Organisation.OrganisationBaseCurrency.TaxCurrency.Code,
                        Id = payroll.Employee.Organisation.OrganisationBaseCurrency.TaxCurrency.Id,
                        Name = payroll.Employee.Organisation.OrganisationBaseCurrency.TaxCurrency.Name,
                        Symbole = payroll.Employee.Organisation.OrganisationBaseCurrency.TaxCurrency.Symbole
                    },
                    BaseCurrency = new Currency
                    {
                        Code = payroll.Employee.Organisation.OrganisationBaseCurrency.BaseCurrency.Code,
                        Id = payroll.Employee.Organisation.OrganisationBaseCurrency.BaseCurrency.Id,
                        Name = payroll.Employee.Organisation.OrganisationBaseCurrency.BaseCurrency.Name,
                        Symbole = payroll.Employee.Organisation.OrganisationBaseCurrency.BaseCurrency.Symbole
                    },

                }

            };
        }

        public PayrollTax GetPayrollTax(long id)
        {
            using var context = Context();
            var payroll = context.PayrollEmployee.FirstOrDefault(u => u.Id == id);
            return new PayrollTax
            {
                Date = payroll.Date,
                DeductSalary = payroll.PayrollEmployeeTax.SalaryTax,
                Salary = payroll.PayrollEmployeeTax.Salary,
                EmployeeId = payroll.EmployeeId,
                Employee = new Employee
                {
                    DisplayName = payroll.Employee.DisplayName,
                    JobTitle = payroll.Employee.JobTitle,
                    IsResidentEmployee=payroll.PayrollEmployeeTax.IsResidentEmployee,
                    IsConfederationThatHosts=payroll.PayrollEmployeeTax.ConfederationThatHosts,
                    NumberOfChild=payroll.PayrollEmployeeTax.NumberOfChilds,
                    Id=payroll.EmployeeId
                },
                Id = payroll.Id,
                NumberOfChilds = payroll.PayrollEmployeeTax.NumberOfChilds,
                Total = payroll.Total,
                OtherBenefit = payroll.PayrollEmployeeTax.OtherBenefit,
                TransactionDate = payroll.TransactionDate,
                OtherBenefitTaxDeduct = payroll.PayrollEmployeeTax.OtherBenefitTaxDeduct,
                Organsation=new Organsation
                {
                    TaxNumber = payroll.Employee.Organisation.TaxNumber,
                    BussinessRegistrationNumber = payroll.Employee.Organisation.BussinessRegistrationNumber,
                    Contact = new Contact
                    {
                        Address = payroll.Employee.Organisation.Contact.Address,
                        ContactName = payroll.Employee.Organisation.Contact.ContactName,
                        Email = payroll.Employee.Organisation.Contact.Email,
                        Id = payroll.Employee.Organisation.Contact.Id,
                        PhoneNumber = payroll.Employee.Organisation.Contact.PhoneNumber,
                        Website = payroll.Employee.Organisation.Contact.Website
                    },
                    Id = payroll.Employee.OrganisationId,
                    Description = payroll.Employee.Organisation.Description,
                    DisplayName = payroll.Employee.Organisation.DisplayName,
                    LegalLocalName = payroll.Employee.Organisation.LegalLocalName,
                    LegalName = payroll.Employee.Organisation.LegalName,
                    LogoUrl = payroll.Employee.Organisation.LogoUrl,
                    LineBusiness = payroll.Employee.Organisation.LineBusiness,
                    TaxCurrency = new Currency
                    {
                        Code = payroll.Employee.Organisation.OrganisationBaseCurrency.TaxCurrency.Code,
                        Id = payroll.Employee.Organisation.OrganisationBaseCurrency.TaxCurrency.Id,
                        Name = payroll.Employee.Organisation.OrganisationBaseCurrency.TaxCurrency.Name,
                        Symbole = payroll.Employee.Organisation.OrganisationBaseCurrency.TaxCurrency.Symbole
                    },
                    BaseCurrency = new Currency
                    {
                        Code = payroll.Employee.Organisation.OrganisationBaseCurrency.BaseCurrency.Code,
                        Id = payroll.Employee.Organisation.OrganisationBaseCurrency.BaseCurrency.Id,
                        Name = payroll.Employee.Organisation.OrganisationBaseCurrency.BaseCurrency.Name,
                        Symbole = payroll.Employee.Organisation.OrganisationBaseCurrency.BaseCurrency.Symbole
                    },

                }
                
            };

        }

        public PurchaseOrder GetPurchase(long id)
        {
            using var context = Context();
            var order = context.PurchaseOrder.FirstOrDefault(u => u.Id == id).IdNavigation;
            return new PurchaseOrder
            {
                BaseCurrencyExchangeRate=order.BaseCurrencyExchangeRate.Value,
                Id=order.Id,
                Currency=new Currency
                {
                    Id=order.CurrencyId,
                    Code=order.Currency.Code,
                    Name=order.Currency.Name,
                    Symbole=order.Currency.Symbole
                },
                CurrencyId=order.CurrencyId,
                Date=order.Date,
                DeliveryDate=order.DeliveryDate,
                Note=order.Note,
                OrderNumber=order.Number,
                RefNo=order.RefNo,
                TaxCurrencyExchangeRate=order.TaxCurrencyExchangeRate.Value,
                VendorId=order.VendorId,
                TermAndCondition=order.TermAndCondition,
                Vendor=new Vendor
                {
                    BusinessRegistrationNumber=order.Vendor.BusinessRegistrationNumber,
                    Contact=new Contact
                    {
                        Address=order.Vendor.Contact.Address,
                        ContactName=order.Vendor.Contact.ContactName,
                        Email=order.Vendor.Contact.Email,
                        PhoneNumber=order.Vendor.Contact.PhoneNumber,
                        Website=order.Vendor.Contact.Website,
                        Id=order.Vendor.ContactId
                    },
                    DisplayName=order.Vendor.DisplayName,
                    Id=order.VendorId,
                    LegalName=order.Vendor.LegalName,
                    LocalLegalName=order.Vendor.LocalLegalName,
                    TaxNumber=order.Vendor.TaxNumber,
                    
                },
                Total=order.Total,
                TotalIncludeTax=order.TotalIncludeTax,
                Organisation=new Organsation
                {
                    TaxNumber=order.Organisation.TaxNumber,
                    BussinessRegistrationNumber=order.Organisation.BussinessRegistrationNumber,
                    Contact=new Contact
                    {
                        Address=order.Organisation.Contact.Address,
                        ContactName=order.Organisation.Contact.ContactName,
                        Email=order.Organisation.Contact.Email,
                        Id=order.Organisation.Contact.Id,
                        PhoneNumber=order.Organisation.Contact.PhoneNumber,
                        Website=order.Organisation.Contact.Website
                    },
                    Id=order.OrganisationId,
                    Description=order.Organisation.Description,
                    DisplayName=order.Organisation.DisplayName,
                    LegalLocalName=order.Organisation.LegalLocalName,
                    LegalName=order.Organisation.LegalName,
                    LogoUrl=order.Organisation.LogoUrl,
                    LineBusiness=order.Organisation.LineBusiness,
                    TaxCurrency=new Currency
                    {
                        Code=order.Organisation.OrganisationBaseCurrency.TaxCurrency.Code,
                        Id=order.Organisation.OrganisationBaseCurrency.TaxCurrency.Id,
                        Name=order.Organisation.OrganisationBaseCurrency.TaxCurrency.Name,
                        Symbole=order.Organisation.OrganisationBaseCurrency.TaxCurrency.Symbole
                    },
                    
                },
                ExpenseItems=order.ExpenseItem.Select(u=>new PurchaseOrderItem
                {
                    Description=u.Description,
                    DiscountRate=u.DiscountRate,
                    Id=u.Id,
                    LineTotal=u.LineTotal,
                    LineTotalIncludeTax=u.LineTotalIncludeTax,
                    Quantity=u.Quantity,
                    UnitPrice=u.UnitPrice,
                    ProductId=u.ExpenseProductItem.ProductId,
                    Product=new Product
                    {
                        Id = u.ExpenseProductItem.ProductId,
                        Code = u.ExpenseProductItem.Product.Code,
                        ImageUrl = u.ExpenseProductItem.Product.ImageUrl,
                        Name = u.ExpenseProductItem.Product.ProductPurchaseInformation.Title,
                        ProductPurchaseInformation = new ProductSalePurchaseDetail
                        {
                            Title = u.ExpenseProductItem.Product.ProductPurchaseInformation.Title,
                            Description = u.ExpenseProductItem.Product.ProductPurchaseInformation.Description
                        },
                    },
                    Tax=u.Tax==null?null:new Tax
                    {
                        Id=u.Tax.Id,
                        Name=u.Tax.Name,
                        Components=u.Tax.TaxComponent.Select(t=>new TaxComponent
                        {
                            Id=t.Id,
                            Name=t.Name,
                            Rate=t.Rate
                        }).ToList(),
                        TotalRate=u.Tax.TaxComponent.Sum(u=>u.Rate)
                    }
                }).ToList()

            };
        }

        public Quote GetQuote(long id)
        {
            using var context = Context();
            var quote = context.Quote.FirstOrDefault(u => u.Id == id);
            return new Quote
            {
                Id = quote.Id,
                BaseCurrencyExchangeRate = quote.BaseCurrencyExchangeRate,
                Date = quote.Date,
                CurrencyId = quote.CurrencyId,
                Currency = new Currency
                {
                    Id = quote.CurrencyId,
                    Code = quote.Currency.Code,
                    Name = quote.Currency.Name,
                    Symbole = quote.Currency.Symbole
                },
                Customer = new Customer
                {
                    BusinessRegistrationNumber = quote.Customer.BusinessRegistrationNumber,
                    DisplayName = quote.Customer.DisplayName,
                    Id = quote.CustomerId,
                    LegalName = quote.Customer.LegalName,
                    TaxNumber = quote.Customer.TaxNumber,
                    Contact = new Contact
                    {
                        Address = quote.Customer.Contact.Address,
                        Email = quote.Customer.Contact.Email,
                        Id = quote.Customer.ContactId,
                        ContactName = quote.Customer.Contact.ContactName,
                        PhoneNumber = quote.Customer.Contact.PhoneNumber,
                        Website = quote.Customer.Contact.Website,

                    },
                    LocalLegalName = quote.Customer.LocalLegalName
                },
                TermAndCondition = quote.TermAndCondition,
                CustomerId = quote.CustomerId,
                Note = quote.Note,
                Number = quote.Number,
                OrganisationId = quote.OrganisationId,
                Organisation = new Organsation
                {
                    BussinessRegistrationNumber = quote.Organisation.BussinessRegistrationNumber,
                    Description = quote.Organisation.Description,
                    DisplayName = quote.Organisation.DisplayName,
                    Id = quote.OrganisationId,
                    LegalName = quote.Organisation.LegalName,
                    LineBusiness = quote.Organisation.LineBusiness,
                    LegalLocalName = quote.Organisation.LegalLocalName,
                    LogoUrl = quote.Organisation.LogoUrl,
                    TaxNumber = quote.Organisation.TaxNumber,
                    Contact = new Contact
                    {
                        Address = quote.Organisation.Contact.Address,
                        ContactName = quote.Organisation.Contact.ContactName,
                        Email = quote.Organisation.Contact.Email,
                        Id = quote.Organisation.ContactId,
                        PhoneNumber = quote.Organisation.Contact.PhoneNumber,
                        Website = quote.Organisation.Contact.Website,

                    },
                    TaxCurrency = new Currency
                    {
                        Id = quote.Organisation.OrganisationBaseCurrency.TaxCurrencyId,
                        Code = quote.Organisation.OrganisationBaseCurrency.TaxCurrency.Code,
                        Name = quote.Organisation.OrganisationBaseCurrency.TaxCurrency.Name,
                        Symbole = quote.Organisation.OrganisationBaseCurrency.TaxCurrency.Symbole
                    }
                },
                RefNo = quote.RefNo,
                TaxCurrencyExchangeRate = quote.TaxCurrencyExchangeRate,
                TotalIncludeTax = quote.TotalIncludeTax,
                Total = quote.Total,
                QuoteItems = quote.QuoteItem.Select(u => new QuoteItem
                {
                    Description = u.Description,
                    DiscountRate = u.DiscountRate,
                    Id = u.Id,
                    LineTotal = u.LineTotal,
                    LineTotalIncludeTax = u.LineTotalIncludeTax,
                    ProductId = u.ProductId,
                    Quantity = u.Quantity,
                    UnitPrice = u.UnitPrice,
                    Product = new Product
                    {
                        Id = u.ProductId,
                        Code = u.Product.Code,
                        ImageUrl = u.Product.ImageUrl,
                        Name = u.Product.ProductSaleInformation.Title,
                        ProductSaleInformation = new ProductSalePurchaseDetail
                        {
                            Title = u.Product.ProductSaleInformation.Title,
                            Description = u.Product.ProductSaleInformation.Description
                        }
                    },
                    TaxId = u.TaxId,
                    Tax = u.Tax == null ? null : new Tax
                    {
                        Id = u.Tax.Id,
                        Name = u.Tax.Name,
                        Components = u.Tax.TaxComponent.Select(t => new TaxComponent
                        {
                            Id = t.Id,
                            Name = t.Name,
                            Rate = t.Rate
                        }).ToList(),
                        TotalRate = u.Tax.TaxComponent.Sum(u => u.Rate)
                    },


                }).ToList()

            };
        }

        public Receipt GetReceipt(long id, string purpose)
        {
            using var context = Context();
            var invoice = context.Invoice.FirstOrDefault(u => u.Id == id).IdNavigation;
            return new Receipt
            {
                Customer = new Customer
                {
                    BusinessRegistrationNumber = invoice.Customer.BusinessRegistrationNumber,
                    DisplayName = invoice.Customer.DisplayName,
                    Id = invoice.CustomerId,
                    LegalName = invoice.Customer.LegalName,
                    TaxNumber = invoice.Customer.TaxNumber,
                    Contact = new Contact
                    {
                        Address = invoice.Customer.Contact.Address,
                        Email = invoice.Customer.Contact.Email,
                        Id = invoice.Customer.ContactId,
                        ContactName = invoice.Customer.Contact.ContactName,
                        PhoneNumber = invoice.Customer.Contact.PhoneNumber,
                        Website = invoice.Customer.Contact.Website,

                    },
                    LocalLegalName = invoice.Customer.LocalLegalName
                },
                Orgainsation = new Organsation
                {
                    BussinessRegistrationNumber = invoice.Organisation.BussinessRegistrationNumber,
                    Description = invoice.Organisation.Description,
                    DisplayName = invoice.Organisation.DisplayName,
                    Id = invoice.OrganisationId,
                    LegalName = invoice.Organisation.LegalName,
                    LineBusiness = invoice.Organisation.LineBusiness,
                    LegalLocalName = invoice.Organisation.LegalLocalName,
                    LogoUrl = invoice.Organisation.LogoUrl,
                    TaxNumber = invoice.Organisation.TaxNumber,
                    Contact = new Contact
                    {
                        Address = invoice.Organisation.Contact.Address,
                        ContactName = invoice.Organisation.Contact.ContactName,
                        Email = invoice.Organisation.Contact.Email,
                        Id = invoice.Organisation.ContactId,
                        PhoneNumber = invoice.Organisation.Contact.PhoneNumber,
                        Website = invoice.Organisation.Contact.Website,

                    },
                    TaxCurrency = new Currency
                    {
                        Id = invoice.Organisation.OrganisationBaseCurrency.TaxCurrencyId,
                        Code = invoice.Organisation.OrganisationBaseCurrency.TaxCurrency.Code,
                        Name = invoice.Organisation.OrganisationBaseCurrency.TaxCurrency.Name,
                        Symbole = invoice.Organisation.OrganisationBaseCurrency.TaxCurrency.Symbole
                    }
                },
                Total=invoice.TotalIncludeTax,
                CurrencyCode=invoice.Currency.Code,
                Purpose=purpose,
                Date=CurrentCambodiaTime,
                AmountInWord= WordCurrencyService.ConvertNumberToWord(invoice.TotalIncludeTax.ToString(),invoice.Currency.Code),
                IsTaxInvoice=invoice.TotalIncludeTax>invoice.Total
            };
        }

        public bool IsTaxPayslip(long payrollId)
        {
            using var context = Context();
            return context.PayrollEmployeeTax.FirstOrDefault(u => u.Id == payrollId) != null;
        }
    }
}
