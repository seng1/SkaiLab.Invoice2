import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { Currency } from '../models/currency';
import { Customer } from '../models/customer';
import { OrganisationParentComponent } from '../OrganisationParentComponent';
import { CurrecyService } from '../service/currency-service';
import { CustomerService } from '../service/customer-service';
import { OrganisationService } from '../service/organisation-service';

@Component({
  selector: 'new-customer-component',
  templateUrl: './new-customer-component.html'
})
export class NewCustomerComponent extends OrganisationParentComponent implements OnInit {
@ViewChild('f', { static: true }) form: NgForm;
  customer:Customer=new Customer();
  currencies:Currency[]=[];
  constructor(private router: Router, private translate: TranslateService,organisationService:OrganisationService,private customerService:CustomerService,private currencyService:CurrecyService ) {
    super("New Customer",organisationService);
    this.setPageTitleFromLocalise(this.translate,"newCustomer");
    this.ShowBackButton();
  }
  ngOnInit(): void {
      this.showProgressBar();
      this.currencyService.getCurrenciesWithoutNote().subscribe(result=>{
        this.currencies=result;
        this.customer.currencyId=this.currencies[0].id;
        this.hideProgressBar();
      },err=>{
          this.handleError(err);
      })
  }
  onSave(isClose:boolean){
    if (this.form.invalid) {
        return;
      }
      this.customer.currency=null;
      this.showProgressBar();
      this.customerService.add(this.customer).subscribe(result=>{
        this.hideProgressBar();
        if(isClose){
            this.router.navigate(['/customer'])
        }
        else{
            this.customer=new Customer();
            this.customer.currencyId=this.currencies[0].id;
        }
      },err=>{
        this.handleError(err);
      });
  }
}

