
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { InvoiceService } from '../service/invoice-service';
import { CustomerTransactionItem, Invoice } from '../models/customer-transaction';
import { SubTotal } from '../models/sub-total';
import {Location} from '../models/location';
import { LocationService } from '../service/location-service';
import { Currency } from '../models/currency';
import { OrganisationService } from '../service/organisation-service';
import { ProductService } from '../service/product-service';
import { FileService } from '../service/file-service';
import { Tax } from '../models/tax';
import { TaxService } from '../service/tax-service';
import { OrganisationInvoiceSettingService } from '../service/organisation-invoice-setting-service';
import { PrintComponent } from '../print/print-component';
import { PrintDocumentType } from '../models/enum';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { PrintReceiptComponent } from '../print/print-receipt-component';
import { Attachment } from '../models/attachment';
import { OrganisationParentComponent } from '../OrganisationParentComponent';
import { PrintService } from '../service/print-service';
import { TranslateService } from '@ngx-translate/core';
import { Utility } from '../models/utility';

@Component({
    selector: 'new-invoice-quote-component',
    templateUrl: './new-invoice-quote-component.html'
})
export class NewInvoiceFromQuoteComponent extends OrganisationParentComponent implements OnInit {
    private quoteId:any;
    invoice:Invoice=new Invoice();
    subTotals: SubTotal[] = [];
    locations:Location[]=[];
    taxes: Tax[] = [];
    taxCurrency:Currency=new Currency();
    constructor(private invoiceService: InvoiceService,
        private router: Router,
        private locationService:LocationService,
        organisationService:OrganisationService,
        private productService:ProductService,
        private fileService:FileService,
        private taxService:TaxService,
        private modalService: NgbModal,
        private printService:PrintService,
        private translate: TranslateService,
        private invoiceSettingService:OrganisationInvoiceSettingService,
        private route: ActivatedRoute
        ) {
        super("Create Invoice",organisationService);
        this.ShowBackButton();
        this.setPageTitleFromLocalise(this.translate,"newInvoice");
        this.router.events.subscribe((evt) => {
            if (evt instanceof NavigationEnd) {
              this.router.navigated = false;
              var param = this.route.snapshot.params;
              this.quoteId=param.id.replace(":", "");
            }
          });
    }
    fileChange(event: any) {
        if (event) {
          let self = this;
          var reader = new FileReader();
          reader.readAsDataURL(event.target.files[0]);
          reader.onload = function () {
             self.showProgressBar();
             let fileName=event.target.files[0].name;
             self.fileService.upload(reader.result.toString(),fileName).subscribe(result=>{
                let attachment:Attachment=new Attachment();
                attachment.fileUrl=result.url;
                attachment.fileName=fileName;
                self.invoice.attachments.push(attachment);
                self.hideProgressBar();
             },err=>{
                 self.handleError(err);
             })
          };
        }
      };
      onRemoveAttachment(url:Attachment){
        this.invoice.attachments = this.invoice.attachments.filter(obj => obj.fileUrl !== url.fileUrl);
      }
    ngOnInit(): void {
        this.showProgressBar();
        this.invoiceService.generateInvoiceFromQuote(this.quoteId).subscribe(result=>{
            this.invoice=result;
            this.hideProgressBar();
            if(this.taxCurrency.id!=0){
                this.calculateTotal();
            }
            this.invoice.customerTransactionItems.forEach(it=>{
                if(it.product.trackInventory){
                    this.getInventory(it)
                }
            });
            this.invoiceSettingService.get().subscribe(result=>{
                this.invoice.termAndCondition=result.termAndConditionForInvoice;
            });
        },err=>{
            this.handleError(err);
        })
        this.locationService.gets().subscribe(result=>{
            this.locations=result;
            let location =new Location();
            location.id=null;
            location.name=this.selectText();
            this.locations.splice(0, 0, location);
        })
        this.organisationService.getTaxCurrency().subscribe(result=>{
            this.taxCurrency=result;
            if(this.invoice.id!=0){
                this.calculateTotal();
            }
        })
        this.taxService.getTaxesIncludeComponent().subscribe(result => {
            this.taxes = result;
            let tax =new Tax();
            tax.id=null;
            tax.name=this.selectText();
            this.taxes.splice(0, 0, tax);
        })
        
    }
    getInventory(invoiceItem:CustomerTransactionItem){
        if(invoiceItem.locationId!=null){
            invoiceItem.requestingInventory=true;
            this.productService.getInventoryQty(invoiceItem.productId,invoiceItem.locationId).subscribe(result=>{
                invoiceItem.inventoryQty=result;
                invoiceItem.requestingInventory=false;
               
            })
        }
        else{
            invoiceItem.inventoryQty=null;
        }
    }
    
    onExchangeRateChange(){
        this.calculateTotal();
    }
    onDateChange(event:any){
        this.showProgressBar();
        this.invoiceService.GenerateInvoiceNumber(this.invoice).subscribe(result=>{
            this.invoice.number=result.number;
            this.hideProgressBar();
        },err=>{
            this.handleError(err);
        })
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
            subTotal.label = Utility.totalInKHRText()+": ";
            subTotal.amount = taxExchangeRate * this.invoice.totalIncludeTax;
            subTotal.currency = this.taxCurrency;
            this.subTotals.push(subTotal);
        }


    }
    onSave(isPrint:boolean){
       var errorText:string[]=[];
       let lineNumber:number=1;
       this.invoice.customerTransactionItems.forEach(it=>{
           if(it.product.trackInventory&&it.locationId==null){
               errorText.push(Utility.locationInLineRequireText(lineNumber));
           }
           lineNumber+=1;
       })
       if(errorText.length>0){
           this.showErrorTexts(errorText);
           return;
       }
       this.showProgressBar();
       this.invoiceService.createInvoiceFromQuote(this.invoice).subscribe(result=>{
         this.hideProgressBar();
           if(isPrint){
            this.invoice.id=result.id;
            this.invoice.number=result.number;
            this.onPrint();
           }
           else{
            this.router.navigate(['/invoice']);
           }
       },err=>{
           this.handleError(err);
       })
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
    allowSave(){
        return this.invoice.id==0;
    }
    onPrint(){
        this.print(this.printService,this.invoice.id,PrintDocumentType.Invoice,this.invoice.number);
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
        }
    }
    onRemvoePurchaseOrderItem(orderItem: CustomerTransactionItem) {
        this.invoice.customerTransactionItems = this.invoice.customerTransactionItems.filter(obj => obj !== orderItem);
        this.calculateTotal();
    }
}

