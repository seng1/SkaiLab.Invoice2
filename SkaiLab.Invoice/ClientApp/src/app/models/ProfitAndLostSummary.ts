export class ProfitAndLostSummaryToal{
    name:string;
    values:number[];
    constructor(){
        this.name="";
        this.values=[];
    }
}
export class ProfitAndLostSummaryRow{
    name:string;
    values:number[];
    constructor(){
        this.name="";
        this.values=[];
    }
}
export class ProfitAndLostSummaryRowHeader{
    name:string;
    profitAndLostSummaryRows:ProfitAndLostSummaryRow[];
    constructor(){
        this.name="";
        this.profitAndLostSummaryRows=[];
    }
}
export class ProfitAndLostSummary{
    headers:string[];
    profitAndLostSummaryRowHeaders:ProfitAndLostSummaryRowHeader[];
    profitAndLostSummaryToal:ProfitAndLostSummaryToal;
    constructor(){
        this.headers=[];
        this.profitAndLostSummaryRowHeaders=[];
        this.profitAndLostSummaryToal=new ProfitAndLostSummaryToal();

    }
}