export class Plan{
    id:number;
    name:string;
    monthlyPrice:number;
    monthlyRenewalPrice:number;
    yearlyPrice:number;
    yearlyRenewalPrice:number;
    yearlySavePercent:number;
    constructor(){
        this.id=0;
        this.name="";
        this.monthlyPrice=0;
        this.monthlyRenewalPrice=0;
        this.yearlyPrice=0;
        this.yearlyRenewalPrice=0;
        this.yearlySavePercent=0;
    }
}