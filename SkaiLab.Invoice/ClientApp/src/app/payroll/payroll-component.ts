import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { PayrollFilter } from '../models/filter';
import { Payroll, PayrollMonthTax, PayrollTax } from '../models/payroll';
import { ParentComponent } from '../parentComponent';
import { OrganisationService } from '../service/organisation-service';
import { PayrollService } from '../service/payroll-service';
import { PrintService } from '../service/print-service';
import { ReportService } from '../service/report-service';
@Component({
    selector: 'payroll-component',
    templateUrl: './payroll-component.html'
})
export class PayrollComponent extends ParentComponent implements OnInit {
    filter: PayrollFilter = new PayrollFilter();
    payroll: PayrollMonthTax = new PayrollMonthTax();
    @ViewChild('content', { static: true }) content: ElementRef;
    showGeneratePayroll:boolean=false;
    constructor(
        private payrollService: PayrollService,
        private router: Router,
        private reportService: ReportService,
        private printService:PrintService,
        private route: ActivatedRoute,
        private translate: TranslateService,
        private organisationService:OrganisationService) {
        super("Pay run");
        this.ShowBackButton();
        this.setPageTitleFromLocalise(this.translate,"payRun");
        this.route.queryParamMap.subscribe((parm)=>{
            var month=parm.get("month");
            if(month!=null){
                this.filter.monthFilters.forEach(it=>{
                    if(it.id==month){
                        this.filter.monthFilter=it;
                    }
                })
            }
            });
    }
    ngOnInit(): void {
        this.showProgressBar();
        this.organisationService.get().subscribe(result=>{
            if(!result.declareTax){
                this.router.navigate(['/payroll-nonetax'], { queryParams: { month: this.filter.monthFilter.id } })
            }  
            else{
                this.getPayroll();
            } 
        })
    }
    getPayroll() {
        this.showProgressBar();
        this.payrollService.getMonthTax(this.filter.monthFilter.id).subscribe(result => {
            this.hideProgressBar();
            this.payroll = result;
           this.onExchangeRateChange();
           this.showGeneratePayroll=this.payroll.id==0;

        }, err => {
            this.handleError(err);
        })
    }
    getTotalNet(payroll: PayrollTax): number {
        var total = payroll.salary - payroll.deductSalary;
        if (payroll.otherBenefit != null) {
            total += payroll.otherBenefit - payroll.otherBenefitTaxDeduct;
        }
        return total;
    }
    getTotalTax(payroll: PayrollTax): number {
        var total = payroll.deductSalary;
        if (payroll.otherBenefitTaxDeduct != null) {
            total += payroll.otherBenefitTaxDeduct;
        }
        return total;
    }
    onSalaryChange(payroll: PayrollTax) {
        if (!payroll.employee.isResidentEmployee) {
            payroll.deductSalary = (payroll.salary * this.payroll.noneResidentRate) / 100;
        }
        else {
            this.calculateTaxSalary(payroll);
        }
    }
    onOtherBenefitChange(payroll: PayrollTax){
        if(!payroll.employee.isResidentEmployee){
            payroll.otherBenefitTaxDeduct=null;
            return;
        }
        if(payroll.otherBenefit==null){
            payroll.otherBenefitTaxDeduct=null;
            return;
        }
        if(payroll.otherBenefit<0){
            payroll.otherBenefit=0;
            payroll.otherBenefitTaxDeduct=0;
            return;
        }
        payroll.otherBenefitTaxDeduct=(payroll.otherBenefit * this.payroll.additionalBenefitsRate)/100;
        
    }
    onExchangeRateChange(){
        this.payroll.payrolls.forEach(it=>{
            if(it.employee.isResidentEmployee){
                this.calculateTaxSalary(it);
            }
        })
    }
    calculateTaxSalary(payroll: PayrollTax) {
        var salary = payroll.salary * this.payroll.exchangeRate;
        var numberOfChild = payroll.employee.numberOfChild;
        if (payroll.employee.isConfederationThatHosts) {
            numberOfChild += 1;
        }
        salary = salary - (numberOfChild * this.payroll.childOrSpouseAmount);
        var rate: number = this.findRate(salary);
        var taxAmount: number = this.getDeductAmount(salary, rate);
        payroll.deductSalary = taxAmount / this.payroll.exchangeRate;
    }
    findRate(salary: number): number {

        if (salary <= 0) {
            return 0;
        }
        var result: number = 0;
        for (var i = 0; i < this.payroll.taxSalaryRanges.length; i++) {
            if (this.payroll.taxSalaryRanges[i].toAmount == null && salary>=this.payroll.taxSalaryRanges[i].fromAmount) {
                result = this.payroll.taxSalaryRanges[i].rate;
                break;
            }
            if (salary >= this.payroll.taxSalaryRanges[i].fromAmount && salary <= this.payroll.taxSalaryRanges[i].toAmount) {
                result = this.payroll.taxSalaryRanges[i].rate;
                break;
            }
        }
        return result;
    }
    getDeductAmount(salary: number, rate: number): number {
        var deductAmountFromPreviuseRate: number = 0;
        for (var i = 0; i < this.payroll.taxSalaryRanges.length; i++) {
            if (this.payroll.taxSalaryRanges[i].rate >= rate) {
                break;
            }
            if (this.payroll.taxSalaryRanges[i].rate > 0) {
                var amount = this.payroll.taxSalaryRanges[i].toAmount - this.payroll.taxSalaryRanges[i - 1].toAmount;
                deductAmountFromPreviuseRate += (amount * this.payroll.taxSalaryRanges[i].rate) / 100;
            }

        }
        var deductAmount: number = 0;
        for (var i = 0; i < this.payroll.taxSalaryRanges.length; i++) {
            if (this.payroll.taxSalaryRanges[i].rate == rate) {
                var amount = salary - this.payroll.taxSalaryRanges[i].fromAmount;
                deductAmount = (amount * rate) / 100;
                break;
            }
        }
        deductAmount += deductAmountFromPreviuseRate;
        return deductAmount;
    }
    getTotalColSpan():number{
        if(this.payroll.currency.id!=this.payroll.taxCurrency.id){
            return 13;
        }
        return 11;
    }
    getTotalExpense():number{
        var total:number=0;
        this.payroll.payrolls.forEach(it=>{
            total+=this.getTotalNet(it)+this.getTotalTax(it);
        })
        return total;
    }
    printData() {
        
        this.reportService.generatePdf("Payroll-"+this.filter.monthFilter.name+ ".pdf","a3","l",this.content.nativeElement.innerHTML);
    }
    onGeneratePayrollClick(){
        if(this.payroll.payrolls.length==0){
            this.showErrorText("No employee to generate payroll");
            return;
        }
        this.showProgressBar();
        this.payrollService.createOrUpdatePayrollTax(this.filter.monthFilter.id,this.payroll).subscribe(result=>{
            this.payroll=result;
            this.hideProgressBar();
            this.showGeneratePayroll=false;
        },err=>{
            this.handleError(err);
        })
    }
    onRegeneratePayrollClick(){
        this.showGeneratePayroll=true;
    }
    onGenerateEmployeePaySlip(p:PayrollTax){
        this.showProgressBar();
        this.printService.printPayslip(p.id,"Payroll" + this.filter.monthFilter.name+".pdf").subscribe(result=>{
            this.hideProgressBar();
            this.openFileFromUrl(result.result);
        },err=>{
            this.handleError(err);
        })
    }
    onGenerateEmployeePaySlips(){
        this.showProgressBar();
        let ids:number[]=[];
        this.payroll.payrolls.forEach(it=>{
            ids.push(it.id);
        })
        this.printService.printPayslips(this.filter.monthFilter.name,ids).subscribe(result=>{
            this.hideProgressBar();
            this.openFileFromUrl(result.result);
        },err=>{
            this.handleError(err);
        })
    }
}


