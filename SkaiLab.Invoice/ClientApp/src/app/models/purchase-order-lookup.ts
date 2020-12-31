import { Currency } from "./currency";
import { Product } from "./product";
import { Tax } from "./tax";
import { Vendor } from "./vendor";
import {Location} from "./location";
import { PurchaseOrder } from "./purchaseOrder";

export class PurchaseOrderLookup{
    vendors:Vendor[];
    currencies:Currency[];
    taxes:Tax[];
    products:Product[];
    orderNumber:string;
    baseCurrencyId:number;
    locations:Location[]=[];
    taxCurrency:Currency;
    constructor(){
        this.vendors=[];
        this.currencies=[];
        this.taxes=[];
        this.products=[];
        this.orderNumber="";
        this.baseCurrencyId=0;
        this.taxCurrency=new Currency();
        this.locations=[];
    }
}
export class PurchaseOrderForUpdate{
    purchaseOrder:PurchaseOrder;
    currencies:Currency[];
    locations:Location[];
    taxes:Tax[];
    baseCurrencyId:number;
    taxCurrency:Currency;
    constructor(){
        this.purchaseOrder=new PurchaseOrder();
        this.currencies=[];
        this.locations=[];
        this.taxes=[];
        this.baseCurrencyId=0;
        this.taxCurrency=new Currency();
    }
}