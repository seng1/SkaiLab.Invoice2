import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { Tax } from '../models/tax';
import { ParentComponent } from '../parentComponent';
import { TaxService } from '../service/tax-service';

@Component({
    selector: 'update-tax-component',
    templateUrl: './update-tax-component.html'
})
export class UpdateTaxRateComponent extends ParentComponent implements OnInit {
    tax: Tax = new Tax();
    @ViewChild('f', { static: true }) form: NgForm;
    constructor(private router: Router, 
        private taxService: TaxService, 
        private translate: TranslateService,
        private route: ActivatedRoute) {
        super("Update Tax");
        this.ShowBackButton();
        this.setPageTitleFromLocalise(this.translate,"updateTax");
        this.router.events.subscribe((evt) => {
            if (evt instanceof NavigationEnd) {
                this.router.navigated = false;
                var param = this.route.snapshot.params;
                this.id = param.id.replace(":", "");
            }
        });
    }
    ngOnInit(): void {
        this.showProgressBar();
        this.taxService.get(this.id).subscribe(result=>{
            this.tax=result;
            this.hideProgressBar();
        },err=>{
            this.handleError(err);
        })

    }
    onSave(isClose: boolean) {
        if (this.form.invalid) {
            return;
        }
        if (this.tax.components.length == 0) {
            this.showErrorText("The component is require");
            return;
        }
        let isValidated: boolean = true;
        this.tax.components.forEach(it => {
            if (it.name.length == 0 || it.rate == null || it.rate < 0) {
                isValidated = false;
                this.showErrorText("The component name and rate are require");
            }
        });
        if (!isValidated) {
            return false;
        }
        this.showProgressBar();
        this.taxService.update(this.tax).subscribe(result => {
            this.hideProgressBar();
            if (isClose) {
                this.router.navigate(['/taxrate'])
            }
            else {
                this.tax = new Tax();
                this.router.navigate(['/taxrate-new'])
            }
        }, err => {
            this.handleError(err);
        })
    }
}

