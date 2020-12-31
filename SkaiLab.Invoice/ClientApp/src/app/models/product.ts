import { InventoryHistory } from "./inventoryHistory";
import { Tax } from "./tax";
import {Location} from './location';

export class Product{
    id:number;
    organisationId:string;
    code:string;
    name:string;
    imageUrl:string;
    trackInventory:boolean;
    productPurchaseInformation:ProductSalePurchaseDetail;
    productSaleInformation:ProductSalePurchaseDetail;
    inventoryHistories:InventoryHistory[];
    locationId:number;
    location:Location;
    qtyBalance:number;
    constructor(){
        this.id=0;
        this.organisationId="";
        this.code="";
        this.imageUrl="";
        this.trackInventory=false;
        this.productPurchaseInformation=new ProductSalePurchaseDetail();
        this.productSaleInformation=new ProductSalePurchaseDetail();
        this.inventoryHistories=[];
        this.locationId=null;
        this.location=new Location();
        this.qtyBalance=0;
    }
}
export class ProductSalePurchaseDetail{
    price:number;
    taxId?:number;
    tax:Tax;
    description:string;
    title:string;
    constructor(){
        this.price=0;
        this.taxId=null;
        this.tax=null;
        this.description="";
        this.title="";
    }
}