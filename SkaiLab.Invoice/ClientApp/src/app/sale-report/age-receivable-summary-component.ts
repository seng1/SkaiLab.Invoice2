import { Component, ViewChild, ElementRef, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ReportFilter } from '../models/report-filter';
import { ParentReportComponent } from '../report/parent-report-component';
import { OrganisationService } from '../service/organisation-service';
import { ReportService } from '../service/report-service';
import { UserService } from '../service/user-service';
@Component({
  selector: 'age-receivable-summary-component',
  templateUrl: './age-receivable-summary-component.html',
})
export class AgedReceivablesSummaryComponent extends ParentReportComponent implements OnInit {
  @ViewChild('content', { static: true }) content: ElementRef;
  filter: ReportFilter = new ReportFilter();
  constructor(private organisationService: OrganisationService, private router: Router, private userService: UserService, private reportService: ReportService) {
    super("Aged Receivables Summary Report");
    this.ShowBackButton();
    this.multipleDropDownSetting.labelKey = "displayName";
    this.multipleDropDownSetting.enableSearchFilter = false;
  }
    ngOnInit(): void {
      this.init(this.organisationService,this.userService);
      this.getWorkingOrganisation(this.userService);
      this.filter.organisationIds.push(this.userService.getWorkingOrganisationId().toString());
      this.filter.fromDate=new Date();
    }
    printData() {
      this.reportService.generatePdf("Aged Receivables Summary Report.pdf","letter","l",this.content.nativeElement.innerHTML);
    }
  }

