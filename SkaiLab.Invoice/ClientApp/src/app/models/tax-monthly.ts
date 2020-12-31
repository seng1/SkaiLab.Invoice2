import { Currency } from "./currency";
import { Invoice } from "./customer-transaction";
import { Expense } from "./expense";
import { PayrollMonthTax } from "./payroll";

export class TaxMonthly{
    totalInvoice:number;
    totalExpense:number;
    totalEmployeeSalary:number;
    totalPayToTax:number;
    totalPayToTaxInKHR:number;
    isPayrollRun:boolean;
    invoices:Invoice[];
    expenses:Expense[];
    payroll:PayrollMonthTax;
    currency:Currency;
    taxCurrency:Currency;
    totalInvoiceTaxInBaseCurrency:number;
    totalInvoiceTaxInTaxCurrency:number;
    totalExpenseTaxInBaseCurrency:number;
    totalExpenseTaxInTaxCurrency:number;
    totalEmployeeTaxInBaseCurrency:number;
    totalEmployeeTaxInTaxCurrency:number;
    showInvoicePanel:boolean;
    showExpensePanel:boolean;
    showEmployeePanel:boolean;
    constructor(){
        this.totalInvoice=0;
        this.totalExpense=0;
        this.totalEmployeeSalary=0;
        this.totalPayToTax=0;
        this.totalPayToTaxInKHR=0;
        this.isPayrollRun=true;
        this.invoices=[];
        this.expenses=[];
        this.payroll=new PayrollMonthTax();
        this.currency=new Currency();
        this.taxCurrency=new Currency();
        this.totalInvoiceTaxInBaseCurrency=0;
        this.totalInvoiceTaxInTaxCurrency=0;
        this.totalExpenseTaxInBaseCurrency=0;
        this.totalExpenseTaxInTaxCurrency=0;
        this.totalEmployeeTaxInBaseCurrency=0;
        this.totalEmployeeTaxInTaxCurrency=0;
    }

}