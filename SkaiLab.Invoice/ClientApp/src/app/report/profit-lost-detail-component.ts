import { Component, ViewChild, ElementRef, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { Currency } from '../models/currency';
import { Organisation } from '../models/organisation';
import { ProfitAndLostDetail, ProfitAndLostDetailItem, ProfitAndLostDetailParent } from '../models/ProfitAndLostDetail';
import { ReportFilter } from '../models/report-filter';
import { ParentComponent } from '../parentComponent';
import { OrganisationService } from '../service/organisation-service';
import { ReportService } from '../service/report-service';
import { UserService } from '../service/user-service';
@Component({
  selector: 'profit-lost-detail-component',
  templateUrl: './profit-lost-detail-component.html',
})
export class ProfitAndLostDetailReportComponent extends ParentComponent implements OnInit {
  @ViewChild('content', { static: true }) content: ElementRef;
  filter: ReportFilter = new ReportFilter();
  organisations: any[] = [];
  selectedOrganisations: Organisation[] = [];
  currency: Currency = new Currency();
  printDate:Date=new Date();
  profitAndLostDetail:ProfitAndLostDetail=new ProfitAndLostDetail();
  constructor(private organisationService: OrganisationService,
     private router: Router, 
     private translate: TranslateService,
     private userService: UserService, 
     private reportService: ReportService) {
    super("Profit & Lost Detail Report");
    this.ShowBackButton();
    this.setPageTitleFromLocalise(this.translate,"profitAndLostDetailReport");
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
    this.reportService.getProfitAndLostDetail(this.filter).subscribe(result => {
      this.hideProgressBar();
      this.profitAndLostDetail = result;
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
  onPeriodChange() {
    this.filter.calDate();
  }
  printData() {
    this.profitAndLostDetail.profitAndLostDetailParents.forEach(it=>{
      it.isExpand=true;
    })
    this.reportService.generatePdf("Profit & Lost Detail.pdf","letter","l",this.content.nativeElement.innerHTML);
  }
  expandClick(profitAndLostDetailParent:ProfitAndLostDetailParent){
    profitAndLostDetailParent.isExpand=!profitAndLostDetailParent.isExpand;
  }
  itemClick(profitAndLostDetailItem:ProfitAndLostDetailItem){
    if(profitAndLostDetailItem.transactionType=="Invoice"){
      this.router.navigate(['/invoice-update', profitAndLostDetailItem.parentId]);
      return;
    }
    if(profitAndLostDetailItem.transactionType=="Purchase Order"){
      this.router.navigate(['/order-update', profitAndLostDetailItem.parentId]);
      return;
    }
    if(profitAndLostDetailItem.transactionType=="Employee Salary"){
      this.router.navigate(['/payroll'], { queryParams: { month: profitAndLostDetailItem.number } })
      return;
    }
    this.router.navigate(['/vendor-bill-update', profitAndLostDetailItem.parentId]);
  }
}


