
import { Component, OnInit } from '@angular/core';
import { Currency } from '../models/currency';
import { Customer } from '../models/customer';
import { Product } from '../models/product';
import { Quote, QuoteForUpdateOrCreate, QuoteItem } from '../models/quote';
import { Tax } from '../models/tax';
import { CustomerService } from '../service/customer-service';
import { ProductService } from '../service/product-service';
import { QuoteService } from '../service/quote-service';
import {Location} from '../models/location';
import { DropdownConfig } from '../models/dropdown-config';
import { SubTotal } from '../models/sub-total';
import { PrintDocumentType, QuoteEnum } from '../models/enum';
import { FileService } from '../service/file-service';
import { Router } from '@angular/router';
import $ from "jquery";
import { OrganisationInvoiceSettingService } from '../service/organisation-invoice-setting-service';
import { QuoteUtility, Utility } from '../models/utility';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Attachment } from '../models/attachment';
import { OrganisationParentComponent } from '../OrganisationParentComponent';
import { OrganisationService } from '../service/organisation-service';
import { MenuService } from '../service/menu-service';
import { PrintService } from '../service/print-service';
import { TranslateService } from '@ngx-translate/core';

@Component({
    selector: 'new-quote-component',
    templateUrl: './new-quote-component.html'
})
export class NewQuoteComponent extends OrganisationParentComponent implements OnInit {
    quote: Quote =new Quote();
    customers:Customer[]=[];
    selectCustomer:Customer=null;
    products:Product[]=[];
    selectedProduct:Product=null;
    currencies:Currency[]=[];
    taxes:Tax[]=[];
    locations:Location[]=[];
    quoteForUpdateOrCreate:QuoteForUpdateOrCreate=new QuoteForUpdateOrCreate();
    productDropdownConfig:DropdownConfig=new DropdownConfig();
    subTotals:SubTotal[]=[];
    constructor(private quoteService: QuoteService,
        private productService:ProductService,
        private fileService:FileService,
        private router: Router,
        private menuService:MenuService,
        private translate: TranslateService,
        private organisationInvoiceSettingService: OrganisationInvoiceSettingService,
        private customerService:CustomerService,
        private printService:PrintService,

        organisationService:OrganisationService
        ) {
        super("New Quote",organisationService);
        this.setPageTitleFromLocalise(this.translate,"newQuote");
        this.dropdownConfig.displayKey="displayName";
        this.dropdownConfig.searchOnKey="displayName";
        this.dropdownConfig.placeholder=this.selectText();
        this.productDropdownConfig.placeholder=this.searchProductOrServiceText();
        this.productDropdownConfig.displayKey="name";
        this.ShowBackButton();
    }
    onDeliveryDateChange(event:any){
        $('#delivery').find('.wc-date-container').children('span').show();
    }
    ngOnInit(): void {
        this.customerService.getAll().subscribe(result=>{
            this.customers=result;
        });
        this.productService.getProductsForSale().subscribe(result=>{
            this.products=result;
        });
      
        this.showProgressBar();
        this.quoteService.getLookupForCreae().subscribe(result=>{
            this.currencies=result.currencies;
            this.locations=result.locations;
            this.taxes=result.taxes;
            this.quote.number=result.number;
            this.quote.currencyId=result.baseCurrencyId;
            let tax =new Tax();
            tax.id=null;
            tax.name=this.selectText();
            this.taxes.splice(0, 0, tax);

            let location =new Location();
            location.id=null;
            location.name=this.selectText();
            this.locations.splice(0, 0, location);
            this.currencies.forEach(it=>{
                if(it.id==result.baseCurrencyId){
                    this.quote.currency=it;
                }
            })
            this.quoteForUpdateOrCreate=result;
            this.hideProgressBar();
            
        },err=>{
            this.handleError(err);
        })
        $('#delivery').find('.wc-date-container').children('span').hide();
        this.getTermAndCondition();
        this.getPermission(this.menuService);
    }
    getTermAndCondition(){
        this.organisationInvoiceSettingService.get().subscribe(result=>{
            this.quote.termAndCondition=result.termAndConditionForQuote;
        });
    }
    onSelectedCustomer(event:any){
        this.quote.customerId=this.selectCustomer.id;
        this.quote.customer=this.selectCustomer;
        this.quote.currencyId=this.selectCustomer.currencyId;
        this.currencies.forEach(it=>{
            if(it.id==this.quote.currencyId){
                this.quote.currency=it;
            }
        })
        this.updatePurchasePriceWhenCurrencyChange();
    }
    onCurrencySelectedChange(){
        this.quote.currencyId=this.quote.currency.id;
        this.updatePurchasePriceWhenCurrencyChange();
    }
    addLine(){
        if(this.selectedProduct==null || this.selectedProduct.id==undefined){
            this.showErrorText(Utility.selectProductToAddText());
            return;
        }
        let orderItem:QuoteItem=new QuoteItem();
        orderItem.productId=this.selectedProduct.id;
        orderItem.product=this.selectedProduct;
        orderItem.quantity=1;
        orderItem.taxId=this.selectedProduct.productSaleInformation.taxId;
        orderItem.unitPrice=this.selectedProduct.productSaleInformation.price;
        orderItem.locationId=this.selectedProduct.locationId;
        if(this.quote.currencyId!=this.quoteForUpdateOrCreate.baseCurrencyId){
            this.quote.currency.exchangeRates.forEach(it=>{
                if(it.currencyId==this.quoteForUpdateOrCreate.baseCurrencyId){
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
        this.quote.quoteItems.push(orderItem);
        this.products=this.products;
        this.selectedProduct=null;
        this.calculateTotal();
    }
    calculateTotal(){
        this.quote.total=0;
        this.quote.totalIncludeTax=0;
        let taxSubTotals:SubTotal[]=[];
        this.quote.quoteItems.forEach(it=>{
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
                    subTotal.currency=this.quote.currency;
                    taxSubTotals.push(subTotal);
                  }
              })
              let taxAmount:number=(it.lineTotalIncludeTax*totalTaxRate)/100;
              it.lineTotalIncludeTax+=taxAmount;
            }
            this.quote.total+=it.lineTotal;
            this.quote.totalIncludeTax+=it.lineTotalIncludeTax;
        });
        this.subTotals=[];
        let subTotal:SubTotal=new SubTotal();
        subTotal.label=Utility.subTotalText() +": ";
        subTotal.amount=this.quote.total;
        subTotal.currency=this.quote.currency;
        this.subTotals.push(subTotal);
        taxSubTotals.forEach(it=>{
            this.subTotals.push(it);
        })
        subTotal=new SubTotal();
        subTotal.label=Utility.totalText() +": ";
        subTotal.amount=this.quote.totalIncludeTax;
        subTotal.currency=this.quote.currency;
        this.subTotals.push(subTotal);

        if(taxSubTotals.length>0 && this.quote.currencyId!=this.quoteForUpdateOrCreate.taxCurrency.id){
            let taxExchangeRate:number=0;
            this.quote.currency.exchangeRates.forEach(it=>{
                if(it.currencyId==this.quoteForUpdateOrCreate.taxCurrency.id){
                    taxExchangeRate=it.exchangeRate;
                }
            })
            subTotal=new SubTotal();
            subTotal.label=Utility.exchangeRateText() +": ";
            subTotal.amount=taxExchangeRate;
            subTotal.currency=this.quoteForUpdateOrCreate.taxCurrency;
            this.subTotals.push(subTotal);
            subTotal=new SubTotal();
            subTotal.label=Utility.totalInKHRText() +": ";
            subTotal.amount=taxExchangeRate​*this.quote.totalIncludeTax;
            subTotal.currency=this.quoteForUpdateOrCreate.taxCurrency;
            this.subTotals.push(subTotal);
        }


    }
    updatePurchasePriceWhenCurrencyChange(){
        this.quote.quoteItems.forEach(it=>{
            
            if(this.quote.currencyId==this.quoteForUpdateOrCreate.baseCurrencyId){
                it.unitPrice=it.product.productSaleInformation.price;
            }
            else{
                this.quote.currency.exchangeRates.forEach(rate=>{
                    if(rate.currencyId==this.quoteForUpdateOrCreate.baseCurrencyId){
                        it.unitPrice=this.getTotalAmount(it.product.productSaleInformation.price,rate.exchangeRate)
                    }   
                })
            }
        });
        this.calculateTotal();
    }
    onExchangeRateChange(){
        this.updatePurchasePriceWhenCurrencyChange();
    }
    onRemvoePurchaseOrderItem(orderItem:QuoteItem){
        this.quote.quoteItems = this.quote.quoteItems.filter(obj => obj !== orderItem);
        this.products.push(orderItem.product);
        this.calculateTotal();
    }
    onOrderItemChange(orderItem:QuoteItem){
        this.calculateTotal();
    }
    onTaxSelectChange(orderItem:QuoteItem){
        if(orderItem.taxId!=null){
            if(orderItem.taxId!=null){
                this.quoteForUpdateOrCreate.taxes.forEach(it=>{
                    if(it.id==orderItem.taxId){
                        orderItem.tax=it;
                    }
                })
            }
        }
        else{
            orderItem.tax=null;
        }
        this.calculateTotal();
    }
    onSaveAsDraw(isClose:boolean){
        this.quote.statusId=QuoteEnum.Draft;
        this.onSave(isClose,false);
    }
    onSaveAndAccept(isClose:boolean){
        this.quote.statusId=QuoteEnum.Accepted;
        this.onSave(isClose,false);
    }
    onSave(isClose:boolean,isPrint:boolean){
        var errorTexts:string[]=[];
        if(this.quote.customerId==null || this.quote.customerId==0){
            errorTexts.push(Utility.customerRequireText());
        }
        if(this.quote.date==null){
            errorTexts.push(Utility.dateRequireText());
        }
        if(this.quote.quoteItems.length==0){
            errorTexts.push(Utility.quoteItemRequireText());
        }
        let lineNumber:number=1;
        
        this.quote.quoteItems.forEach(it=>{
            if(it.quantity==null || it.quantity<=0){
                errorTexts.push(Utility.quantityGreaterThenText(lineNumber,0));
            }
            if(it.unitPrice==null || it.unitPrice<0){
                errorTexts.push(Utility.unitPriceGreaterThenText(lineNumber,0));
            }
            if(it.discountRate!=null && it.discountRate<0){
                errorTexts.push(Utility.discountRateMustBetween(lineNumber,0,100));
            }
            lineNumber++;
        });
        if(errorTexts.length>0){
            this.showErrorTexts(errorTexts);
            return;
        }
        this.showProgressBar();
        if(this.quote.id!=0){
            this.quoteService.update(this.quote).subscribe(result=>{
                this.hideProgressBar();
                if(isPrint){
                    this.onPrint();
                }
                else{
                    if(isClose){
                        this.router.navigate(['/quote'])
                    }
                    else{
                        this.quote=new Quote();
                        this.quote.statusId=QuoteEnum.Draft;
                        this.selectCustomer=null;
                        this.quote.currencyId=this.quoteForUpdateOrCreate.baseCurrencyId;
                        this.currencies.forEach(it=>{
                            if(it.id==this.quote.currencyId){
                                this.quote.currency=it;
                            }
                        })
                        $('#delivery').find('.wc-date-container').children('span').hide();
                        this.quote.expireDate=null;
                        this.quote.number=result.number;
                        this.getTermAndCondition();
                    }
                }
               
            },err=>{
                this.handleError(err);
            })
        }
        else{
            this.quoteService.add(this.quote).subscribe(result=>{
                this.hideProgressBar();
                this.quote.id=result.id;
                this.quote.number=result.number;
                if(isPrint){
                    this.onPrint();
                }
                else{
                    if(isClose){
                        this.router.navigate(['/quote'])
                    }
                    else{
                        this.quote=new Quote();
                        this.selectCustomer=null;
                        this.quote.statusId=QuoteEnum.Draft;
                        this.quote.currencyId=this.quoteForUpdateOrCreate.baseCurrencyId;
                        this.currencies.forEach(it=>{
                            if(it.id==this.quote.currencyId){
                                this.quote.currency=it;
                            }
                        })
                        $('#delivery').find('.wc-date-container').children('span').hide();
                        this.quote.expireDate=null;
                        this.quote.number=result.number;
                        this.getTermAndCondition();
                    }
                }
                
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
                self.fileService.upload(reader.result.toString(),fileName).subscribe(result => {
                    let attachment:Attachment=new Attachment();
                    attachment.fileUrl=result.url;
                    attachment.fileName=fileName;
                    self.quote.attachments.push(attachment);
                    self.hideProgressBar();
                }, err => {
                    self.handleError(err);
                })
            };
        }
    }
    onRemoveAttachment(url: Attachment) {
        this.quote.attachments = this.quote.attachments.filter(obj => obj.fileUrl !== url.fileUrl);
    }
    showSaveButtonPanel(){
        return QuoteUtility.showSaveDrawButton(this.quote.statusId);
    }
    showSaveAndPrint(){
        return QuoteUtility.allowSave(this.quote.statusId);
    }
    onSaveAndPrint(){
        this.onSave(false,true);
    }
    onPrint(){
        this.print(this.printService,this.quote.id,PrintDocumentType.Quote,this.quote.number);
    }
    showLocation(){
        return this.quote.quoteItems.filter(u=>u.product.trackInventory).length>0;
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
    onOfficalFileChange(attachment:Attachment){
        if(attachment.isFinalOfficalFile){
            this.quote.attachments.forEach(it=>{
                if(it!=attachment){
                    it.isFinalOfficalFile=false;
                }
            })
        }
    }
}

