import { Component, ViewChild, ElementRef, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Invoice } from '../models/customer-transaction';
import { ReportFilter } from '../models/report-filter';
import { ParentReportComponent } from '../report/parent-report-component';
import { OrganisationService } from '../service/organisation-service';
import { ReportService } from '../service/report-service';
import { UserService } from '../service/user-service';
@Component({
    selector: 'customer-invoice-component',
    templateUrl: './customer-invoice-component.html',
})
export class CustomerInvoiceReportComponent extends ParentReportComponent implements OnInit {
    @ViewChild('content', { static: true }) content: ElementRef;
    filter: ReportFilter = new ReportFilter();
    invoices: Invoice[] = [];
    constructor(private organisationService: OrganisationService, private router: Router, private userService: UserService, private reportService: ReportService) {
        super("Customer Invoice Report");
        this.ShowBackButton();
        this.multipleDropDownSetting.labelKey = "displayName";
        this.multipleDropDownSetting.enableSearchFilter = false;
    }
    ngOnInit(): void {
        this.init(this.organisationService, this.userService);
        this.getReport();
    }
    getReport() {
        this.showProgressBar();
        this.reportService.getCustomerInvoices(this.filter).subscribe(result => {
            this.hideProgressBar();
            this.invoices = result;
            console.log(result);
        }, err => {
            this.handleError(err);
        })
    }

    onApplyFilter() {
        if (this.selectedOrganisations.length == 0) {
            this.showErrorText("Please select organisation");
            return;
        }
        this.filter.organisationIds = [];
        this.selectedOrganisations.forEach(it => {
            this.filter.organisationIds.push(it.id);
        })
        this.getReport();
    }
    itemClick(invoice: Invoice) {
        this.router.navigate(['/invoice-update', invoice.id]);
    }
    printData() {
        this.reportService.generatePdf("Customer Invoice Report", "letter", "p", this.content.nativeElement.innerHTML);
    }
    onPeriodChange() {
        this.filter.calDate();
    }
    isOverDue(invoice: Invoice) {
        if (invoice.dueDate == null) {
            return false;
        }
        if (invoice.paidDate != null) {
            return false;
        }
        return  new Date(invoice.dueDate)  <= new Date();
    }
    getTotal(){
        var total:number=0;
        this.invoices.forEach(it=>{
            total+=it.totalIncludeTax;
        })
        return total;
    }
    getTotalUnPaid(){
        var total:number=0;
        this.invoices.forEach(it=>{
            if(it.paidDate==null){
                total+=it.totalIncludeTax;
            }
            
        })
        return total;
    }
    getTotalOverdue(){
        var total:number=0;
        this.invoices.forEach(it=>{
            if(it.paidDate==null && new Date(it.dueDate)  <= new Date()){
                total+=it.totalIncludeTax;
            }
            
        })
        return total;
    }
    getTotalPaid(){
        var total:number=0;
        this.invoices.forEach(it=>{
            if(it.paidDate!=null){
                total+=it.totalIncludeTax;
            }
            
        })
        return total;
    }
}

