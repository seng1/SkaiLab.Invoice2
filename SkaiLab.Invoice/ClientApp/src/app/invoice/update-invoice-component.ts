
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { InvoiceService } from '../service/invoice-service';
import { SubTotal } from '../models/sub-total';
import { Currency } from '../models/currency';
import { OrganisationService } from '../service/organisation-service';
import { Invoice } from '../models/customer-transaction';
import { InvoiceStatusEnum, PrintDocumentType } from '../models/enum';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { PrintReceiptComponent } from '../print/print-receipt-component';
import { Attachment } from '../models/attachment';
import { Tax } from '../models/tax';
import { TaxService } from '../service/tax-service';
import { LocationService } from '../service/location-service';
import { Location } from '../models/location';
import { OrganisationParentComponent } from '../OrganisationParentComponent';
import { MenuService } from '../service/menu-service';
import { PrintService } from '../service/print-service';
import { TranslateService } from '@ngx-translate/core';
import { Utility } from '../models/utility';
@Component({
    selector: 'update-invoice-component',
    templateUrl: './update-invoice-component.html'
})
export class UpdateInvoiceFromQuoteComponent extends OrganisationParentComponent implements OnInit {
    invoice:Invoice=new Invoice();
    subTotals: SubTotal[] = [];
    taxCurrency:Currency=new Currency();
    taxes: Tax[] = [];
    locations: Location[] = [];
    constructor(private invoiceService: InvoiceService,
        private router: Router,
        private modalService: NgbModal,
        private taxService: TaxService,
        private locationService: LocationService,
        organisationService:OrganisationService,
        private menuService:MenuService,
        private printService:PrintService,
        private translate: TranslateService,
        private route: ActivatedRoute) {
        super("Invoice Detail",organisationService);
        this.setPageTitleFromLocalise(this.translate,"invoiceDetail");
        this.ShowBackButton();
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
        this.organisationService.getTaxCurrency().subscribe(result=>{
            this.taxCurrency=result;
            if(this.invoice.id!=0){
                this.calculateTotal();
            }
        })
        this.invoiceService.getInvoice(this.id).subscribe(result=>{
            this.invoice=result;
            if(this.taxCurrency.id!=0){
                this.calculateTotal();
            }
            this.hideProgressBar();
        },err=>{
            this.handleError(err);
        })
        this.taxService.getTaxesIncludeComponent().subscribe(result => {
            this.taxes = result;
            let tax =new Tax();
            tax.id=null;
            tax.name=this.selectText();
            this.taxes.splice(0, 0, tax);
        })
        this.locationService.gets().subscribe(result => {
            this.locations = result;
            let location =new Location();
            location.id=null;
            location.name=this.selectText();
            this.locations.splice(0, 0, location);
        })
        this.getPermission(this.menuService);
    }
 
