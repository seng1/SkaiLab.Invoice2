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
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import $ from "jquery";
import { QuoteUtility, Utility } from '../models/utility';
import { Attachment } from '../models/attachment';
import { OrganisationParentComponent } from '../OrganisationParentComponent';
import { OrganisationService } from '../service/organisation-service';
import { MenuService } from '../service/menu-service';
import { PrintService } from '../service/print-service';
import { TranslateService } from '@ngx-translate/core';

@Component({
    selector: 'update-quote-component',
    templateUrl: './update-quote-component.html'
})
export class UpdateQuoteComponent extends OrganisationParentComponent implements OnInit {
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
        private router: Router,
        private route: ActivatedRoute,
        private menuService:MenuService,
        private printervice:PrintService,
        private customerService:CustomerService,
        private translate: TranslateService,
        organisationService:OrganisationService) {
        super("Update Quote",organisationService);
        this.setPageTitleFromLocalise(this.translate,"updateQuote")
        this.router.events.subscribe((evt) => {
            if (evt instanceof NavigationEnd) {
              this.router.navigated = false;
              var param = this.route.snapshot.params;
              this.id=param.id.replace(":", "");
            }
          });
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
            if(this.quote.id!=0){
                this.customers.forEach(it=>{
                    if(it.id==this.quote.customerId){
                        this.selectCustomer=it;
                    }
                })
            }
        });
        this.productService.getProductsForSale().subscribe(result=>{
            this.products=result;
        });
        this.showProgressBar();
        this.quoteService.getForUpdate(this.id).subscribe(result=>{
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
            this.quote=result.quote;
            if(this.quote.expireDate==null){
                $('#delivery').find('.wc-date-container').children('span').hide();
            }
            this.currencies.forEach(it=>{
                if(it.id==this.quote.currencyId){
                    this.quote.currency=it;
                }
            })
            this.calculateTotal();
            if(this.customers.length>0){
                this.customers.forEach(it=>{
                    if(it.id==this.quote.customerId){
                        this.selectCustomer=it;
                    }
                })
            }
            this.hideProgressBar();  
           
        },err=>{
            this.handleError(err);
        })
        this.getPermission(this.menuService);
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
    showPrintButton(){
        return QuoteUtility.showPrintButton(this.quote.statusId);
    }
    showSaveDrawButton(){
        return QuoteUtility.showSaveDrawButton(this.quote.statusId) && this.permission.readWritePurchaseSale;
    }
    showSaveAcceptButton(){
        return QuoteUtility.showSaveDrawButton(this.quote.statusId) && this.permission.readWritePurchaseSale &&this.permission.approvaPayPurchaseSale;
    }
    showCreateInvoiceButton(){
        return QuoteUtility.showCreateInvoiceButton(this.quote.statusId) && this.permission.readWritePurchaseSale;
    }
    disableUpdate(){
        return this.quote.statusId!=QuoteEnum.Draft;
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
        subTotal.label=Utility.subTotalText()+": ";
        subTotal.amount=this.quote.total;
        subTotal.currency=this.quote.currency;
        this.subTotals.push(subTotal);
        taxSubTotals.forEach(it=>{
            this.subTotals.push(it);
        })
        subTotal=new SubTotal();
        subTotal.label=Utility.totalText()+": ";
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
            subTotal.label=Utility.exchangeRateText()+": ";
            subTotal.amount=taxExchangeRate;
            subTotal.currency=this.quoteForUpdateOrCreate.taxCurrency;
            this.subTotals.push(subTotal);
            subTotal=new SubTotal();
            subTotal.label=Utility.totalInKHRText()+": ";
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
    onSave(isClose:boolean,isPrint){
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
                errorTexts.push(Utility.quantityGreaterThenText(lineNumber,-1));
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
                    this.router.navigate(['/quote-new'])
                }
            }
         
        },err=>{
            this.handleError(err);
        })
    }
    fileChange(event: any) {
        if (event) {
            let self = this;
            var reader = new FileReader();
            reader.readAsDataURL(event.target.files[0]);
            reader.onload = function () {
                self.showProgressBar();
                let fileName=event.target.files[0].name;
                self.quoteService.addAttachment(self.quote.id,reader.result.toString(),fileName).subscribe(result=>{
                    let attachment:Attachment=new Attachment();
                    attachment.fileUrl=result.url;
                    attachment.fileName=fileName;
                    self.quote.attachments.push(attachment);
                    self.hideProgressBar();
                },err=>{
                    self.handleError(err);
                })
            };
        }
    }
    onRemoveAttachment(url: Attachment) {
        this.showProgressBar();
        this.quoteService.removeAttachment(this.quote.id,url.fileUrl).subscribe(result=>{
            this.quote.attachments = this.quote.attachments.filter(obj => obj !== url);
            this.hideProgressBar();
        },err=>{
            this.handleError(err);
        })
      
    }
    onPrint(){
        this.print(this.printervice,this.quote.id,PrintDocumentType.Quote,this.quote.number);
    }
    showSaveAndPrint(){
        return QuoteUtility.allowSave(this.quote.statusId) && this.permission.readWritePurchaseSale;
    }
    onSaveAndPrint(){
        this.onSave(false,true);
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
        this.showProgressBar();
        this.quoteService.changeOfficialDocument(this.quote.id,attachment.fileUrl).subscribe(result=>{
            this.hideProgressBar();
        },err=>{
            this.handleError(err);
        })
    }
}

