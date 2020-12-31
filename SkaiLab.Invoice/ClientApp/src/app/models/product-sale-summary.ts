import { Product } from "./product";

export class ProductSaleSummary{
    product:Product;
    avgPurchasePrice:number;
    purchaseQty:number;
    purchaseTotal:number;
    avgSalePrice:number;
    saleQty:number;
    saleTotal:number;
    netQty:number;
    netTotal:number;
    constructor(){
        this.product=new Product();
        this.avgPurchasePrice=0;
        this.purchaseQty=0;
        this.purchaseTotal=0;
        this.avgSalePrice=0;
        this.saleQty=0;
        this.saleTotal=0;
        this.netQty=0;
        this.netTotal=0;
    }
}