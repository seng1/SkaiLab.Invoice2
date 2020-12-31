import { Currency } from "./currency";
import { Employee } from "./employee";

export class PayrollMonth{
    id:number;
    month:string;
    total:number;
    exchangeRate:number;
    startDate:Date;
    endDate:Date;
    organisationId:string;
    currency:Currency;
    constructor(){
        this.id=0;
        this.month="";
        this.exchangeRate=0;
        this.startDate=new Date();
        this.endDate=new Date();
        this.organisationId="";
        this.currency=new Currency();
    }
}
export class PayrollMonthTax extends PayrollMonth{
    childOrSpouseAmount:number;
    noneResidentRate:number;
    additionalBenefitsRate:number;
    taxSalaryRanges:TaxSalaryRange[];
    payrolls:PayrollTax[];
    taxCurrency:Currency;
    constructor(){
        super();
        this.childOrSpouseAmount=0;
        this.noneResidentRate=0;
        this.additionalBenefitsRate=0;
        this.taxSalaryRanges=[];
        this.payrolls=[];
        this.taxCurrency=new Currency();
    }
}
export class PayrollMonthNoneTax extends PayrollMonth{
    payrolls:PayrollNoneTax[];
    constructor(){
        super();
        this.payrolls=[];
    }
}
export class TaxSalaryRange{
    id:number;
    fromAmount:number;
    toAmount?:number;
    rate:number;
    constructor(){
        this.id=0;
        this.fromAmount=0;
        this.toAmount=0;
        this.rate=0;
    }

}

export class Payroll{
    id:number;
    employeeId:number;
    employee:Employee;
    date:Date;
    transactionDate:Date;
    total:number;
    constructor(){
        this.id=0;
        this.employeeId=0;
        this.employee=new Employee();
        this.date=new Date();
        this.transactionDate=new Date();
        this.total=0;
    }

}
export class PayrollNoneTax extends Payroll{
    salary:number;
    otherBenefit?:number;
    constructor(){
        super();
        this.salary=0;
        this.otherBenefit=0;
    }

}
export class PayrollTax extends Payroll{
    salary:number;
    numberOfChilds?:number;
    hasSpouse:boolean;
    deductSalary:number;
    otherBenefit?:number;
    otherBenefitTaxDeduct?:number;
    constructor(){
        super();
        this.salary=0;
        this.numberOfChilds=0;
        this.hasSpouse=false;
        this.deductSalary=0;
        this.otherBenefit=0;
        this.otherBenefitTaxDeduct=0;
    }

}