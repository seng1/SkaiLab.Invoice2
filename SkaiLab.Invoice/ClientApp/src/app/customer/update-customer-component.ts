import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { Currency } from '../models/currency';
import { Customer } from '../models/customer';
import { OrganisationParentComponent } from '../OrganisationParentComponent';
import { CurrecyService } from '../service/currency-service';
import { CustomerService } from '../service/customer-service';
import { MenuService } from '../service/menu-service';
import { OrganisationService } from '../service/organisation-service';

@Component({
  selector: 'update-customer-component',
  templateUrl: './update-customer-component.html'
})
export class UpdateCustomerComponent extends OrganisationParentComponent implements OnInit {
@ViewChild('f', { static: true }) form: NgForm;
  customer:Customer=new Customer();
  currencies:Currency[]=[];
  constructor(private router: Router,
    private customerService:CustomerService,
    organisationService:OrganisationService,
    private currencyService:CurrecyService, 
    private menuService:MenuService,
    private translate: TranslateService,
    private route: ActivatedRoute ) {
    super("Update Customer",organisationService);
    this.ShowBackButton();
    this.setPageTitleFromLocalise(this.translate,"updateCustomer");
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
      this.customerService.get(this.id).subscribe(result=>{
        this.customer=result;
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
      this.customer.currency=null;
      this.showProgressBar();
      this.customerService.update(this.customer).subscribe(result=>{
        this.hideProgressBar();
        if(isClose){
            this.router.navigate(['/customer'])
        }
        else{
            this.router.navigate(['/customer-new'])
        }
      },err=>{
        this.handleError(err);
      });
  }
}

