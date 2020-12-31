import { Component, OnInit, ViewChild } from '@angular/core';
import { DropdownConfig } from '../models/dropdown-config';
import { Product } from '../models/product';
import { PurchaseOrderLookup } from '../models/purchase-order-lookup';
import { PurchaseOrder, PurchaseOrderItem } from '../models/purchaseOrder';
import { SubTotal } from '../models/sub-total';
import { Tax } from '../models/tax';
import { Vendor } from '../models/vendor';
import { PurchaseService } from '../service/purchase-service';
import $ from "jquery";
import { NgForm } from '@angular/forms';
import { VendorService } from '../service/vendor-service';
import { ProductService } from '../service/product-service';
import { Router } from '@angular/router';
import {Location} from "../models/location";
import { ExepnseStatusEnum, PrintDocumentType } from '../models/enum';
import { FileService } from '../service/file-service';
import { OrganisationInvoiceSettingService } from '../service/organisation-invoice-setting-service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Attachment } from '../models/attachment';
import { OrganisationParentComponent } from '../OrganisationParentComponent';
import { OrganisationService } from '../service/organisation-service';
import { MenuService } from '../service/menu-service';
import { CloseDateComponent } from '../modal/close-date-component';
import { PrintService } from '../service/print-service';
import { TranslateService } from '@ngx-translate/core';
import { Utility } from '../models/utility';
@Component({
    selector: 'new-purchase-component',
    templateUrl: './new-purchase-component.html',
    styleUrls: ['./new-purchase-component.css']
})
export class NewPurchaseComponent extends OrganisationParentComponent implements OnInit {
    purchaseOrder:PurchaseOrder=new PurchaseOrder();
    selectedVendor:Vendor=new Vendor();
    purchaseOrderLookup:PurchaseOrderLookup=new PurchaseOrderLookup();
    productDropdownConfig:DropdownConfig=new DropdownConfig();
    selectProduct:Product=null;
    subTotals:SubTotal[]=[];
    @ViewChild('f', { static: true }) form: NgForm;
    constructor(private router: Router,
        private purchaseService: PurchaseService,
        private vendorService:VendorService,
        private productService:ProductService,
        private modalService: NgbModal,
        organisationService:OrganisationService,
        private menuService:MenuService,
        private printService:PrintService,
        private translate: TranslateService,
        private organisationInvoiceSettingService: OrganisationInvoiceSettingService,
        private fileService:FileService) {
        super("New Purchase Order",organisationService);
        this.setPageTitleFromLocalise(this.translate,"newPurchase");
        this.ShowBackButton();
        this.dropdownConfig.displayKey="displayName";
        this.dropdownConfig.searchOnKey="displayName";
        this.dropdownConfig.placeholder=this.selectText();
        this.productDropdownConfig.placeholder=this.searchProductOrServiceText();
        this.productDropdownConfig.displayKey="name";
    }
    addLine(){
       
        if(this.selectProduct==null){
            this.showErrorText(Utility.selectProductToAddText());
            return;
        }
        let orderItem:PurchaseOrderItem=new PurchaseOrderItem();
        orderItem.productId=this.selectProduct.id;
        orderItem.product=this.selectProduct;
        orderItem.quantity=1;
        orderItem.taxId=this.selectProduct.productPurchaseInformation.taxId;
        orderItem.unitPrice=this.selectProduct.productPurchaseInformation.price;
        orderItem.locationId=this.selectProduct.locationId;
        if(this.purchaseOrder.currencyId!=this.purchaseOrderLookup.baseCurrencyId){
            this.purchaseOrder.currency.exchangeRates.forEach(it=>{
                if(it.currencyId==this.purchaseOrderLookup.baseCurrencyId){
                    orderItem.unitPrice=this.getTotalAmount(orderItem.product.productPurchaseInformation.price,it.exchangeRate)
                }
            })
        }
        orderItem.discountRate=null;
        if(orderItem.taxId!=null){
            this.purchaseOrderLookup.taxes.forEach(it=>{
                if(it.id==orderItem.taxId){
                    orderItem.tax=it;
                }
            })
        }
        this.purchaseOrder.expenseItems.push(orderItem);
        this.selectProduct=null;
        this.calculateTotal();
        console.log(this.purchaseOrder.expenseItems);
    }
    onRemvoePurchaseOrderItem(orderItem:PurchaseOrderItem){
        this.purchaseOrder.expenseItems = this.purchaseOrder.expenseItems.filter(obj => obj !== orderItem);
        this.calculateTotal();
    }
    onOrderItemChange(orderItem:PurchaseOrderItem){
        this.calculateTotal();
    }
    onTaxSelectChange(orderItem:PurchaseOrderItem){
        if(orderItem.taxId!=null){
            if(orderItem.taxId!=null){
                this.purchaseOrderLookup.taxes.forEach(it=>{
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
                self.purchaseOrder.attachments.push(attachment);
                self.hideProgressBar();
             },err=>{
                 self.handleError(err);
             })
          };
        }
      };
      onRemoveAttachment(url:Attachment){
        this.purchaseOrder.attachments = this.purchaseOrder.attachments.filter(obj => obj.fileUrl !== url.fileUrl);
      }
    calculateTotal(){
        this.purchaseOrder.total=0;
        this.purchaseOrder.totalIncludeTax=0;
        let taxSubTotals:SubTotal[]=[];
        this.purchaseOrder.expenseItems.forEach(it=>{
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
                    subTotal.currency=this.purchaseOrder.currency;
                    taxSubTotals.push(subTotal);
                  }
              })
              let taxAmount:number=(it.lineTotalIncludeTax*totalTaxRate)/100;
              it.lineTotalIncludeTax+=taxAmount/100;
            }
            this.purchaseOrder.total+=it.lineTotal;
            this.purchaseOrder.totalIncludeTax+=it.lineTotalIncludeTax;
        });
        this.subTotals=[];
        let subTotal:SubTotal=new SubTotal();
        subTotal.label=Utility.subTotalText()+": ";
        subTotal.amount=this.purchaseOrder.total;
        subTotal.currency=this.purchaseOrder.currency;
        this.subTotals.push(subTotal);
        taxSubTotals.forEach(it=>{
            this.subTotals.push(it);
        })
        subTotal=new SubTotal();
        subTotal.label=Utility.totalText()+": ";
        subTotal.amount=this.purchaseOrder.totalIncludeTax;
        subTotal.currency=this.purchaseOrder.currency;
        this.subTotals.push(subTotal);

        if(taxSubTotals.length>0 && this.purchaseOrder.currencyId!=this.purchaseOrderLookup.taxCurrency.id){
            let taxExchangeRate:number=0;
            this.purchaseOrder.currency.exchangeRates.forEach(it=>{
                if(it.currencyId==this.purchaseOrderLookup.taxCurrency.id){
                    taxExchangeRate=it.exchangeRate;
                }
            })
            subTotal=new SubTotal();
            subTotal.label=Utility.exchangeRateText()+": ";;
            subTotal.amount=taxExchangeRate;
            subTotal.currency=this.purchaseOrderLookup.taxCurrency;
            this.subTotals.push(subTotal);
            subTotal=new SubTotal();
            subTotal.label=Utility.totalInKHRText()+": ";;
            subTotal.amount=taxExchangeRate​*this.purchaseOrder.totalIncludeTax;
            subTotal.currency=this.purchaseOrderLookup.taxCurrency;
            this.subTotals.push(subTotal);
        }


    }
    onOfficalFileChange(attachment:Attachment){
        if(attachment.isFinalOfficalFile){
            this.purchaseOrder.attachments.forEach(it=>{
                if(it!=attachment){
                    it.isFinalOfficalFile=false;
                }
            })
        }
    }
    onExchangeRateChange(){
        this.updatePurchasePriceWhenCurrencyChange();
    }
    ngOnInit(): void {
        this.showProgressBar();
        this.purchaseService.getPurchaseLookupForCreateOrUpdate().subscribe(result=>{
            this.purchaseOrder.orderNumber=result.orderNumber;
            this.purchaseOrder.currencyId=result.baseCurrencyId;
            this.purchaseOrderLookup.currencies=result.currencies;
            this.purchaseOrderLookup.taxes=result.taxes;
            this.purchaseOrderLookup.taxCurrency=result.taxCurrency;
            this.purchaseOrderLookup.baseCurrencyId=result.baseCurrencyId;
            this.purchaseOrderLookup.locations=result.locations;
            this.purchaseOrderLookup.currencies.forEach(it=>{
                if(it.id==this.purchaseOrder.currencyId){
                    this.purchaseOrder.currency=it;
                }
            })
            let tax =new Tax();
            tax.id=null;
            tax.name=this.selectText();
            this.purchaseOrderLookup.taxes.splice(0, 0, tax);

            let location =new Location();
            location.id=null;
            location.name=this.selectText();
            this.purchaseOrderLookup.locations.splice(0, 0, location);
            this.hideProgressBar();
        },err=>{
            this.handleError(err);
        });
        this.vendorService.getAllVendors().subscribe(result=>{
            this.purchaseOrderLookup.vendors=result;
            this.selectedVendor=null;
        });
        this.getProducts();
        $('#delivery').find('.wc-date-container').children('span').hide();
        this.purchaseOrder.deliveryDate=null;
        this.getTermAndCondition();
        this.getPermission(this.menuService);
    }
    getTermAndCondition(){
        this.organisationInvoiceSettingService.get().subscribe(result=>{
            this.purchaseOrder.termAndCondition=result.termAndConditionForPurchaseOrder;
        });
    }
    getProducts(){
        this.productService.getProductsForPurchase().subscribe(result=>{
            this.purchaseOrderLookup.products=result;
        })
    }
    updatePurchasePriceWhenCurrencyChange(){
        this.purchaseOrder.expenseItems.forEach(it=>{
            
            if(this.purchaseOrder.currencyId==this.purchaseOrderLookup.baseCurrencyId){
                it.unitPrice=it.product.productPurchaseInformation.price;
            }
            else{
                this.purchaseOrder.currency.exchangeRates.forEach(rate=>{
                    if(rate.currencyId==this.purchaseOrderLookup.baseCurrencyId){
                        it.unitPrice=this.getTotalAmount(it.product.productPurchaseInformation.price,rate.exchangeRate)
                    }   
                })
            }
        });
        this.calculateTotal();
    }

    onSelectedVendor(event:any){
        this.purchaseOrder.vendorId=this.selectedVendor.id;
        if(this.selectedVendor.id!=0){
            this.purchaseOrderLookup.currencies.forEach(it=>{
                if(it.id==this.selectedVendor.currencyId){
                    this.purchaseOrder.currencyId=it.id;
                    this.purchaseOrder.currency=it;
                    this.updatePurchasePriceWhenCurrencyChange();
                }
            })
        }
    }
    onCurrencySelectedChange(){
       this.purchaseOrder.currencyId=this.purchaseOrder.currency.id;
       this.updatePurchasePriceWhenCurrencyChange();
    }
    onSaveAsDraw(isClose:boolean){
      
        this.purchaseOrder.expenseStatusId=ExepnseStatusEnum.Draft;
        this.onSave(isClose,false);
    }
    onSaveForApproval(isClose:boolean){
        this.purchaseOrder.expenseStatusId=ExepnseStatusEnum.WaitingForApproval;
        this.onSave(isClose,false);
    }
    onSaveApproval(isClose:boolean){
        let hasCloseDoc =this.purchaseOrder.attachments.filter(u=>u.isFinalOfficalFile).length>0;
        const modalRef = this.modalService.open(CloseDateComponent);
        modalRef.componentInstance.init(modalRef,"Purchase order: "+this.purchaseOrder.orderNumber,hasCloseDoc);
        modalRef .result.then((result) => {
           this.purchaseOrder.closeDate=new Date(result.closeDate);
           if(result.attachment.fileUrl.length>0){
               this.purchaseOrder.attachments.push(result.attachment);
           }
           this.purchaseOrder.expenseStatusId=ExepnseStatusEnum.Approved;
           this.onSave(isClose,false);
          });
    }
    onSave(isClose:boolean,isPrint:boolean){
        var errorTexts:string[]=[];
        if(this.purchaseOrder.vendorId==null || this.purchaseOrder.vendorId==0){
            errorTexts.push(Utility.vendorRequireText());
        }
        if(this.purchaseOrder.date==null){
            errorTexts.push(Utility.dateRequireText());
        }
        if(this.purchaseOrder.expenseItems.length==0){
            errorTexts.push(Utility.lineItemRequireText());
        }
        let lineNumber:number=1;
        
        this.purchaseOrder.expenseItems.forEach(it=>{
            if(it.quantity==null || it.quantity<=0){
                errorTexts.push(Utility.quantityGreaterThenText(lineNumber,0));
            }
            if(it.unitPrice==null || it.unitPrice<0){
                errorTexts.push(Utility.unitPriceGreaterThenText(lineNumber,-1));
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
        if(this.purchaseOrder.id==0){
            this.purchaseService.create(this.purchaseOrder).subscribe(result=>{
                this.hideProgressBar();
                if(isPrint){
                    this.purchaseOrder.id=result.id;
                    this.purchaseOrder.orderNumber=result.orderNumber;
                    this.onPrint();
                }
                else{
                    if(isClose){
                        this.router.navigate(['/order'])
                    }
                    else{
                        this.purchaseOrder=new PurchaseOrder();
                        this.purchaseOrder.currencyId=this.purchaseOrderLookup.baseCurrencyId;
                        this.purchaseOrderLookup.vendors=this.purchaseOrderLookup.vendors;
                        this.selectedVendor=null;
                        $('#delivery').find('.wc-date-container').children('span').hide();
                        this.getProducts();
                        this.calculateTotal();
                        this.purchaseOrderLookup.currencies.forEach(it=>{
                            if(this.purchaseOrder.currencyId==it.id){
                                this.purchaseOrder.currency=it;
                            }
                        });
                        this.purchaseService.createPurchaseOrderNumber().subscribe(result=>{
                            this.purchaseOrder.orderNumber=result.orderNumber;
                        });
                        this.purchaseOrder.deliveryDate=null;
                        this.getTermAndCondition();
                    }
                }
                
            },err=>{
                this.handleError(err);
            })
        }
        else{
            this.purchaseService.update(this.purchaseOrder).subscribe(result=>{
                this.hideProgressBar();
                if(isPrint){
                    this.onPrint();
                }
                else{
                    if(isClose){
                        this.router.navigate(['/order'])
                    }
                    else{
                        this.purchaseOrder=new PurchaseOrder();
                        this.purchaseOrder.currencyId=this.purchaseOrderLookup.baseCurrencyId;
                        this.purchaseOrderLookup.vendors=this.purchaseOrderLookup.vendors;
                        this.selectedVendor=null;
                        $('#delivery').find('.wc-date-container').children('span').hide();
                        this.getProducts();
                        this.calculateTotal();
                        this.purchaseOrderLookup.currencies.forEach(it=>{
                            if(this.purchaseOrder.currencyId==it.id){
                                this.purchaseOrder.currency=it;
                            }
                        });
                        this.purchaseService.createPurchaseOrderNumber().subscribe(result=>{
                            this.purchaseOrder.orderNumber=result.orderNumber;
                        });
                        this.purchaseOrder.deliveryDate=null;
                        this.getTermAndCondition();
                    }
                }
                
            },err=>{
                this.handleError(err);
            })
        }
        

    }
    onSavePrint(){
        this.purchaseOrder.expenseStatusId=ExepnseStatusEnum.Draft;
        this.onSave(false,true);
    }
    onPrint(){
        this.print(this.printService,this.purchaseOrder.id,PrintDocumentType.PurchaseOrder,this.purchaseOrder.orderNumber);
    }
    onDeliveryDateChange(event:any){
        $('#delivery').find('.wc-date-container').children('span').show();
    }
    showLocation(){
        return this.purchaseOrder.expenseItems.filter(u=>u.product.trackInventory).length>0;
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
   
}

