import { Customer } from "./customer";

export class CustomerBalanceDetailItem{
        date:Date;
        transactionType:string;
        number:string;
        dueDate?:Date;
        amount:number;
        isOverDue:boolean;
        constructor(){
            this.date=new Date();
            this.transactionType="";
            this.number="";
            this.dueDate=new Date();
            this.amount=0;
            this.isOverDue=false;
        }
}
export class CustomerBalanceDetailHeader{
    customerBalanceDetailItems:CustomerBalanceDetailItem[];
    total:number;
    customer:Customer;
    isExpand:boolean;
    parentId:number;
    constructor(){
        this.customerBalanceDetailItems=[];
        this.total=0;
        this.customer=new Customer();
        this.isExpand=true;
        this.parentId=0;
    }
}
export class CustomerBalanceDetail{
    total:number;
    customerBalanceDetailHeaders:CustomerBalanceDetailHeader[];
    constructor(){
        this.total=0;
        this.customerBalanceDetailHeaders=[];
    }
}
