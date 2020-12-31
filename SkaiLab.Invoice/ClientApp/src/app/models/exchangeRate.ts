import { Currency } from "./currency";

export class ExchangeRate{
    currencyId:number;
    exchangeRate:number;
    isAuto:boolean;
    currency:Currency;
    constructor(){
        this.currencyId=0;
        this.exchangeRate=0;
        this.isAuto=true;
        this.currency=new Currency();
    }
}