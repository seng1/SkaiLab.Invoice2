import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { DropdownConfig } from '../models/dropdown-config';
import { ExepnseStatusEnum, PrintDocumentType } from '../models/enum';
import { Expense } from '../models/expense';
import { Product } from '../models/product';
import { PurchaseOrderLookup } from '../models/purchase-order-lookup';
import { PurchaseOrderItem } from '../models/purchaseOrder';
import { SubTotal } from '../models/sub-total';
import { Vendor } from '../models/vendor';
import { BillService } from '../service/bill-service';
import { VendorService } from '../service/vendor-service';
import { ProductService } from '../service/product-service';
import { ExpenseService } from '../service/expense-service';
import { ViewEncapsulation } from '@angular/core';
import { Tax } from '../models/tax';
import { BillUtility, Utility } from '../models/utility';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Attachment } from '../models/attachment';
import { OrganisationParentComponent } from '../OrganisationParentComponent';
import { OrganisationService } from '../service/organisation-service';
import { MenuService } from '../service/menu-service';
import { CloseDateComponent } from '../modal/close-date-component';
import { PrintService } from '../service/print-service';
import { TranslateService } from '@ngx-translate/core';
@Component({
    selector: 'update-bill-component',
    templateUrl: './update-bill-component.html',
    encapsulation: ViewEncapsulation.None

})
export class UpdateBillComponent extends OrganisationParentComponent implements OnInit {
    @ViewChild('f', { static: true }) form: NgForm;
    bill: Expense = new Expense();
    selectedVendor: Vendor = new Vendor();
    billLookup: PurchaseOrderLookup = new PurchaseOrderLookup();
    productDropdownConfig: DropdownConfig = new DropdownConfig();
    selectProduct: Product = null;
    subTotals: SubTotal[] = [];
    drawStatusId: number = ExepnseStatusEnum.Draft;
    waitingForApprovalStatusId: number = ExepnseStatusEnum.WaitingForApproval;
    approveStatusId: number = ExepnseStatusEnum.Approved;
    deleteStatusId: number = ExepnseStatusEnum.Delete;
    billStatusId: number = ExepnseStatusEnum.Billed;
    oldStatusId: number = 0;
    constructor(private router: Router,
        private vendorService: VendorService,
        private route: ActivatedRoute,
        private billService: BillService,
        private productService: ProductService,
        private modalService: NgbModal,
        private expenseService: ExpenseService,
        private menuService:MenuService,
        private printService:PrintService,
        private translate: TranslateService,
        organisationService:OrganisationService
        ) {
        super("Update Bill",organisationService);
        this.ShowBackButton();
        this.setPageTitleFromLocalise(this.translate,"updateBill");
        this.router.events.subscribe((evt) => {
            if (evt instanceof NavigationEnd) {
                this.router.navigated = false;
                var param = this.route.snapshot.params;
                this.id = param.id.replace(":", "");
            }
        });
        this.dropdownConfig.displayKey = "displayName";
        this.dropdownConfig.searchOnKey = "displayName";
        this.dropdownConfig.placeholder = this.selectText();
        this.productDropdownConfig.placeholder = this.searchProductOrServiceText();
    }
    ngOnInit(): void {
        this.vendorService.getAllVendors().subscribe(result => {
            this.billLookup.vendors = result;
            if(this.bill.vendorId!=null && this.bill.vendorId!=0){
                this.billLookup.vendors.forEach(it=>{
                    if(it.id==this.bill.vendorId){
                        this.selectedVendor=it;

                    }
                });
            }
        });
        this.productService.getProductsForPurchase().subscribe(result => {
            this.billLookup.products = result;
          
            if (this.bill.expenseItems.length > 0) {
                if (this.billLookup.products.length > 0) {
                    this.bill.expenseItems.forEach(it => {
                        if (it.productId != null) {
                            this.billLookup.products.forEach(product => {
                                if (product.id == it.productId) {
                                    it.product = product;
                                }
                            })
                        }
                    })
                }
            }
            this.calculateTotal();
        })
        this.showProgressBar();
        this.billService.getExpenseForUpdate(this.id).subscribe(result => {
            this.bill = result.expense;
            this.billLookup.currencies = result.currencies;
            this.billLookup.taxCurrency = result.taxCurrency;
            this.billLookup.taxes = result.taxes;
            this.billLookup.baseCurrencyId = result.baseCurrencyId;
            this.billLookup.currencies.forEach(it => {
                if (it.id == this.bill.currencyId) {
                    this.bill.currency = it;
                }
                if (this.billLookup.products.length > 0) {
                    this.bill.expenseItems.forEach(it => {
                        if (it.productId != null) {
                            this.billLookup.products.forEach(product => {
                                if (product.id == it.productId) {
                                    it.product = product;
                                }
                            })
                        }
                    })
                }


            })
            let tax = new Tax();
            tax.id = null;
            tax.name = this.selectText();
            this.billLookup.taxes.splice(0, 0, tax);
            this.calculateTotal();
            if(this.billLookup.vendors.length>0){
                this.billLookup.vendors.forEach(it=>{
                    if(it.id==this.bill.vendorId){
                        this.selectedVendor=it;

                    }
                });
            }
            this.hideProgressBar();
        }, err => {
            this.handleError(err);
        });
        this.getPermission(this.menuService);
    }
    fileChange(event: any) {
        if (event) {
            let self = this;
            var reader = new FileReader();
            reader.readAsDataURL(event.target.files[0]);
            reader.onload = function () {
                self.showProgressBar();
                let fileName=event.target.files[0].name;
                self.expenseService.addAttachment(self.id, reader.result.toString(),fileName).subscribe(result => {
                    let attachment:Attachment=new Attachment();
                    attachment.fileUrl=result.url;
                    attachment.fileName=fileName;
                    self.bill.attachments.push(attachment);
                    self.hideProgressBar();
                }, err => {
                    self.handleError(err);
                });
            };
        }
    }
    isDisable(): boolean {
        return this.bill.expenseStatusId == this.approveStatusId || this.bill.expenseStatusId == ExepnseStatusEnum.Billed || this.bill.expenseStatusId == ExepnseStatusEnum.Delete;
    }
    onRemoveAttachment(url: string) {
        this.showProgressBar();
        this.expenseService.removeAttachment(this.id, url).subscribe(result => {
            this.bill.attachments = this.bill.attachments.filter(obj => obj.fileUrl !== url);
            this.hideProgressBar();
        }, err => {
            this.handleError(err);
        })

    }
    onOrderItemChange(orderItem: PurchaseOrderItem) {
        this.calculateTotal();
    }
    onTaxSelectChange(orderItem: PurchaseOrderItem) {
        if (orderItem.taxId != null) {
            if (orderItem.taxId != null) {
                this.billLookup.taxes.forEach(it => {
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
    onExchangeRateChange() {
        this.updatePurchasePriceWhenCurrencyChange();
    }
    updatePurchasePriceWhenCurrencyChange() {
        this.bill.expenseItems.forEach(it => {
            if (it.product != null && it.product.id != undefined) {
                if (this.bill.currencyId == this.billLookup.baseCurrencyId) {
                    it.unitPrice = it.product.productPurchaseInformation.price;
                }
                else {
                    this.bill.currency.exchangeRates.forEach(rate => {
                        if (rate.currencyId == this.billLookup.baseCurrencyId) {
                            it.unitPrice = this.getTotalAmount(it.product.productPurchaseInformation.price, rate.exchangeRate)
                        }
                    })
                }
            }

        });
        this.calculateTotal();
    }
    onRemvoePurchaseOrderItem(orderItem: PurchaseOrderItem) {
        this.bill.expenseItems = this.bill.expenseItems.filter(obj => obj !== orderItem);
        this.calculateTotal();
    }
    onSelectedVendor(event: any) {
        this.bill.vendorId = this.selectedVendor.id;
        if (this.selectedVendor.id != 0) {
            this.billLookup.currencies.forEach(it => {
                if (it.id == this.selectedVendor.currencyId) {
                    this.bill.currencyId = it.id;
                    this.bill.currency = it;
                    this.updatePurchasePriceWhenCurrencyChange();
                }
            })
        }
    }
    onCurrencySelectedChange() {
        this.bill.currencyId = this.bill.currency.id;
        this.updatePurchasePriceWhenCurrencyChange();
    }
    calculateTotal() {
        this.bill.total = 0;
        this.bill.totalIncludeTax = 0;
        let taxSubTotals: SubTotal[] = [];
        this.bill.expenseItems.forEach(it => {
            it.lineTotal = it.quantity * it.unitPrice;
            if (it.discountRate != null) {
                it.lineTotal -= (it.lineTotal * it.discountRate) / 100;
            }
            it.lineTotalIncludeTax = it.lineTotal;
            if (it.tax != null) {
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
                        subTotal.currency = this.bill.currency;
                        taxSubTotals.push(subTotal);
                    }
                })
                let taxAmount: number = (it.lineTotalIncludeTax * totalTaxRate) / 100;
                it.lineTotalIncludeTax += taxAmount;
            }
            this.bill.total += it.lineTotal;
            this.bill.totalIncludeTax += it.lineTotalIncludeTax;
        });
        this.subTotals = [];
        let subTotal: SubTotal = new SubTotal();
        subTotal.label = Utility.subTotalText()+": ";
        subTotal.amount = this.bill.total;
        subTotal.currency = this.bill.currency;
        this.subTotals.push(subTotal);
        taxSubTotals.forEach(it => {
            this.subTotals.push(it);
        })
        subTotal = new SubTotal();
        subTotal.label = Utility.totalText()+": ";
        subTotal.amount = this.bill.totalIncludeTax;
        subTotal.currency = this.bill.currency;
        this.subTotals.push(subTotal);

        if (taxSubTotals.length > 0 && this.bill.currencyId != this.billLookup.taxCurrency.id) {
            let taxExchangeRate: number = 0;
            this.bill.currency.exchangeRates.forEach(it => {
                if (it.currencyId == this.billLookup.taxCurrency.id) {
                    taxExchangeRate = it.exchangeRate;
                }
            })
            subTotal = new SubTotal();
            subTotal.label =Utility.exchangeRateText()+": ";
            subTotal.amount = taxExchangeRate;
            subTotal.currency = this.billLookup.taxCurrency;
            this.subTotals.push(subTotal);
            subTotal = new SubTotal();
            subTotal.label = Utility.totalInKHRText()+": ";
            subTotal.amount = taxExchangeRate * this.bill.totalIncludeTax;
            subTotal.currency = this.billLookup.taxCurrency;
            this.subTotals.push(subTotal);
        }


    }

    onSaveAsDraw(isClose: boolean) {
        this.oldStatusId = this.bill.expenseStatusId;
        this.bill.expenseStatusId = ExepnseStatusEnum.Draft;
        this.onSave(isClose, false);
    }
    onSaveForApproval(isClose: boolean) {
        this.oldStatusId = this.bill.expenseStatusId;
        this.bill.expenseStatusId = ExepnseStatusEnum.WaitingForApproval;
        this.onSave(isClose, false);
    }
    onSaveApproval(isClose: boolean) {
        let hasCloseDoc =this.bill.attachments.filter(u=>u.isFinalOfficalFile).length>0;
        const modalRef = this.modalService.open(CloseDateComponent);
        modalRef.componentInstance.init(modalRef,Utility.approveBillText()+": "+this.bill.orderNumber,hasCloseDoc);
        modalRef .result.then((result) => {
           this.bill.closeDate=new Date(result.closeDate);
           if(result.attachment.fileUrl.length>0){
               this.bill.attachments.push(result.attachment);
           }
           this.oldStatusId=this.bill.expenseStatusId;
           this.bill.expenseStatusId=ExepnseStatusEnum.Approved;
           this.onSave(isClose,false);
          });
    }
    onSave(isClose: boolean, isPrint: boolean) {
        var errorTexts: string[] = [];
        if (this.bill.vendorId == null || this.bill.vendorId == 0) {
            errorTexts.push(Utility.vendorRequireText());
        }
        if (this.bill.date == null) {
            errorTexts.push(Utility.dateRequireText());
        }
        if (this.bill.expenseItems.length == 0) {
            errorTexts.push(Utility.lineItemRequireText());
        }
        let lineNumber: number = 1;

        this.bill.expenseItems.forEach(it => {
            if (it.quantity == null || it.quantity <= 0) {
                errorTexts.push(Utility.quantityGreaterThenText(lineNumber,0));
            }
            if (it.unitPrice == null || it.unitPrice < 0) {
                errorTexts.push(Utility.unitPriceGreaterThenText(lineNumber,-1));
            }
            if (it.discountRate != null && it.discountRate < 0) {
                errorTexts.push(Utility.discountRateMustBetween(lineNumber,0,100));
            }
            if (it.description.length == 0) {
                errorTexts.push(Utility.descriptionRequire());
            }
            lineNumber++;
        });
        if (errorTexts.length > 0) {
            this.showErrorTexts(errorTexts);
            if(this.oldStatusId!=0){
                this.bill.expenseStatusId=this.oldStatusId;
            }
            return;
        }
        this.bill.expenseItems.forEach(it => {
            if (it.product == null || it.product.id == undefined) {
                it.product = null;
            }
        })
        this.showProgressBar();
        this.billService.update(this.bill).subscribe(result => {
            this.hideProgressBar();
            if (isPrint) {
                this.onPrint();
            }
            else {
                if (isClose) {
                    this.router.navigate(['/vendor-bill'])
                }
                else {
                    this.router.navigate(['/vendor-bill-new'])
                }
            }

        }, err => {
            this.bill.expenseStatusId = this.oldStatusId;
            this.handleError(err);
        })


    }
    addLine() {
        if(this.selectProduct==null || this.selectProduct.id==undefined){
            this.showErrorText(Utility.selectProductToAddText());
            return;
        }

        let expenseItem: PurchaseOrderItem = new PurchaseOrderItem();

        expenseItem.quantity = 1;
        expenseItem.unitPrice = this.selectProduct.productPurchaseInformation.price;
        expenseItem.discountRate = null;
        expenseItem.locationId = null;
        expenseItem.productId = this.selectProduct.id;
        expenseItem.taxId = this.selectProduct.productPurchaseInformation.taxId;
        expenseItem.lineTotal =expenseItem.unitPrice * expenseItem.quantity;
        expenseItem.lineTotalIncludeTax = 0;
        expenseItem.product = this.selectProduct;
        expenseItem.description=this.selectProduct.productPurchaseInformation.title;
        if(this.selectProduct.productPurchaseInformation.description!=null && this.selectProduct.productPurchaseInformation.description.length>0){
            expenseItem.description+=" "+this.selectProduct.productPurchaseInformation.description;
        }
        if (this.bill.currencyId != this.billLookup.baseCurrencyId) {
            this.bill.currency.exchangeRates.forEach(it => {
                if (it.currencyId == this.billLookup.baseCurrencyId) {
                    expenseItem.unitPrice = this.getTotalAmount(this.selectProduct.productPurchaseInformation.price, it.exchangeRate)
                }
            })
        }
        if (this.selectProduct.productPurchaseInformation.taxId != null) {
            this.billLookup.taxes.forEach(it => {
                if (it.id == this.selectProduct.productPurchaseInformation.taxId) {
                    expenseItem.taxId = it.id;
                    expenseItem.tax = it;
                }
            })
        }
        this.bill.expenseItems.push(expenseItem);
        this.updatePurchasePriceWhenCurrencyChange();
        this.selectProduct=null;
    }
    addLineWithoutProduct(){
        let expenseItem: PurchaseOrderItem = new PurchaseOrderItem();
        expenseItem.quantity = 1;
        expenseItem.unitPrice = 0;
        expenseItem.discountRate = null;
        expenseItem.locationId = null;
        expenseItem.productId =null;
        expenseItem.taxId =null;
        expenseItem.lineTotal =0;
        expenseItem.lineTotalIncludeTax = 0;
        expenseItem.product = null;
        expenseItem.description="";
        this.bill.expenseItems.push(expenseItem);
        this.updatePurchasePriceWhenCurrencyChange();
    }
    onMarkAsBill() {
        this.bill.expenseItems.forEach(it => {
            if (it.product == null || it.product.id == undefined) {
                it.product = null;
            }
        })
        this.showProgressBar();
        this.billService.markAsBill(this.bill).subscribe(result => {
            this.hideProgressBar();
            this.bill.expenseStatusId = ExepnseStatusEnum.Billed;
        }, err => {
            this.handleError(err);
        })

    }
    onMarkAsDelete() {
        if (confirm(Utility.deleteQ(this.bill.orderNumber))) {
            this.bill.expenseItems.forEach(it => {
                if (it.product == null || it.product.id == undefined) {
                    it.product = null;
                }
            })
            this.showProgressBar();
            this.billService.markDelete(this.bill).subscribe(result => {
                this.hideProgressBar();
                this.bill.expenseStatusId = ExepnseStatusEnum.Delete;
                this.router.navigate(['/vendor-bill']);
            }, err => {
                this.handleError(err);
            })
        }


    }
    allowSaveDraw() {
        return BillUtility.allowSaveDraw(this.bill.expenseStatusId);
    }
    allowSubmitForApproval() {
        return BillUtility.allowSubmitForApproval(this.bill.expenseStatusId) && this.permission.approvaPayPurchaseSale;
    }
    allowApprove() {
        return BillUtility.allowApprove(this.bill.expenseStatusId)&& this.permission.approvaPayPurchaseSale;
    }
    allowDelete() {
        return BillUtility.allowDelete(this.bill.expenseStatusId)&& this.permission.approvaPayPurchaseSale;
    }
    allowBill() {
        return BillUtility.allowBill(this.bill.expenseStatusId)&& this.permission.approvaPayPurchaseSale;
    }
    allowSaveAndPrint() {
        return BillUtility.allowSaveAndPrint(this.bill.expenseStatusId) && this.permission.readWritePurchaseSale;
    }
    onPrint() {
        this.print(this.printService,this.bill.id,PrintDocumentType.Bill,this.bill.orderNumber);
    }
    onSaveAndPrint() {
        this.onSave(false, true);
    }
    showLocation() {
        return this.bill.expenseItems.filter(u => u.product.trackInventory).length > 0;
    }
    onOfficalFileChange(attachment:Attachment){
        if(attachment.isFinalOfficalFile){
            this.bill.attachments.forEach(it=>{
                if(it!=attachment){
                    it.isFinalOfficalFile=false;
                }
            })
            this.showProgressBar();
            this.expenseService.changeOfficialDocument(this.bill.id,attachment.fileUrl).subscribe(result=>{
                this.hideProgressBar();
            },err=>{
                this.handleError(err);
            });
        }
    }
    getSubTotalColSpan(){
        if(this.declareTax){
            return 4;
        }
        return 3;
    }
}

