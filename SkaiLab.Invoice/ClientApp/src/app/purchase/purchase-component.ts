import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { TranslateService } from '@ngx-translate/core';
import { CloseDateComponent } from '../modal/close-date-component';
import { Currency } from '../models/currency';
import { DateTypeFilter } from '../models/date-type-filter';
import { ExepnseStatusEnum, PrintDocumentType } from '../models/enum';
import { ExpenseStatus } from '../models/expense';
import { PurchaseFilter } from '../models/filter';
import { PurchaseOrder } from '../models/purchaseOrder';
import { Utility } from '../models/utility';
import { Vendor } from '../models/vendor';
import { ParentComponent } from '../parentComponent';
import { MenuService } from '../service/menu-service';
import { OrganisationService } from '../service/organisation-service';
import { PagerService } from '../service/page-service';
import { PrintService } from '../service/print-service';
import { PurchaseService } from '../service/purchase-service';
import { VendorService } from '../service/vendor-service';

@Component({
    selector: 'purchase-component',
    templateUrl: './purchase-component.html'
})
export class PurchaseComponent extends ParentComponent implements OnInit {
    filter: PurchaseFilter = new PurchaseFilter();
    purchaseOrders: PurchaseOrder[] = [];
    vendors:Vendor[]=[];
    purchaseOrderStatuses:ExpenseStatus[]=[];
    selectedVendor:Vendor=new Vendor();
    drawStatusId:number=ExepnseStatusEnum.Draft;
    waitingForApprovalStatusId:number=ExepnseStatusEnum.WaitingForApproval;
    approveStatusId:number=ExepnseStatusEnum.Approved;
    selected:boolean=false;
    dateTypes:DateTypeFilter[]=[];
    baseCurrency:Currency=new Currency();
   
