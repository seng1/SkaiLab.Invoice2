export class Tax{
    id:number;
    name:string;
    organisationId:string;
    totalRate:number;
    components:TaxComponent[];
    constructor(){
        this.id=0;
        this.name="";
        this.organisationId="";
        this.totalRate=0;
        this.components=[];
    }
}
export class TaxComponent{
    id:number;
    name:string;
    rate:number;
    constructor(){
        this.id=0;
        this.name="";
        this.rate=0;
    }
}