    calculateTotal() {
        this.invoice.total = 0;
        this.invoice.totalIncludeTax = 0;
        let taxSubTotals: SubTotal[] = [];
        this.invoice.customerTransactionItems.forEach(it => {
            it.lineTotal = it.quantity * it.unitPrice;
            if (it.discountRate != null) {
                it.lineTotal -= (it.lineTotal * it.discountRate) / 100;
            }
            it.lineTotalIncludeTax = it.lineTotal;
            if (it.taxId != null) {
                let totalTaxRate: number = 0;
                it.tax.components.forEach(com => {
                    totalTaxRate += com.rate;
                    let hasSubTotalTaax: boolean = false;
                    taxSubTotals.forEach(tax => {
                        if (tax.taxComponentId == com.id) {
                            hasSubTotalTaax = true;
                            tax.amount += (it.lineTotalIncludeTax * com.rate) / 100
                        }
                    })
                    if (hasSubTotalTaax == false) {
                        let subTotal: SubTotal = new SubTotal();
                        subTotal.label = com.name;
                        subTotal.taxComponentId = com.id;
                        subTotal.amount = (it.lineTotalIncludeTax * com.rate) / 100;
                        subTotal.currency = this.invoice.currency;
                        taxSubTotals.push(subTotal);
                    }
                })
                let taxAmount: number = (it.lineTotalIncludeTax * totalTaxRate) / 100;
                it.lineTotalIncludeTax += taxAmount / 100;
            }
            this.invoice.total += it.lineTotal;
            this.invoice.totalIncludeTax += it.lineTotalIncludeTax;
        });
        this.subTotals = [];
        let subTotal: SubTotal = new SubTotal();
        subTotal.label = Utility.subTotalText()+": ";
        subTotal.amount = this.invoice.total;
        subTotal.currency = this.invoice.currency;
        this.subTotals.push(subTotal);
        taxSubTotals.forEach(it => {
            this.subTotals.push(it);
        })
        subTotal = new SubTotal();
        subTotal.label = Utility.totalText()+": ";
        subTotal.amount = this.invoice.totalIncludeTax;
        subTotal.currency = this.invoice.currency;
        this.subTotals.push(subTotal);

        if (taxSubTotals.length > 0 && this.invoice.currencyId != this.taxCurrency.id) {
            let taxExchangeRate: number = 0;
            this.invoice.currency.exchangeRates.forEach(it => {
                if (it.currencyId == this.taxCurrency.id) {
                    taxExchangeRate = it.exchangeRate;
                }
            })
            subTotal = new SubTotal();
            subTotal.label = Utility.exchangeRateText()+": ";;
            subTotal.amount = taxExchangeRate;
            subTotal.currency = this.taxCurrency;
            this.subTotals.push(subTotal);
            subTotal = new SubTotal();
            subTotal.label = Utility.totalInKHRText()+": ";;
            subTotal.amount = taxExchangeRate * this.invoice.totalIncludeTax;
            subTotal.currency = this.taxCurrency;
            this.subTotals.push(subTotal);
        }


    }
    showPayButton(){
        return this.invoice.statusId==InvoiceStatusEnum.WaitingForPayment && this.permission.approvaPayPurchaseSale;
    }
    showLocation(){
        return this.invoice.customerTransactionItems.filter(u=>u.product.trackInventory).length>0;
    }
    getSubTotalColSpan(){
        if(this.showLocation()){
            if(this.declareTax){
                return 6;
            }
            return 5;
        }
        if(this.declareTax){
            return 5;
        }
        return 4;
    }
    onPrint(){
        this.print(this.printService,this.invoice.id,PrintDocumentType.Invoice,this.invoice.number);
    }
    pay(){
        this.showProgressBar();
        this.invoiceService.pay(this.invoice.id).subscribe(result=>{
            this.invoice.statusId=InvoiceStatusEnum.Paid;
            this.hideProgressBar();
        },err=>{
            this.handleError(err);
        });
    }
    onPrintReceipt(){
        const modalRef = this.modalService.open(PrintReceiptComponent);
        modalRef.componentInstance.init(this.invoice,modalRef);
    }
    onOfficalFileChange(attachment:Attachment){
        if(attachment.isFinalOfficalFile){
            this.invoice.attachments.forEach(it=>{
                if(it!=attachment){
                    it.isFinalOfficalFile=false;
                }
            })
            this.showProgressBar();
            this.invoiceService.changeOfficialDocument(this.invoice.id,attachment.fileUrl).subscribe(result=>{
                this.hideProgressBar();
            },err=>{
                this.handleError(err);
            })
        }
    }
    fileChange(event: any) {
        if (event) {
            let self = this;
            var reader = new FileReader();
            reader.readAsDataURL(event.target.files[0]);
            reader.onload = function () {
                self.showProgressBar();
                let fileName=event.target.files[0].name;
                let attachment:Attachment=new Attachment();
                attachment.fileUrl=reader.result.toString();
                attachment.fileName=fileName;
                self.invoiceService.uploadFile(self.invoice.id,attachment).subscribe(result=>{
                    attachment.fileUrl=result.fileUrl;
                    self.invoice.attachments.push(attachment);
                    self.hideProgressBar();
                },err=>{
                    self.handleError(err);
                })
            };
        }
    }
}