    constructor(private purchaseService: PurchaseService
        ,private pagerService: PagerService
        ,private route: ActivatedRoute
        ,private modalService: NgbModal,
        private menuService:MenuService,
        private printService:PrintService,
        private translate: TranslateService,
        private organisationService:OrganisationService,
        private vendorService:VendorService) {
        super("Purchase Orders");
        this.setPageTitleFromLocalise(this.translate,"purchaseorders");
        this.ShowBackButton();
       this.dropdownConfig.displayKey="displayName";
       this.dropdownConfig.searchOnKey="displayName";
       this.dropdownConfig.placeholder=this.allText();
       this.dateTypes=Utility.getPurchaseDateTypeFilter();
       this.filter.dateTypeFilter=this.dateTypes[0];
       this.route.queryParamMap.subscribe((parm)=>{
            var statusId=parm.get("statusId");
            if(statusId!=null){
                this.filter.purchaseOrderStatusId=parseInt(statusId);
                this.onSearch();
            }
       })
       
    }
    ngOnInit(): void {
        this.getPurchaseOrders();
        this.getTotalPages();
        this.vendorService.getAllVendors().subscribe(result=>{
            this.vendors=result;
            let c =new Vendor();
            c.id=0;
            c.displayName=this.allText();
            this.vendors.splice(0, 0, c);
            this.selectedVendor=c;
        });
        this.organisationService.getBaseCurrency().subscribe(result=>{
            this.baseCurrency=result;
        })
       this.getStatues();
       this.getPermission(this.menuService);
    }
    getStatues(){
        this.purchaseService.getExpenseStatuses(this.filter).subscribe(result=>{
            this.purchaseOrderStatuses=result;
        });
    }
    onSelectedVendor(event:any){
        this.filter.vendorId=this.selectedVendor.id;
        this.onSearch();
    }
    onTabStatusClick(status:ExpenseStatus){
        this.filter.purchaseOrderStatusId=status.id;
        this.onSearch();
    }
    onSearch() {
        this.filter.page = 1;
        this.getPurchaseOrders();
        this.getTotalPages();
    }
    onAllSelectedChange(){
        this.purchaseOrders.forEach(it=>{
            it.selected=this.selected;
        })
    }
    getPurchaseOrders() {
        this.showProgressBar();
        this.purchaseService.gets(this.filter).subscribe(result => {
            this.purchaseOrders = result;
            if(this.selected){
                this.purchaseOrders.forEach(it=>{
                    it.selected=this.selected;
                })
            }
            this.hideProgressBar();
        }, err => {
            this.handleError(err);
        })
    }
    getTotalPages() {
        this.purchaseService.getTotalPages(this.filter).subscribe(result => {
            this.filter.totalRow=result.totalRow;
            this.filter.totalPage=result.totalPage;
            this.setPage(1);
        })
    }
    pageClick(page:number){
        this.setPage(page);
        this.filter.page=page;
        this.getPurchaseOrders();
    }
    setPage(page: number) {
        this.pager = this.pagerService.getPager(this.filter.totalRow, page, this.filter.pageSize);
    }
    onMarkAsWaitingForApproval(){
        var orderIds:number[]=[];
        this.purchaseOrders.forEach(it=>{
            if(it.selected){
                orderIds.push(it.id);
            }
            
        });
        if(orderIds.length==0){
            this.showErrorText(Utility.noPurchaseSelectedText());
            return;
        }
        this.purchaseService.markPurchaseOrdersAsWaitingForApproval(orderIds).subscribe(result=>{
            this.hideProgressBar();
            this.getPurchaseOrders();
            this.getStatues();
        },error=>{
            this.handleError(error);
        });
    }
    markPurchaseOrdersAsApprove(){
        let exepsens=this.purchaseOrders.filter(u=>u.selected);
        if(exepsens.length==0){
            this.showErrorText(Utility.noPurchaseSelectedText());
            return;
        }
        const modalRef = this.modalService.open(CloseDateComponent);
        modalRef.componentInstance.initExpense(modalRef,Utility.appovePurchaseText(),exepsens);
        modalRef.result.then((result) => {
            this.showProgressBar();
            this.purchaseService.markPurchaseOrdersAsApprove(exepsens).subscribe(result=>{
                this.hideProgressBar();
                this.getPurchaseOrders();
                this.getStatues();
            },err=>{
                this.handleError(err);
            })
          });
    }
    markPurchaseOrdersAsBill(){
        var orderIds:number[]=[];
        this.purchaseOrders.forEach(it=>{
            if(it.selected){
                orderIds.push(it.id);
            }
            
        });
        if(orderIds.length==0){
            this.showErrorText(Utility.noPurchaseSelectedText());
            return;
        }
        if(confirm(Utility.movePurchasesToBillText())) {
            this.showProgressBar();
            this.purchaseService.markPurchaseOrdersAsBill(orderIds).subscribe(result=>{
                this.hideProgressBar();
                this.getPurchaseOrders();
                this.getStatues();
            },error=>{
                this.handleError(error);
            });
        }
    }
    markPurchaseOrdersAsDelete(){
        var orderIds:number[]=[];
        this.purchaseOrders.forEach(it=>{
            if(it.selected){
                orderIds.push(it.id);
            }
            
        });
        if(orderIds.length==0){
            this.showErrorText(Utility.noPurchaseSelectedText());
            return;
        }
        if(confirm(Utility.deleteAllPurchaseText())) {
            this.showProgressBar();
            this.purchaseService.markPurchaseOrdersAsDelete(orderIds).subscribe(result=>{
                this.hideProgressBar();
                this.getPurchaseOrders();
                this.getStatues();
            },error=>{
                this.handleError(error);
            });
        }
    }
    showPrintButton(){
        return this.purchaseOrders.filter(u=>u.selected).length==1;
    }
    onPrint(){
        var quote=this.purchaseOrders.filter(u=>u.selected)[0];
        this.print(this.printService,quote.id,PrintDocumentType.PurchaseOrder,quote.orderNumber);
    }
    showWaitingForApprovalButton(){
        return this.filter.purchaseOrderStatusId==ExepnseStatusEnum.Draft && this.purchaseOrders.filter(u=>u.selected).length>0 && this.permission.approvaPayPurchaseSale;
    }
    showApprovalButton(){
        return (this.filter.purchaseOrderStatusId==ExepnseStatusEnum.WaitingForApproval || this.filter.purchaseOrderStatusId==ExepnseStatusEnum.Draft) && this.purchaseOrders.filter(u=>u.selected).length>0&& this.permission.approvaPayPurchaseSale;
    }
    showBillButton(){
        return this.filter.purchaseOrderStatusId==ExepnseStatusEnum.Approved && this.purchaseOrders.filter(u=>u.selected).length>0 && this.permission.approvaPayPurchaseSale;
    }
    showDeleteButton(){
        return (this.filter.purchaseOrderStatusId==ExepnseStatusEnum.WaitingForApproval || this.filter.purchaseOrderStatusId==ExepnseStatusEnum.Draft) && this.purchaseOrders.filter(u=>u.selected).length>0&& this.permission.approvaPayPurchaseSale;
    }
    getTotalSelectedItems(){
        return this.purchaseOrders.filter(u=>u.selected).length
    }
    getTotalSelectedAmount(){
        var totalTotal:number=0;
        this.purchaseOrders.forEach(it=>{
            if(it.selected){
                totalTotal+=it.totalIncludeTax*it.baseCurrencyExchangeRate
            }
        })
        return totalTotal;
    }
}

