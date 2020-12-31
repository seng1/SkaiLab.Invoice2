import { DashboardPeriodEnum, DisplayColumnEnum } from "./enum";
import { DashboardPeriodFilter } from "./filter";
import { Utility } from "./utility";

export class ReportFilter{
    organisationIds:string[];
    periodFilter:DashboardPeriodFilter;
    fromDate:Date;
    toDate:Date;
    periods:DashboardPeriodFilter[];
    displayColumns:DisplayColumn[];
    displayColumn:DisplayColumn;
    constructor(){
        this.organisationIds=[];
        this.fromDate=new Date();
        this.toDate=new Date();
        this.periods=Utility.getReportPerid();
        this.periodFilter=this.periods[0];
        this.calDate();
    }
    calDate(){
        switch(this.periodFilter.id){
            case DashboardPeriodEnum.Last30Day:
                this.fromDate=new Date();
                this.fromDate.setDate(this.fromDate.getDate()-30);
                this.toDate=new Date();
                break;
            case DashboardPeriodEnum.ThisMonth:
                this.fromDate=Utility.beginningOfMonth(new Date());
                this.toDate=Utility.endOfMonth(new Date());
                break;
            case DashboardPeriodEnum.ThisQuarter:
                this.fromDate=Utility.beginningOfMonth(Utility.getBeginQuaterFromDate(new Date()));
                this.toDate=Utility.getBeginQuaterFromDate(new Date());
                this.toDate.setMonth(this.toDate.getMonth()+2);
                this.toDate=Utility.endOfMonth(this.toDate);
                break;
            case DashboardPeriodEnum.ThisYear:
                this.fromDate=new Date();
                this.fromDate.setMonth(0);
                this.fromDate=Utility.beginningOfMonth(this.fromDate);
                this.toDate=new Date();
                this.toDate.setMonth(11);
                this.toDate=Utility.endOfMonth(this.toDate);
                break;
            case DashboardPeriodEnum.LastMonth:
                this.fromDate=new Date();
                this.fromDate.setMonth(this.fromDate.getMonth()-1);
                this.fromDate=Utility.beginningOfMonth(this.fromDate);
                this.toDate=Utility.endOfMonth(this.fromDate);
                break;
            case DashboardPeriodEnum.LastQuater:
                this.fromDate=Utility.beginningOfMonth(Utility.getPreviousQuaterFromDate(new Date()));
                this.toDate=Utility.getPreviousQuaterFromDate(new Date());
                this.toDate.setMonth(this.toDate.getMonth()+2);
                this.toDate=Utility.endOfMonth(this.toDate);
                break;
            case DashboardPeriodEnum.LastYear:
                this.fromDate=new Date();
                this.fromDate.setFullYear(this.fromDate.getFullYear()-1);
                this.fromDate.setMonth(0);
                this.fromDate=Utility.beginningOfMonth(this.fromDate);
                this.toDate=new Date();
                this.toDate.setFullYear(this.toDate.getFullYear()-1);
                this.toDate.setMonth(11);
                this.toDate=Utility.endOfMonth(this.toDate);
                break;

        }
    }
}
export class ProfitAndLostSummaryFilter extends ReportFilter{
    constructor(){
        super();
        this.displayColumns=[];
        if(Utility.isKhmer()){
            this.displayColumns.push(new DisplayColumn(DisplayColumnEnum.Month,"ខែ"));
            this.displayColumns.push(new DisplayColumn(DisplayColumnEnum.TotalOnly,"សរុបតែប៉ុណ្ណោះ"));
            this.displayColumns.push(new DisplayColumn(DisplayColumnEnum.Organisation,"អង្គភាពឬក្រុមហ៊ុន"));
        }
        else{
            this.displayColumns.push(new DisplayColumn(DisplayColumnEnum.Month,"Month"));
            this.displayColumns.push(new DisplayColumn(DisplayColumnEnum.TotalOnly,"Total only"));
            this.displayColumns.push(new DisplayColumn(DisplayColumnEnum.Organisation,"Organisation"));
        }
      
        this.displayColumn=this.displayColumns[0];
    }
}
export class InventoryHistoryFilter extends ReportFilter{
    pageSize:number;
    page:number;
    constructor(){
        super();
        this.pageSize=50;
        this.page=1;
        this.periodFilter=this.periods[2];
    }
}
export class DisplayColumn{
    id:number;
    name:string;
    constructor(id:number,name:string){
        this.id=id;
        this.name=name;
    }
}