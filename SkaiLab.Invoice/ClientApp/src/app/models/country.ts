export class Country{
    id:number;
    alpha2Code:string;
    alpha3Code:string;
    name:string;
    nationality:string;
    constructor(){
        this.id=0;
        this.alpha2Code="";
        this.alpha3Code="";
        this.name="";
        this.nationality="";
    }
}