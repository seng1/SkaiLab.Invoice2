import { Expense } from "./expense";
import { Product } from "./product";
import { Tax } from "./tax";

export class PurchaseOrder extends Expense{
    
    constructor(){
        super();
       
    }
}

export class PurchaseOrderItem{
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