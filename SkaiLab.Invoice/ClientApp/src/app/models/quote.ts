import { Currency } from "./currency";
import { Customer } from "./customer";
import { Product } from "./product";
import { Tax } from "./tax";
import {Location} from "./location";
import { QuoteEnum } from "./enum";
import { Attachment } from "./attachment";
export class QuoteItem{
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
    }
}
export class QuoteStatus{
    id:number;
    name:string;
    count:number;
    constructor(){
        this.id=0;
        this.name="";
        this.count=0;
    }
}
export class Quote{
    id:number;
    customerId:number;
    organisationId:string;
    created:Date;
    expireDate:Date;
    refNo:string;
    number:string;
    note:string;
    currencyId:number;
    taxCurrencyExchangeRate:number;
    baseCurrencyExchangeRate:number;
    total:number;
    totalIncludeTax:number;
    statusId:number;
    createdBy:string;
    acceptedBy:string;
    acceptedDate:Date;
    declinedBy:string;
    declinedDate:string;
    invoicedBy:string;
    invoicedDate:Date;
    currency:Currency;
    customer:Customer;
    status:QuoteStatus;
    date:Date;
    selected:boolean;
    quoteItems:QuoteItem[];
    attachments:Attachment[];
    termAndCondition:string;
    constructor(){
        this.id=0;
        this.customerId=null;
        this.customer=null;
        this.organisationId="";
        this.created=new Date();
        this.expireDate=null;
        this.refNo="";
        this.number="";
        this.note="";
        this.currencyId=null;
        this.taxCurrencyExchangeRate=0;
        this.baseCurrencyExchangeRate=0;
        this.total=0;
        this.totalIncludeTax=0;
        this.statusId=QuoteEnum.Draft;
        this.createdBy="";
        this.acceptedBy="";
        this.acceptedDate=null;
        this.declinedBy="";
        this.declinedDate=null;
        this.invoicedBy="";
        this.invoicedDate=null;
        this.currency=new Currency();
        this.status=new QuoteStatus();
        this.date=new Date();
        this.quoteItems=[];
        this.attachments=[];
        this.termAndCondition="";
    }
}
export class QuoteForUpdateOrCreate{
    quote:Quote;
    currencies:Currency[];
    locations:Location[];
    taxes:Tax[];
    baseCurrencyId:number;
    taxCurrency:Currency;
    number:string;
    constructor(){
        this.quote=new Quote();
        this.currencies=[];
        this.locations=[];
        this.taxes=[];
        this.taxCurrency=new Currency();
        this.number="";
    }
}