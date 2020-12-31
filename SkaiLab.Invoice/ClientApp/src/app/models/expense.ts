import { Attachment } from "./attachment";
import { Currency } from "./currency";
import { ExepnseStatusEnum } from "./enum";
import { PurchaseOrderItem } from "./purchaseOrder";
import { Tax } from "./tax";
import { Vendor } from "./vendor";

export class ExpenseStatus{
    id:number;
    name:string;
    count:number;
    constructor(){
        this.id=0;
        this.name="";
        this.count=0;
    }
}
export class Expense{
    id:number;
    vendorId:number;
    organisationId:string;
    vendor:Vendor;
    creaed:Date;
    date:Date;
    deliveryDate:Date;
    orderNumber:string;
    note:string;
    currencyId:number;
    currency:Currency;
    taxCurrencyExchangeRate:number;
    baseCurrencyExchangeRate:number;
    total:number;
    totalIncludeTax:number;
    createdBy:string;
    approvedDate:Date;
    approvedBy:string;
    billedDate:Date;
    billedBy:string;
    customerInvoiceUrl:string;
    expenseStatusId:number;
    expenseStatus:ExpenseStatus;
    refNo:string;
    selected:boolean;
    attachments:Attachment[];
    expenseItems:PurchaseOrderItem[];
    termAndCondition:string;
    closeDate?:Date;
    hasCloseDoc:boolean;
    loading:boolean;
    closeAttachment:Attachment;
    constructor(){
        this.id=0;
        this.vendorId=null;
        this.organisationId="";
        this.vendor=new Vendor();
        this.creaed=new Date();
        this.date=new Date();
        this.deliveryDate=new Date();
        this.orderNumber="";
        this.note="";
        this.currencyId=null;
        this.currency=new Currency();
        this.taxCurrencyExchangeRate=0;
        this.baseCurrencyExchangeRate=0;
        this.total=0;
        this.totalIncludeTax=0;
        this.createdBy="";
        this.approvedDate=null;
        this.approvedBy="";
        this.billedDate=null;
        this.billedBy="";
        this.customerInvoiceUrl="";
        this.expenseStatusId=ExepnseStatusEnum.Draft;
        this.expenseStatus=new ExpenseStatus();
        this.selected=false;
        this.attachments=[];
        this.expenseItems=[];
        this.termAndCondition="";
        this.closeDate=null;
        this.hasCloseDoc=false;
        this.closeAttachment=new Attachment();
    }
}
export class ExpenseForUpdate{
    expense:Expense;
    currencies:Currency[];
    taxes:Tax[];
    baseCurrencyId:number;
    taxCurrency:Currency;
    constructor(){
        this.expense=new Expense();
        this.currencies=[];
        this.taxes=[];
        this.baseCurrencyId=0;
        this.taxCurrency=new Currency();
    }
}