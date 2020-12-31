import { Attachment } from "./attachment";
import { Currency } from "./currency";
import { Customer } from "./customer";
import { Product } from "./product";
import { Tax } from "./tax";

export class CustomerTransaction{
    id:number;
    customerId:number;
    organisationId:string;
    refNo:string;
    note:string;
    date:Date;
    currencyId:number;
    taxCurrencyExchangeRatefNo:number;
    baseCurrencyExchangeRate:number;
    total:number;
    totalIncludeTax:number;
    isTaxIncome:boolean;
    paidDate:Date;
    dueDate:Date;
    number:string;
    customer:Customer;
    attachments:Attachment[];
    currency:Currency;
    selected:boolean;
    termAndCondition:string;
    customerTransactionItems:CustomerTransactionItem[];
    constructor(){
        this.id=0;
        this.customerId=null;
        this.organisationId="";
        this.refNo="";
        this.note="";
        this.date=new Date();
        this.currencyId=0;
        this.taxCurrencyExchangeRatefNo=0;
        this.baseCurrencyExchangeRate=0;
        this.total=0;
        this.totalIncludeTax=0;
        this.isTaxIncome=false;
        this.paidDate=null;
        this.dueDate=new Date();
        this.number="";
        this.customer=new Customer();
        this.attachments=[];
        this.customerTransactionItems=[];
        this.currency=new Currency();
        this.selected=false;
        this.termAndCondition="";
    }

}
export class CustomerTransactionItem{
    id:number;
    productId:number;
    quantity:number;
    taxId?:number;
    lineTotal:number;
    lineTotalIncludeTax:number;
    product:Product;
    tax:Tax;
    discountRate?:number;
    unitPrice:number;
    locationId:number;
    description:string;
    inventoryQty:number;
    requestingInventory:boolean;
    
    constructor(){
        this.id=0;
        this.productId=null;
        this.product=new Product();
        this.quantity=null;
        this.taxId=null;
        this.discountRate=null;
        this.tax=null;
        this.lineTotal=0;
        this.lineTotalIncludeTax=0;
        this.unitPrice=0;
        this.locationId=null;
        this.description="";
        this.inventoryQty=null;
        this.requestingInventory=false;
        
    }
}
export class Invoice extends CustomerTransaction{
    statusId:number;
    status:InvoiceStatus;
    constructor(){
        super();
        this.statusId=0;
        this.status=new InvoiceStatus();
    }
}
export class InvoiceStatus{
    id:number;
    name:string;
    constructor(){
        this.id=0;
        this.name="";
    }
}
export class CustomerCreditStatus{
    id:number;
    name:string;
    constructor(){
        this.id=0;
        this.name="";
    }
}
export class CustomerCredit extends CustomerTransaction{
    statusId:number;
    status:InvoiceStatus;
    constructor(){
        super();
        this.statusId=0;
        this.status=new InvoiceStatus();
    }
}