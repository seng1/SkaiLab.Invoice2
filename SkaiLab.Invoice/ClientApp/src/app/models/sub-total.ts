import { Currency } from "./currency";

export class SubTotal{
    label:string;
    amount:number;
    currency:Currency;
    taxComponentId:number;
    constructor(){
        this.label="";
        this.amount=0;
        this.currency=new Currency();
        this.taxComponentId=0;
    }
}