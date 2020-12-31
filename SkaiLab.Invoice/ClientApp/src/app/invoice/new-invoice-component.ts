import { Component, OnInit } from '@angular/core';
import { InvoiceService } from '../service/invoice-service';
import { SubTotal } from '../models/sub-total';
import { Location } from '../models/location';
import { LocationService } from '../service/location-service';
import { Currency } from '../models/currency';
import { OrganisationService } from '../service/organisation-service';
import { ProductService } from '../service/product-service';
import { FileService } from '../service/file-service';
import { CustomerTransactionItem, Invoice } from '../models/customer-transaction';
import { Tax } from '../models/tax';
import { TaxService } from '../service/tax-service';
import { Product } from '../models/product';
import { CurrecyService } from '../service/currency-service';
import { Customer } from '../models/customer';
import { CustomerService } from '../service/customer-service';
import { DropdownConfig } from '../models/dropdown-config';
import { Router } from '@angular/router';
import $ from "jquery";
import { OrganisationInvoiceSettingService } from '../service/organisation-invoice-setting-service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { PrintDocumentType } from '../models/enum';
import { PrintReceiptComponent } from '../print/print-receipt-component';
import { Attachment } from '../models/attachment';
import { OrganisationParentComponent } from '../OrganisationParentComponent';
import { PrintService } from '../service/print-service';
import { Utility } from '../models/utility';
import { TranslateService } from '@ngx-translate/core';

