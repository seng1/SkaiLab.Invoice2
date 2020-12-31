export class ProductSaleDetailItem{
    date:Date;
    to:string;
    qty:number;
    discountRate?:number;
    taxRate?:number;
    unitPrice:number;
    total:number;
    refNo:string;
    constructor(){
        this.date=new Date();
        this.to="";
        this.qty=0;
        this.discountRate=0;
        this.taxRate=0;
        this.unitPrice=0;
        this.total=0;
        this.refNo="";
    }

}
export class ProductSaleDetail{
    purchaseItems:ProductSaleDetailItem[];
    saleItems:ProductSaleDetailItem[];
    constructor(){
        this.purchaseItems=[];
        this.saleItems=[];
    }
}