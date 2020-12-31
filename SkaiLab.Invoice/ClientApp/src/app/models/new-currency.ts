import { Currency } from "./currency";

export class NewCurrency{
    currencies:Currency[];
    baseCurrency:Currency;
    taxCurrency:Currency;
    constructor(){
        this.currencies=[];
        this.baseCurrency=new Currency();
        this.taxCurrency=new Currency();
    }
}