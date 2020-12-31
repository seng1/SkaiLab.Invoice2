import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { Tax, TaxComponent } from '../models/tax';
import { ParentComponent } from '../parentComponent';
import { TaxService } from '../service/tax-service';

@Component({
  selector: 'new-tax-component',
  templateUrl: './new-tax-component.html'
})
export class NewTaxRateComponent extends ParentComponent implements OnInit {
    tax:Tax=new Tax();
    @ViewChild('f', { static: true }) form: NgForm;
  constructor( private router: Router,
    private taxService: TaxService,
    private translate: TranslateService) {
    super("New Tax Rate");
    this.ShowBackButton();
    this.setPageTitleFromLocalise(this.translate,"newTax");
  }
  ngOnInit(): void {
    this.onAddComponent();
  }
  onRemoveComponent(component:TaxComponent){
    this.tax.components = this.tax.components.filter(obj => obj !== component);
  }
  onAddComponent(){
    let compnent:TaxComponent=new TaxComponent();
    this.tax.components.push(compnent);
  }
  onSave(isClose:boolean){
    if (this.form.invalid) {
        return;
      }
      if(this.tax.components.length==0){
          this.showErrorText("The component is require");
          return;
      }
      let isValidated:boolean=true;
      this.tax.components.forEach(it=>{
        if(it.name.length==0 || it.rate==null || it.rate<0){
            isValidated=false;
            this.showErrorText("The component name and rate are require");
        }
      });
      if(!isValidated){
          return false;
      }
      this.showProgressBar();
      this.taxService.create(this.tax).subscribe(result=>{
        this.hideProgressBar();
        if(isClose){
            this.router.navigate(['/taxrate'])
        }
        else{
            this.tax=new Tax();
            this.onAddComponent();
        }
      },err=>{
          this.handleError(err);
      })
  }
}