@Component({
    selector: 'new-invoice-component',
    templateUrl: './new-invoice-component.html'
})
export class NewInvoiceComponent extends OrganisationParentComponent implements OnInit {
    invoice: Invoice = new Invoice();
    subTotals: SubTotal[] = [];
    locations: Location[] = [];
    taxes: Tax[] = [];
    products: Product[] = [];
    taxCurrency: Currency = new Currency();
    baseCurrency: Currency = new Currency();
    currencies: Currency[] = [];
    customers: Customer[] = [];
    productDropdownConfig: DropdownConfig = new DropdownConfig();
    selectedProduct: Product = null;
    selectCustomer: Customer = null;
    constructor(private invoiceService: InvoiceService,
        private router: Router,
        private locationService: LocationService,
        private taxService: TaxService,
        private currencyService: CurrecyService,
        organisationService: OrganisationService,
        private productService: ProductService,
        private customerService: CustomerService,
        private invoiceSettingService:OrganisationInvoiceSettingService,
        private modalService: NgbModal,
        private printService:PrintService,
        private translate: TranslateService,
        private fileService: FileService) {
        super("New Invoice",organisationService);
        this.ShowBackButton();
        this.setPageTitleFromLocalise(this.translate,"newInvoice");
        this.dropdownConfig.displayKey = "displayName";
        this.dropdownConfig.searchOnKey = "displayName";
        this.dropdownConfig.placeholder =this.selectText();
        this.productDropdownConfig.placeholder = this.searchProductOrServiceText();
        this.productDropdownConfig.displayKey = "name";
       
    }
    fileChange(event: any) {
        if (event) {
            let self = this;
            var reader = new FileReader();
            reader.readAsDataURL(event.target.files[0]);
            reader.onload = function () {
                self.showProgressBar();
                let fileName=event.target.files[0].name;
                self.fileService.upload(reader.result.toString(),fileName).subscribe(result => {
                    let attachment:Attachment=new Attachment();
                    attachment.fileUrl=result.url;
                    attachment.fileName=fileName;
                    self.invoice.attachments.push(attachment);
                    self.hideProgressBar();
                }, err => {
                    self.handleError(err);
                })
            };
        }
    };
    addLine(){
        if(this.selectedProduct==null || this.selectedProduct.id==undefined){
            this.showErrorText(Utility.selectProductToAddText());
            return;
        }
        let orderItem:CustomerTransactionItem=new CustomerTransactionItem();
        orderItem.productId=this.selectedProduct.id;
        orderItem.product=this.selectedProduct;
        orderItem.quantity=1;
        orderItem.taxId=this.selectedProduct.productSaleInformation.taxId;
        orderItem.unitPrice=this.selectedProduct.productSaleInformation.price;
        orderItem.locationId=this.selectedProduct.locationId;
        if(this.invoice.currencyId!=this.baseCurrency.id){
            this.invoice.currency.exchangeRates.forEach(it=>{
                if(it.currencyId==this.baseCurrency.id){
                    orderItem.unitPrice=this.getTotalAmount(orderItem.unitPrice,it.exchangeRate)
                }
            })
        }
        orderItem.discountRate=null;
        if(orderItem.taxId!=null){
            this.taxes.forEach(it=>{
                if(it.id==orderItem.taxId){
                    orderItem.tax=it;
                }
            })
        }
        this.invoice.customerTransactionItems.push(orderItem);
        this.products=this.products;
        this.selectedProduct=null;
        this.calculateTotal();
        this.getInventoryQty(orderItem);
    }
    onRemoveAttachment(url: Attachment) {
        this.invoice.attachments = this.invoice.attachments.filter(obj => obj.fileUrl !== url.fileUrl);
    }
    ngOnInit(): void {
        this.locationService.gets().subscribe(result => {
            this.locations = result;
            let location =new Location();
            location.id=null;
            location.name=this.selectText();
            this.locations.splice(0, 0, location);
        })
        this.productService.getProductsForSale().subscribe(result => {
            this.products = result;
        })
        this.organisationService.getTaxCurrency().subscribe(result => {
            this.taxCurrency = result;
        })

        this.customerService.getAll().subscribe(result => {
            this.customers = result
        })
        this.currencyService.getCurrenciesWithExchangeRate().subscribe(result => {
            this.currencies = result;
            this.organisationService.getBaseCurrency().subscribe(result => {
                this.baseCurrency = result;
                this.invoice.currencyId = this.baseCurrency.id;
                this.currencies.forEach(it => {
                    if (it.id == this.invoice.currencyId) {
                        this.baseCurrency = it
                        this.invoice.currency = it;
                       
                    }
                })
            })
        })
        this.taxService.getTaxesIncludeComponent().subscribe(result => {
            this.taxes = result;
            let tax =new Tax();
            tax.id=null;
            tax.name=this.selectText();
            this.taxes.splice(0, 0, tax);
        })
        this.getInvoiceNumber();
        this.invoiceSettingService.get().subscribe(result=>{
            this.invoice.termAndCondition=result.termAndConditionForInvoice;
        });
    }
    onSelectedCustomer(event: any) {
        this.invoice.customerId = this.selectCustomer.id;
        this.invoice.customer = this.selectCustomer;
        this.invoice.currencyId = this.selectCustomer.currencyId;
        this.currencies.forEach(it => {
            if (it.id == this.invoice.currencyId) {
                this.invoice.currency = it;
            }
        })
        this.updatePurchasePriceWhenCurrencyChange();
    }
    getInvoiceNumber(){
        console.log(this.invoice);
        this.invoiceService.GenerateInvoiceNumber(this.invoice).subscribe(result=>{
            this.invoice.number=result.number
        })
    }
    onDeliveryDateChange() {
        $('#delivery').find('.wc-date-container').children('span').show();
    }
    onCurrencySelectedChange() {
        this.invoice.currencyId = this.invoice.currency.id;
        this.updatePurchasePriceWhenCurrencyChange();
    }
    onExchangeRateChange() {
        this.updatePurchasePriceWhenCurrencyChange();
    }
    onRemvoePurchaseOrderItem(orderItem: CustomerTransactionItem) {
        this.invoice.customerTransactionItems = this.invoice.customerTransactionItems.filter(obj => obj !== orderItem);
        this.calculateTotal();
    }
    onOrderItemChange(orderItem: CustomerTransactionItem) {
        this.calculateTotal();
    }
    onTaxSelectChange(orderItem: CustomerTransactionItem) {
        if (orderItem.taxId != null) {
            if (orderItem.taxId != null) {
                this.taxes.forEach(it => {
                    if (it.id == orderItem.taxId) {
                        orderItem.tax = it;
                    }
                })
            }
        }
        else {
            orderItem.tax = null;
        }
        this.calculateTotal();
    }
    calculateTotal(){
        this.invoice.total=0;
        this.invoice.totalIncludeTax=0;
        let taxSubTotals:SubTotal[]=[];
        this.invoice.customerTransactionItems.forEach(it=>{
            it.lineTotal=it.quantity*it.unitPrice;
            if(it.discountRate!=null){
                it.lineTotal-=(it.lineTotal*it.discountRate)/100;
            }
            it.lineTotalIncludeTax=it.lineTotal;
            if(it.tax!=null){
              let totalTaxRate:number=0;
              it.tax.components.forEach(com=>{
                  totalTaxRate+=com.rate;
                  let hasSubTotalTaax:boolean=false;
                  taxSubTotals.forEach(tax=>{
                    if(tax.taxComponentId==com.id){
                        hasSubTotalTaax=true;
                        tax.amount+=(it.lineTotalIncludeTax*com.rate)/100
                    }
                  })
                  if(hasSubTotalTaax==false){
                    let subTotal:SubTotal=new SubTotal();
                    subTotal.label=com.name;
                    subTotal.taxComponentId=com.id;
                    subTotal.amount=(it.lineTotalIncludeTax*com.rate)/100;
                    subTotal.currency=this.invoice.currency;
                    taxSubTotals.push(subTotal);
                  }
              })
              let taxAmount:number=(it.lineTotalIncludeTax*totalTaxRate)/100;
              it.lineTotalIncludeTax+=taxAmount/100;
            }
            this.invoice.total+=it.lineTotal;
            this.invoice.totalIncludeTax+=it.lineTotalIncludeTax;
        });
        this.subTotals=[];
        let subTotal:SubTotal=new SubTotal();
        subTotal.label=Utility.subTotalText()+": ";
        subTotal.amount=this.invoice.total;
        subTotal.currency=this.invoice.currency;
        this.subTotals.push(subTotal);
        taxSubTotals.forEach(it=>{
            this.subTotals.push(it);
        })
        subTotal=new SubTotal();
        subTotal.label=Utility.totalText()+": ";
        subTotal.amount=this.invoice.totalIncludeTax;
        subTotal.currency=this.invoice.currency;
        this.subTotals.push(subTotal);

        if(taxSubTotals.length>0 && this.invoice.currencyId!=this.taxCurrency.id){
            let taxExchangeRate:number=0;
            this.invoice.currency.exchangeRates.forEach(it=>{
                if(it.currencyId==this.taxCurrency.id){
                    taxExchangeRate=it.exchangeRate;
                }
            })
            subTotal=new SubTotal();
            subTotal.label=Utility.exchangeRateText()+": ";
            subTotal.amount=taxExchangeRate;
            subTotal.currency=this.taxCurrency;
            this.subTotals.push(subTotal);
            subTotal=new SubTotal();
            subTotal.label=Utility.totalInKHRText()+": ";
            subTotal.amount=taxExchangeRate​*this.invoice.totalIncludeTax;
            subTotal.currency=this.taxCurrency;
            this.subTotals.push(subTotal);
           
        }
        this.invoice.isTaxIncome=this.invoice.customerTransactionItems.filter(u=>u.taxId!=null).length>0;
        this.getInvoiceNumber()

    }
    updatePurchasePriceWhenCurrencyChange(){
        this.invoice.customerTransactionItems.forEach(it=>{
            
            if(this.invoice.currencyId==this.baseCurrency.id){
                it.unitPrice=it.product.productSaleInformation.price;
            }
            else{
                this.invoice.currency.exchangeRates.forEach(rate=>{
                    if(rate.currencyId==this.baseCurrency.id){
                        it.unitPrice=this.getTotalAmount(it.product.productSaleInformation.price,rate.exchangeRate)
                    }   
                })
            }
        });
        this.calculateTotal();
    }
    getInventoryQty(invoiceItem:CustomerTransactionItem){
        if(invoiceItem.product.trackInventory){
            if(invoiceItem.locationId!=null){
                invoiceItem.requestingInventory=true
                this.productService.getInventoryQty(invoiceItem.productId,invoiceItem.locationId).subscribe(result=>{
                    invoiceItem.inventoryQty=result
                    invoiceItem.requestingInventory=false
                },err=>{
                    this.handleError(err);
                })
            }
            else{
                invoiceItem.inventoryQty=null
                invoiceItem.requestingInventory=false
            }
        }
    }
    onSave(isPrint:boolean){
        var errorTexts:string[]=[];
        if(this.invoice.customerId==null || this.invoice.customerId==0){
            errorTexts.push(Utility.customerRequireText());
        }
        if(this.invoice.date==null){
            errorTexts.push(Utility.dateRequireText());
        }
        if(this.invoice.customerTransactionItems.length==0){
            errorTexts.push(Utility.lineItemRequireText());
        }
        let lineNumber:number=1;
        this.invoice.customerTransactionItems.forEach(it=>{
            if(it.quantity==null || it.quantity<=0){
                errorTexts.push(Utility.quantityGreaterThenText(lineNumber,0));
            }
            if(it.unitPrice==null || it.unitPrice<0){
                errorTexts.push(Utility.unitPriceGreaterThenText(lineNumber,0));
            }
            if(it.discountRate!=null && (it.discountRate<0||it.discountRate>100)){
                errorTexts.push(Utility.discountRateMustBetween(lineNumber,0,100));
            }
            if(it.product.trackInventory && it.locationId==null){
                errorTexts.push(Utility.locationInLineRequireText(lineNumber));
            }
            lineNumber++;
        });
        if(errorTexts.length>0){
            this.showErrorTexts(errorTexts);
            return;
        }
        this.showProgressBar();
        this.invoiceService.create(this.invoice).subscribe(result=>{
             this.hideProgressBar();
             if(isPrint){
                this.invoice.id=result.id;
                this.invoice.number=result.number;
                this.onPrint();
             }
             else{
                this.router.navigate(['/invoice'])
             }
             
        },err=>{
            this.handleError(err);
        })
     }
     onPrint(){
         this.print(this.printService,this.invoice.id,PrintDocumentType.Invoice,this.invoice.number);
    }
    allowSave(){
        return this.invoice.id==0;
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
}

