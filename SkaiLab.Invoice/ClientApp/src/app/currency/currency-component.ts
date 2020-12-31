import { Component, OnInit, ViewChild } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { Currency } from '../models/currency';
import { ParentComponent } from '../parentComponent';
import { CurrecyService } from '../service/currency-service';

@Component({
  selector: 'currency-component',
  templateUrl: './currency-component.html'
})
export class CurrencyComponent extends ParentComponent implements OnInit {
    currencies: Currency[] = [];
  constructor( private currencyService: CurrecyService,
    private translate: TranslateService) {
    super("Currencies");
    this.ShowBackButton();
    this.setPageTitleFromLocalise(this.translate,"currencies");
  }
  ngOnInit(): void {
    this.showProgressBar();
    this.currencyService.getOrganisationCurrencies().subscribe(result=>{
        this.currencies=result;
        this.hideProgressBar();
    },error=>{

    })
  }
}

