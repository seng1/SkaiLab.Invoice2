import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { PayrollFilter } from '../models/filter';
import { PayrollTax } from '../models/payroll';
import { TaxMonthly } from '../models/tax-monthly';
import { ParentComponent } from '../parentComponent';
import { PrintService } from '../service/print-service';
import { ReportService } from '../service/report-service';

@Component({
    selector: 'tax-monthly-component.ts',
    templateUrl: 'tax-monthly-component.html'
})
export class TaxMonthlyReportComponent extends ParentComponent implements OnInit {
    filter: PayrollFilter = new PayrollFilter();
    taxMonthly:TaxMonthly=new TaxMonthly();
    @ViewChild('content', { static: true }) content: ElementRef;
    constructor(private reportService: ReportService,
        private translate: TranslateService,
        private printService:PrintService) {
        super("Tax Monthly Report");
        this.ShowBackButton();
        this.setPageTitleFromLocalise(this.translate,"taxMonthlyReport");
    }
    ngOnInit(): void {
      this.getReport();
    }
    getReport(){
        this.showProgressBar();
        this.reportService.getTaxMonthly(this.filter.monthFilter.id).subscribe(result=>{
            this.taxMonthly=result;
            this.taxMonthly.showEmployeePanel=false;
            this.taxMonthly.showExpensePanel=false;
            this.taxMonthly.showInvoicePanel=false;
            this.hideProgressBar();
        },err=>{
            this.handleError(err);
        })
    }
    getPayrollTotalDeduct(payroll:PayrollTax){
        var total=payroll.deductSalary;
        if(payroll.otherBenefitTaxDeduct!=null){
            total+=payroll.otherBenefitTaxDeduct;
        }
        return total;
    }
    getTotalSalary(){
        var total=0;
        this.taxMonthly.payroll.payrolls.forEach(it=>{
            total+=it.salary;
        })
        return total;
    }
    getTotalOtherBenefit(){
        var total=0;
        this.taxMonthly.payroll.payrolls.forEach(it=>{
           if(it.otherBenefit!=null){
               total+=it.otherBenefit;
           }
        })
        return total;
    }
    onPrint(){
        this.reportService.generatePdf("Tax Monthly Report for "+this.filter.monthFilter.name+".pdf","letter","l",this.content.nativeElement.innerHTML);
    }
    onInvoiceExpandClick(){
        this.taxMonthly.showInvoicePanel=!this.taxMonthly.showInvoicePanel;
    }
    onExpenseExpandClick(){
        this.taxMonthly.showExpensePanel=!this.taxMonthly.showExpensePanel;
    }
    onEmployeeExpandClick(){
        this.taxMonthly.showEmployeePanel=!this.taxMonthly.showEmployeePanel;
    }
    onDownload(){
        this.showProgressBar();
        this.printService.downloadTax(this.filter.monthFilter.id).subscribe(result=>{
            this.hideProgressBar();
            window.open(result.result, "_blank");
        },err=>{
            this.handleError(err);
        })
    }
}


