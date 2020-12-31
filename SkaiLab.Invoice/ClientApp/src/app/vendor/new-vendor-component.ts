import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { Currency } from '../models/currency';
import { Vendor } from '../models/vendor';
import { OrganisationParentComponent } from '../OrganisationParentComponent';
import { CurrecyService } from '../service/currency-service';
import { OrganisationService } from '../service/organisation-service';
import { VendorService } from '../service/vendor-service';

@Component({
  selector: 'new-vendor-component',
  templateUrl: './new-vendor-component.html'
})
export class NewVendorComponent extends OrganisationParentComponent implements OnInit {
@ViewChild('f', { static: true }) form: NgForm;
  vendor:Vendor=new Vendor();
  currencies:Currency[]=[];
  constructor(private router: Router, private translate: TranslateService,organisationService:OrganisationService,private vendorService:VendorService,private currencyService:CurrecyService ) {
    super("New Vendor",organisationService);
    this.ShowBackButton();
    this.setPageTitleFromLocalise(this.translate,"newVendor");
  }
  ngOnInit(): void {
      this.showProgressBar();
      this.currencyService.getCurrenciesWithoutNote().subscribe(result=>{
        this.currencies=result;
        this.vendor.currencyId=this.currencies[0].id;
        this.hideProgressBar();
      },err=>{
          this.handleError(err);
      })
  }
  onSave(isClose:boolean){
    if (this.form.invalid) {
        return;
      }
      this.vendor.currency=null;
      console.log(this.vendor);
      this.showProgressBar();
      this.vendorService.add(this.vendor).subscribe(result=>{
        this.hideProgressBar();
        if(isClose){
            this.router.navigate(['/vendor'])
        }
        else{
            this.vendor=new Vendor();
            this.vendor.currencyId=this.currencies[0].id;
        }
      },err=>{
        this.handleError(err);
      });
  }
}

