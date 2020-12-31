import { strict } from "assert";
import { DateTypeFilter } from "./date-type-filter";
import { DashboardPeriodEnum } from "./enum";

export class Filter
{
    fromDate?:Date;
    toDate?:Date;
    page:number;
    totalPage:number;
    pageSize:number;
    searchText:string;
    organisationId:string;
    totalRow:number;
    dateTypeFilter:DateTypeFilter;
    constructor(){
        this.page=1;
        this.totalPage=0;
        this.pageSize=40;
        this.searchText="";
        this.organisationId="";
        this.totalRow=0;
        this.fromDate=new Date(new Date().setDate(new Date().getDate() - 90));
        this.toDate=new Date();
        this.dateTypeFilter=new DateTypeFilter(0,"");
        
    }
}
export class VendorFilter extends Filter{
    constructor(){
        super();
    }
}
export class CustomerFilter extends Filter{
    constructor(){
        super();
    }
}
export class ProductFilter extends Filter{
    constructor(){
        super();
    }
}
export class PurchaseFilter extends Filter{
    vendorId:number;
    purchaseOrderStatusId:number;
   
    dateFilterMethod:number;
    constructor(){
        super();
        this.vendorId=0;
        this.purchaseOrderStatusId=0;
        
    }
}
export class QuoteFilter extends Filter{
    customerId:number;
    statusId:number;
    constructor(){
        super();
        this.customerId=0;
        this.statusId=0;
    }
}
export class InvoiceFilter extends Filter{
    customerId:number;
    statusId:number;
    constructor(){
        super();
        this.customerId=0;
        this.statusId=0;
    }
}
export class CustomerCreditFilter extends Filter{
    customerId:number;
    statusId:number;
    constructor(){
        super();
        this.customerId=0;
        this.statusId=0;
    }
}
export class DashboardFilter{
    organisationIds:string[];
    periodFilter:DashboardPeriodFilter;
    constructor(){
        this.organisationIds=[];
        this.periodFilter=new DashboardPeriodFilter(DashboardPeriodEnum.Last30Day,"");
    }
}
export class DashboardPeriodFilter{
    id:number;
    name:string;
    constructor(id:number,name:string){
        this.id=id;
        this.name=name;
    }
}
export class PayrollFilter{
    monthFilters:MonthFilter[];
    monthFilter:MonthFilter;
    constructor(){
        this.monthFilters=[];
        var i=12;
        var date=new Date();
        while(i>=0){
            let value=date.getFullYear().toString()+(date.getMonth()+1).toLocaleString('en-US', {minimumIntegerDigits: 2, useGrouping:false});
            this.monthFilters.push(new MonthFilter(value,value));
            date.setMonth(date.getMonth()-1);
            i=i-1;
        }
        this.monthFilter=this.monthFilters[1];
    }
}
export class MonthFilter{
    id:string;
    name:string;
    constructor(id:string,name:string){
        this.id=id;
        this.name=name;
    }
}