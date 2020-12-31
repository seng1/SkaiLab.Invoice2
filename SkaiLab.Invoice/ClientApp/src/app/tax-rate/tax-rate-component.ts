import { Component, OnInit, ViewChild } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { Tax } from '../models/tax';
import { ParentComponent } from '../parentComponent';
import { TaxService } from '../service/tax-service';

@Component({
    selector: 'tax-rate-component',
    templateUrl: './tax-rate-component.html'
})
export class TaxRateComponent extends ParentComponent implements OnInit {
    taxes:Tax[]=[];
    constructor(private taxService:TaxService,private translate: TranslateService) {
        super("Tax Rates");
        this.ShowBackButton();
        this.setPageTitleFromLocalise(this.translate,"taxes");
    }
    ngOnInit(): void {
      this.showProgressBar();
      this.taxService.gets().subscribe(result=>{
        this.taxes=result;
        this.hideProgressBar();
      },err=>{
          this.handleError(err);
      })
    }
  
}

