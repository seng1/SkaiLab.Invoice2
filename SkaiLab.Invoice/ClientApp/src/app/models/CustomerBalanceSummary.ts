import { Customer } from "./customer";

export class CustomerBalanceSummary{
    customer:Customer;
    total:number;
    constructor(){
        this.customer=new Customer();
        this.total=0;
    }
}