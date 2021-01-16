import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { PayrollFilter } from '../models/filter';
import { PayrollMonthNoneTax, PayrollNoneTax } from '../models/payroll';
import { ParentComponent } from '../parentComponent';
import { OrganisationService } from '../service/organisation-service';
import { PayrollService } from '../service/payroll-service';
import { PrintService } from '../service/print-service';
import { ReportService } from '../service/report-service';
@Component({
    selector: 'payroll-no-tax.component',
    templateUrl: './payroll-no-tax.component.html'
})
export class PayrollNoneTaxComponent extends ParentComponent implements OnInit {
    filter: PayrollFilter = new PayrollFilter();
    payroll: PayrollMonthNoneTax = new PayrollMonthNoneTax();
    @ViewChild('content', { static: true }) content: ElementRef;
    showGeneratePayroll:boolean=false;
    constructor(
        private payrollService: PayrollService,
        private router: Router,
        private reportService: ReportService,
        private translate: TranslateService,
        private printService:PrintService,
        private route: ActivatedRoute,
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
            if(result.declareTax){
                this.router.navigate(['/payroll'])
            }   
            else{
                this.getPayroll();
            }
        })
       
    }
    getPayroll() {
        this.showProgressBar();
        this.payrollService.getGetPayrollMonthNoneTax(this.filter.monthFilter.id).subscribe(result => {
            this.hideProgressBar();
            this.payroll = result;
           this.showGeneratePayroll=this.payroll.id==0;

        }, err => {
            this.handleError(err);
        })
    }
    getPayrollTotal(p:PayrollNoneTax){
        var total =p.salary;
        if(p.otherBenefit!=null){
            total+=p.otherBenefit;
        }
        return total;
    }
    getTotal(){
        var total:number=0;
        this.payroll.payrolls.forEach(it=>{
            total+=this.getPayrollTotal(it);
        })
        return total;
    }
    printData() {
        this.reportService.generatePdf("Payroll-"+this.filter.monthFilter.name+ ".pdf","letter","p",this.content.nativeElement.innerHTML);
    }
    onGeneratePayrollClick(){
        if(this.payroll.payrolls.length==0){
            this.showErrorText("No employee to generate payroll");
            return;
        }
        this.showProgressBar();
        this.payrollService.createOrUpdatePayrollNoneTax(this.filter.monthFilter.id,this.payroll).subscribe(result=>{
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
    onGenerateEmployeePaySlip(p:PayrollNoneTax){
        this.showProgressBar();
        this.printService.printPayslip(p.id,"Payroll" + this.filter.monthFilter.name+".pdf").subscribe(result=>{
            this.hideProgressBar();
            this.openFileFromUrl(result.result);
        },err=>{
            this.handleError(err);
        })
    }
}


