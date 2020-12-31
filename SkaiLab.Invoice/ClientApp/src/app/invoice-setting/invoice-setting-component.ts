import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { OrganisationInvoiceSetting } from '../models/organisation-invoice-setting';
import { ParentComponent } from '../parentComponent';
import { OrganisationInvoiceSettingService } from '../service/organisation-invoice-setting-service';

@Component({
    selector: 'invoice-setting-component',
    templateUrl: './invoice-setting-component.html'
})
export class InvoiceSettingComponent extends ParentComponent implements OnInit {
    @ViewChild('f', { static: true }) form: NgForm;
    organisationSetting:OrganisationInvoiceSetting=new OrganisationInvoiceSetting();
    constructor(private organisationInvoiceSettingService: OrganisationInvoiceSettingService,
        private translate: TranslateService) {
        super("Invoice Setting");
        this.ShowBackButton();
        this.setPageTitleFromLocalise(this.translate,"invoiceSetting");
    }
    ngOnInit(): void {
        this.showProgressBar();
        this.organisationInvoiceSettingService.get().subscribe(result=>{
            this.organisationSetting=result;
            this.hideProgressBar();
            console.log(this.organisationSetting);
        },err=>{
            this.handleError(err);
        })
    }
    onSave(){
        if (this.form.invalid) {
            return;
        }
        this.showProgressBar();
        this.organisationInvoiceSettingService.save(this.organisationSetting).subscribe(result=>{
            this.hideProgressBar();
        },err=>{
            this.handleError(err);
        })
    }
}

