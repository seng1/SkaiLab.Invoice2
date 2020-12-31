import { Component, ViewChild, ElementRef, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Currency } from '../models/currency';
import { CustomerBalanceSummary } from '../models/CustomerBalanceSummary';
import { Organisation } from '../models/organisation';
import { ReportFilter } from '../models/report-filter';
import { ParentComponent } from '../parentComponent';
import { OrganisationService } from '../service/organisation-service';
import { ReportService } from '../service/report-service';
import { UserService } from '../service/user-service';
@Component({
  selector: 'customer-summary-balance-component',
  templateUrl: './customer-summary-balance-component.html',
})
export class CustomerSummaryBalanceReportComponent extends ParentComponent implements OnInit {
  @ViewChild('content', { static: true }) content: ElementRef;
  filter: ReportFilter = new ReportFilter();
  organisations: any[] = [];
  selectedOrganisations: Organisation[] = [];
  currency: Currency = new Currency();
  printDate:Date=new Date();
  customerBalanceSummaries:CustomerBalanceSummary[]=[];
  constructor(private organisationService: OrganisationService, private router: Router, private userService: UserService, private reportService: ReportService) {
    super("Customer Summary Balance Report");
    this.ShowBackButton();
    this.multipleDropDownSetting.labelKey = "displayName";
    this.multipleDropDownSetting.enableSearchFilter = false;
  }
    ngOnInit(): void {
      this.filter.organisationIds.push(this.userService.getWorkingOrganisationId().toString());
      this.organisationService.getOrganisationsWithSameBaseCurrency().subscribe(result => {
        this.organisations = result;
        this.organisations.forEach(it => {
          if (it.id == this.userService.getWorkingOrganisationId().toString()) {
            this.selectedOrganisations.push(it);
          }
        })
      })
      this.organisationService.getBaseCurrency().subscribe(result => {
        this.currency = result;
      });
      this.getReport();
    }
    getReport() {
      this.showProgressBar();
      this.reportService.getCustomerBalanceSummary(this.filter).subscribe(result => {
        this.hideProgressBar();
        this.customerBalanceSummaries = result;
        console.log(result);
      }, err => {
        this.handleError(err);
      })
    }
    getSelectedOrganisationNames(){
      var names="";
      this.selectedOrganisations.forEach(it=>{
        if(names!=""){
          names+=";";
        }
        names+=it.displayName;
      })
      return names;
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
    printData() {
      this.reportService.generatePdf("Customer Summary Balance Report","letter","p",this.content.nativeElement.innerHTML);
    }
    getTotal(){
        var total:number=0;
        this.customerBalanceSummaries.forEach(it=>{
            total+=it.total;
        })
        return total;
    }
  }

