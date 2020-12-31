export class ProfitAndLostDetailItem {
    date: Date;
    transactionType: string;
    number: string;
    clientOrVendorName: string;
    description: string;
    amount: number;
    parentId:number;
    constructor() {
        this.date = new Date();
        this.transactionType = "";
        this.number = "";
        this.clientOrVendorName = "";
        this.description = "";
        this.amount = 0;
        this.parentId=0;
    }
}
export class ProfitAndLostDetailParent{
    name:string;
    total:number;
    isExpand:boolean;
    profitAndLostDetailItems:ProfitAndLostDetailItem[];
    constructor(){
        this.name="";
        this.total=0;
        this.isExpand=true;
        this.profitAndLostDetailItems=[];
    }
}
export class ProfitAndLostDetailTotal{
    name:string;
    total:number;
    constructor(){
        this.name="";
        this.total=0;
    }
}
export class ProfitAndLostDetail{
    profitAndLostDetailTotal:ProfitAndLostDetailTotal;
    profitAndLostDetailParents:ProfitAndLostDetailParent[];
    constructor(){
        this.profitAndLostDetailTotal=new ProfitAndLostDetailTotal();
        this.profitAndLostDetailParents=[];
    }
}