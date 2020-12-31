import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { Currency } from '../models/currency';
import { Vendor } from '../models/vendor';
import { OrganisationParentComponent } from '../OrganisationParentComponent';
import { CurrecyService } from '../service/currency-service';
import { MenuService } from '../service/menu-service';
import { OrganisationService } from '../service/organisation-service';
import { VendorService } from '../service/vendor-service';

@Component({
  selector: 'update-vendor-component',
  templateUrl: './update-vendor-component.html'
})
export class UpdateVendorComponent extends OrganisationParentComponent implements OnInit {
@ViewChild('f', { static: true }) form: NgForm;
  vendor:Vendor=new Vendor();
  currencies:Currency[]=[];
  constructor(private router: Router,
    organisationService:OrganisationService,
    private translate: TranslateService,
    private vendorService:VendorService,
    private currencyService:CurrecyService, 
    private menuService:MenuService,
    private route: ActivatedRoute ) {
    super("Update Vendor",organisationService);
    this.ShowBackButton();
    this.setPageTitleFromLocalise(this.translate,"updateVendor");
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
      this.currencyService.getCurrenciesWithoutNote().subscribe(result=>{
        this.currencies=result;
      },err=>{
          this.handleError(err);
      });
      this.vendorService.get(this.id).subscribe(result=>{
        this.vendor=result;
        this.hideProgressBar();
      },err=>{
          this.handleError(err);
      })
      this.getPermission(this.menuService);
  }
  onSave(isClose:boolean){
    if (this.form.invalid) {
        return;
      }
      this.vendor.currency=null;
      console.log(this.vendor);
      this.showProgressBar();
      this.vendorService.update(this.vendor).subscribe(result=>{
        this.hideProgressBar();
        if(isClose){
            this.router.navigate(['/vendor'])
        }
        else{
            this.router.navigate(['/vendor-new'])
        }
      },err=>{
        this.handleError(err);
      });
  }
}

