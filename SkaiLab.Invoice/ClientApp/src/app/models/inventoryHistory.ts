import {Location} from "./location";

export class InventoryHistory{
    locationId:number;
    location:Location;
    quantity:number;
    unitPrice:number;
    constructor(){
        this.locationId=0;
        this.location=new Location();
        this.quantity=0;
        this.unitPrice=0;
    }
}
export class InventoryHistoryDetail extends InventoryHistory{
    date:Date;
    refNo:string;
    amount:number;
    constructor(){
        super();
        this.date=new Date();
        this.refNo="";
        this.amount=0;
    }
}