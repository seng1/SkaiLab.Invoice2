import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { Currency } from '../models/currency';
import { Organisation } from '../models/organisation';
import { OrganisationType } from '../models/organisationType';
import { Utility } from '../models/utility';
import { CurrecyService } from '../service/currency-service';
import { OrganisationService } from '../service/organisation-service';
import { OrganisationTypeService } from '../service/organisation-type-service';

@Component({
    selector: 'new-company-component',
    templateUrl: './new-company-component.html'
})
export class NewCompanyComponent implements OnInit {
    private modalRef: NgbModalRef;
    organisation: Organisation = new Organisation();
    organisationTypes: OrganisationType[] = [];
    currencies: Currency[] = [];
    @ViewChild('f', { static: true }) form: NgForm;
    baseCurrency: Currency = new Currency();
    taxCurrency: Currency = new Currency();
    showLoading:boolean=false;
    constructor(private router: Router,
        private organisationTypeService: OrganisationTypeService,
        private currencyService: CurrecyService,
        private organisationService: OrganisationService) {
    }
    ngOnInit(): void {
        this.organisationTypeService.gets().subscribe(result => {
            this.organisationTypes = result;
            let tax = new OrganisationType();
            tax.id = null;
            tax.name = Utility.selectText();
            this.organisationTypes.splice(0, 0, tax);
        });
        this.currencyService.getCurrenciesWithoutOrganisation().subscribe(result => {
            this.currencies = result;
            let currency = new Currency();
            currency.id = null;
            currency.name = Utility.selectText();
            this.currencies.splice(0, 0, currency);
            this.currencies.forEach(it => {
                if (it.code == "USD") {
                    this.organisation.organisationBaseCurrency.baseCurrencyId = it.id;
                    this.baseCurrency = it;
                }
                if (it.code == "KHR") {
                    this.organisation.organisationBaseCurrency.taxCurrencyId = it.id;
                    this.taxCurrency = it;
                }
            })
        });
    }
    baseCurrencyChange() {
        this.currencies.forEach(it => {
            if (it.id == this.organisation.organisationBaseCurrency.baseCurrencyId) {
                this.baseCurrency = it;
            }
        })
    }
    taxCurrencyChange() {
        this.currencies.forEach(it => {
            if (it.id == this.organisation.organisationBaseCurrency.taxCurrencyId) {
                this.taxCurrency = it;
            }
        })
    }
    init(modalRef: NgbModalRef) {
        this.modalRef = modalRef;
    }
    close() {
        this.modalRef.close();
    }
    fileChange(event: any) {
        if (event) {
            let self = this;
            var reader = new FileReader();
            reader.readAsDataURL(event.target.files[0]);
            reader.onload = function () {
                self.organisation.logoUrl = reader.result.toString();
            };
        }
    };
    errorBaseCurrency() {
        return this.form.submitted && this.organisation.organisationBaseCurrency.baseCurrencyId == null;
    }
    errorOrganisationType() {
        return this.form.submitted && this.organisation.organisationTypeId == null;
    }
    errorTaxCurrency() {
        return this.form.submitted && this.organisation.organisationBaseCurrency.taxCurrencyId == null;
    }
    onCreate() {
        if (this.form.invalid) {
            return;
        }
        this.showLoading=true;
        this.organisationService.add(this.organisation).subscribe(result=>{
            window.location.reload();
        },err=>{
            this.showLoading=false;
            alert(err);
        })
    }
}

