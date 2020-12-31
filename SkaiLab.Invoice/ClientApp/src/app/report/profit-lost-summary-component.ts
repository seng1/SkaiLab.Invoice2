import { Component, ViewChild, ElementRef, OnInit } from '@angular/core';
import { Currency } from '../models/currency';
import { Organisation } from '../models/organisation';
import { ProfitAndLostSummary } from '../models/ProfitAndLostSummary';
import { ProfitAndLostSummaryFilter } from '../models/report-filter';
import { ParentComponent } from '../parentComponent';
import { OrganisationService } from '../service/organisation-service';
import { ReportService } from '../service/report-service';
import { UserService } from '../service/user-service';
import { DashboardPeriodEnum } from '../models/enum';
import { TranslateService } from '@ngx-translate/core';
@Component({
  selector: 'profit-lost-summary-component',
  templateUrl: './profit-lost-summary-component.html',
})
export class ProfitAndLostSummaryReportComponent extends ParentComponent implements OnInit {
  @ViewChild('content', { static: true }) content: ElementRef;
  filter: ProfitAndLostSummaryFilter = new ProfitAndLostSummaryFilter();
  organisations: any[] = [];
  selectedOrganisations: Organisation[] = [];
  currency: Currency = new Currency();
  printDate:Date=new Date();
  profitAndLostSummary: ProfitAndLostSummary = new ProfitAndLostSummary();
  constructor(private organisationService: OrganisationService,
    private translate: TranslateService,
     private userService: UserService, 
     private reportService: ReportService) {
    super("Profit & Lost Summary Report");
    this.ShowBackButton();
    this.setPageTitleFromLocalise(this.translate,"productAndServiceSummaryReport");
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
  onPeriodChange() {
    this.filter.calDate();
  }
  printData() {
    var orientation:string="p";
    if(this.filter.periodFilter.id==DashboardPeriodEnum.LastYear || this.filter.periodFilter.id==DashboardPeriodEnum.ThisYear){
      orientation="l";
    }
    this.reportService.generatePdf("Profit & Lost Summary.pdf","letter",orientation,this.content.nativeElement.innerHTML);
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
  getReport() {
    this.showProgressBar();
    this.reportService.getProfitAndLostSummary(this.filter).subscribe(result => {
      this.hideProgressBar();
      this.profitAndLostSummary = result;
      console.log(result);
    }, err => {
      this.handleError(err);
    })
  }
  
}


