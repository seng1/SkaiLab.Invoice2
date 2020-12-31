import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { Currency } from '../models/currency';
import { ExchangeRate } from '../models/exchangeRate';
import { NewCurrency } from '../models/new-currency';
import { ParentComponent } from '../parentComponent';
import { CurrecyService } from '../service/currency-service';

@Component({
  selector: 'new-currency-component',
  templateUrl: './new-currency-component.html'
})
export class NewCurrencyComponent extends ParentComponent implements OnInit {
 newCurrency: NewCurrency = new NewCurrency();
 currency:Currency=new Currency();
  constructor( private router: Router,
    private currencyService: CurrecyService,
    private translate: TranslateService) {
    super("New Currency");
    this.ShowBackButton();
    this.setPageTitleFromLocalise(this.translate,"newCurrency");
  }
  ngOnInit(): void {
      this.showProgressBar();
      this.currencyService.getCurrenciesForCreate().subscribe(result=>{
        this.newCurrency=result;
        let c =new Currency();
        c.id=0;
        c.name=this.selectText();
        this.newCurrency.currencies.splice(0, 0, c);
        this.hideProgressBar();
      },error=>{
        this.handleError(error);
      });
  }
  onCurrencySelected(){
    this.currency.exchangeRates=[];
    if(this.currency.id!=0){
        this.newCurrency.currencies.forEach(i=>{
            if(i.id==this.currency.id){
                this.currency.code=i.code;
                this.currency.name=i.name;
                this.currency.symbole=i.symbole;
                this.currency.id=i.id;
            }
        })
        let exchangeRate1=new ExchangeRate();
        exchangeRate1.currencyId=this.newCurrency.taxCurrency.id;
        exchangeRate1.isAuto=false;
        exchangeRate1.exchangeRate=null;
        exchangeRate1.currency=this.newCurrency.taxCurrency;
        this.currency.exchangeRates.push(exchangeRate1);
        if(this.newCurrency.taxCurrency.id!=this.newCurrency.baseCurrency.id){
            let exchangeRate2=new ExchangeRate();
            exchangeRate2.currencyId=this.newCurrency.baseCurrency.id;
            exchangeRate2.isAuto=false;
            exchangeRate2.exchangeRate=null;
            exchangeRate2.currency=this.newCurrency.baseCurrency;
            this.currency.exchangeRates.push(exchangeRate2);
        }
    }
  }
  onSave(){
      if(this.currency.id==0){
          alert("Please select currency");
          return;
      }
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
      this.currencyService.create(this.currency).subscribe(result=>{
        this.hideProgressBar();
        this.router.navigate(['/currencies'])
      },error=>{
        this.handleError(error);
      });
      
  }
}

