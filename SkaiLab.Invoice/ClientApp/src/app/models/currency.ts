import { ExchangeRate } from "./exchangeRate";

export class Currency{
    id:number;
    code:string;
    name:string;
    symbole:string;
    notes:string[];
    exchangeRateTexts:string[];
    exchangeRates:ExchangeRate[];
    constructor(){
        this.id=0;
        this.code="";
        this.name="";
        this.symbole="";
        this.notes=[];
        this.exchangeRateTexts=[];
        this.exchangeRates=[];
    }
}