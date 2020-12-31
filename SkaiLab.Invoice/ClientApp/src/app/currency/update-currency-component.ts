import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { Currency } from '../models/currency';
import { ParentComponent } from '../parentComponent';
import { CurrecyService } from '../service/currency-service';

@Component({
  selector: 'update-currency-component',
  templateUrl: './update-currency-component.html'
})
export class UpdateCurrencyComponent extends ParentComponent implements OnInit {
  currency:Currency=new Currency();
  constructor( private router: Router,
    private currencyService: CurrecyService, 
    private translate: TranslateService,
    private route: ActivatedRoute) {
    super("Update Currency");
    this.ShowBackButton();
    this.setPageTitleFromLocalise(this.translate,"updateCurrency");
    this.router.events.subscribe((evt) => {
        if (evt instanceof NavigationEnd) {
          this.router.navigated = false;
          var param = this.route.snapshot.params;
          this.id=param.id.replace(":", "");
        }
      });
  }
  ngOnInit(): void {
      this.showProgressBar();
      this.currencyService.getCurrencyWithExchangeRate(this.id).subscribe(result=>{
        this.currency=result;
        this.hideProgressBar();
      },error=>{
        this.handleError(error);
      });
  }
  onSave(){
      var isValidated:boolean=true;
      this.currency.exchangeRates.forEach(it=>{
          if(it.exchangeRate==null || it.exchangeRate<0){
              isValidated=false;
              this.showErrorText("Exchange rate is require");
              return;
          }
      });
      if(!isValidated){
          return false;
      }
      this.showProgressBar();
      this.currencyService.updateExchangeRate(this.currency).subscribe(result=>{
        this.hideProgressBar();
        this.router.navigate(['/currencies'])
      },error=>{
        this.handleError(error);
      });
      
  }
}

