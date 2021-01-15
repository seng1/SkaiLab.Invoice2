import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { PieChart } from '../models/chart';
import { Currency } from '../models/currency';
import { DashboardFilter, DashboardPeriodFilter } from '../models/filter';
import { Organisation } from '../models/organisation';
import { Utility } from '../models/utility';
import { ParentComponent } from '../parentComponent';
import { ChartService } from '../service/chart-service';
import { GlobaltranslateService } from '../service/global-translate-service';
import { OrganisationService } from '../service/organisation-service';
import { UserService } from '../service/user-service';
@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',

})
export class HomeComponent extends ParentComponent implements OnInit {
  organisations: any[] = [];
  selectedOrganisations: Organisation[] = [];
  filter: DashboardFilter = new DashboardFilter();
  dashboardPeriodFilters: DashboardPeriodFilter[] = [];
  profitAndLostDate:PieChart=new PieChart();
  showProfitAndLostWating:boolean=false;
  incomeData:PieChart=new PieChart();
  showIncomeWating:boolean=false;
  exepnseData:PieChart=new PieChart();
  showExpenseWating:boolean=false;
  currency:Currency=new Currency();
  options = {
    responsive: true,
    legend: {
      onClick: (e) => e.stopPropagation(),
      labels:{
        fontFamily: this.getFontName()
      }
    },
    animation: {
      duration: 500,
      easing: "easeOutQuart",
      onComplete: function () {
        var ctx = this.chart.ctx;
        ctx.textAlign = 'center';
        ctx.textBaseline = 'bottom';
        ctx.font= "9px Verdana";
        this.data.datasets.forEach(function (dataset) {
          for (var i = 0; i < dataset.data.length; i++) {
            var model = dataset._meta[Object.keys(dataset._meta)[0]].data[i]._model,
              mid_radius = model.innerRadius + (model.outerRadius - model.innerRadius) / 2,
              start_angle = model.startAngle,
              end_angle = model.endAngle,
              mid_angle = start_angle + (end_angle - start_angle) / 2;
              var x = mid_radius * Math.cos(mid_angle);
              var y = mid_radius * Math.sin(mid_angle);
              var val = dataset.data[i];
              ctx.fillStyle = '#444';
              if(val<0){
                ctx.fillStyle = 'Red';
              }
              var percent = String(val);
              if (val != 0) {
                ctx.fillText(percent, model.x + x, model.y + y + 15);
              }
          }
        });
      }
    }
  };
  getFontName():string{
    if(localStorage.getItem("language")==null || localStorage.getItem("language")=="en"){
      return 'Roboto';
    }
    return 'Battambang';
  }
  constructor(private organisationService: OrganisationService, 
    private userService: UserService,
    private chartService:ChartService,
    private translate: TranslateService
    ) {
  
    super("Dashboard");
    this.setPageTitleFromLocalise(this.translate,"dashboard");
    this.multipleDropDownSetting.labelKey = "displayName";
    this.multipleDropDownSetting.enableSearchFilter = false;
  }

  ngOnInit(): void {
    this.filter.organisationIds.push(this.userService.getWorkingOrganisationId().toString());
    this.dashboardPeriodFilters = Utility.getDahboardPeriod();
    this.filter.periodFilter = this.dashboardPeriodFilters[0];
    this.organisationService.getOrganisationsWithSameBaseCurrency().subscribe(result => {
      this.organisations = result;
      this.organisations.forEach(it => {
        if (it.id == this.userService.getWorkingOrganisationId().toString()) {
          this.selectedOrganisations.push(it);
        }
      })
    })
    this.organisationService.getBaseCurrency().subscribe(result=>{
      this.currency=result;
    })
    this.getReport();
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
  getProfitAndLost(){
    console.log(this.filter);
    this.showProfitAndLostWating=true;
    this.chartService.getProfiteAndLost(this.filter).subscribe(result=>{
      this.profitAndLostDate.labels=result.labels;
      this.profitAndLostDate.values=result.values;
      this.showProfitAndLostWating=false;

    },err=>{
      this.handleError(err);
      this.showProfitAndLostWating=false;
    })
  }
  getIncome(){
    this.showIncomeWating=true;
    this.chartService.getIncome(this.filter).subscribe(result=>{
      this.incomeData.labels=result.labels;
      this.incomeData.values=result.values;
      this.showIncomeWating=false;

    },err=>{
      this.handleError(err);
      this.showIncomeWating=false;
    })
  }
  getExpense(){
    this.showExpenseWating=true;
    this.chartService.getExpense(this.filter).subscribe(result=>{
      this.exepnseData.labels=result.labels;
      this.exepnseData.values=result.values;
      this.showExpenseWating=false;

    },err=>{
      this.handleError(err);
      this.showExpenseWating=false;
    })
  }
  getReport(){
    this.getProfitAndLost();
    this.getIncome();
    this.getExpense();
  }
}