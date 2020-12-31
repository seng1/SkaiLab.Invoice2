import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Currency } from '../models/currency';
import { DateTypeFilter } from '../models/date-type-filter';
import { BillDateTypeFilterEnum, ExepnseStatusEnum, PrintDocumentType } from '../models/enum';
import { Expense, ExpenseStatus } from '../models/expense';
import { PurchaseFilter } from '../models/filter';
import { Utility } from '../models/utility';
import { Vendor } from '../models/vendor';
import { ParentComponent } from '../parentComponent';
import { BillService } from '../service/bill-service';
import { OrganisationService } from '../service/organisation-service';
import { PagerService } from '../service/page-service';
import { VendorService } from '../service/vendor-service';
import $ from "jquery";
import { MenuService } from '../service/menu-service';
import { CloseDateComponent } from '../modal/close-date-component';
import { PrintService } from '../service/print-service';
import { TranslateService } from '@ngx-translate/core';

@Component({
    selector: 'vendor-bill-component',
    templateUrl: './vendor-bill-component.html'
})
export class VendorBillComponent extends ParentComponent implements OnInit {
    filter: PurchaseFilter = new PurchaseFilter();
    bills: Expense[] = [];
    vendors:Vendor[]=[];
    purchaseOrderStatuses:ExpenseStatus[]=[];
    selectedVendor:Vendor=new Vendor();
    drawStatusId:number=ExepnseStatusEnum.Draft;
    waitingForApprovalStatusId:number=ExepnseStatusEnum.WaitingForApproval;
    approveStatusId:number=ExepnseStatusEnum.Approved;
    baseCurrency:Currency=new Currency();
    selected:boolean=false;
    dateTypes:DateTypeFilter[]=[];
    ngOnInit(): void {
        this.vendorService.getAllVendors().subscribe(result=>{
            this.vendors=result;
            let c =new Vendor();
            c.id=0;
            c.displayName=this.allText();
            this.vendors.splice(0, 0, c);
            this.selectedVendor=c;
        });
        this.getStatues();
        this.getBills();
        this.getTotalPages();
        this.organisationService.getBaseCurrency().subscribe(result=>{
            this.baseCurrency=result;
        });
        this.getPermission(this.menuService);
    }
    getStatues(){
        this.billService.getExpenseStatuses(this.filter).subscribe(result=>{
            this.purchaseOrderStatuses=result;
            console.log(result);
        });
    }
    constructor(private billService:BillService
        ,private pagerService: PagerService,
        private modalService: NgbModal,
        private route: ActivatedRoute,
        private menuService:MenuService,
        private printService:PrintService,
        private organisationService:OrganisationService,
        private translate: TranslateService,
        private vendorService:VendorService){
        super("Bills");
        this.setPageTitleFromLocalise(this.translate,"bills");
        this.ShowBackButton();
        this.dropdownConfig.displayKey="displayName";
        this.dropdownConfig.searchOnKey="displayName";
        this.dropdownConfig.placeholder=this.allText();
        this.dateTypes=Utility.getBillDateTypeFilter();
        this.filter.dateTypeFilter=this.dateTypes[0];
        this.route.queryParamMap.subscribe((parm)=>{
            var statusId=parm.get("statusId");
            if(statusId!=null){
                if(Number(statusId)-ExepnseStatusEnum.OverDue==0){
                    this.filter.purchaseOrderStatusId=ExepnseStatusEnum.Approved;
                    this.filter.toDate=new Date();
                    this.filter.dateTypeFilter=this.dateTypes.filter(u=>u.id==BillDateTypeFilterEnum.DueDate)[0];
                    this.filter.fromDate=null;
                    $('#start').find('.wc-date-container').children('span').hide();
                }
                else{
                    this.filter.purchaseOrderStatusId=parseInt(statusId);
                }
                this.onSearch();
            }
       })
    }
    onStartChange(event:any){
        $('#start').find('.wc-date-container').children('span').show();
    }
    getBills() {
        this.showProgressBar();
        this.billService.gets(this.filter).subscribe(result => {
            this.bills = result;
            if(this.selected){
                this.bills.forEach(it=>{
                    it.selected=this.selected;
                })
            }
            this.hideProgressBar();
        }, err => {
            this.handleError(err);
        })
    }
    getTotalPages() {
        this.billService.getTotalPages(this.filter).subscribe(result => {
            this.filter.totalRow=result.totalRow;
            this.filter.totalPage=result.totalPage;
            this.setPage(1);
        })
    }
    pageClick(page:number){
        this.setPage(page);
        this.filter.page=page;
        this.getBills();
    }
    setPage(page: number) {
        this.pager = this.pagerService.getPager(this.filter.totalRow, page, this.filter.pageSize);
    }
    onSearch() {
        this.filter.page = 1;
        this.getBills();
        this.getTotalPages();
    }
    onAllSelectedChange(){
        this.bills.forEach(it=>{
            it.selected=this.selected;
        })
    }
    onSelectedVendor(event:any){
        this.filter.vendorId=this.selectedVendor.id;
        this.onSearch();
    }
    onTabStatusClick(status:ExpenseStatus){
        this.filter.purchaseOrderStatusId=status.id;
        this.onSearch();
    }
    onMarkAsWaitingForApproval(){
        var orderIds:number[]=[];
        this.bills.forEach(it=>{
            if(it.selected){
                orderIds.push(it.id);
            }
            
        });
        if(orderIds.length==0){
            this.showErrorText(Utility.noBillSelectedText());
            return;
        }
        this.showProgressBar();
        this.billService.markBillsAsWaitingForApproval(orderIds).subscribe(result=>{
            this.hideProgressBar();
            this.getBills();
            this.getStatues();
        },error=>{
            this.handleError(error);
        });
    }
    markPurchaseOrdersAsApprove(){
        let exepsens=this.bills.filter(u=>u.selected);
        if(exepsens.length==0){
            this.showErrorText(Utility.noBillSelectedText());
            return;
        }
        const modalRef = this.modalService.open(CloseDateComponent);
        modalRef.componentInstance.initExpense(modalRef,Utility.approveBillText(),exepsens);
        modalRef.result.then((result) => {
            this.showProgressBar();
            this.billService.markBillsOrdersAsApprove(exepsens).subscribe(result=>{
                this.hideProgressBar();
                this.getBills();
                this.getStatues();
            },err=>{
                this.handleError(err);
            })
          });
    }
    markPurchaseOrdersAsBill(){
        var orderIds:number[]=[];
        this.bills.forEach(it=>{
            if(it.selected){
                orderIds.push(it.id);
            }
            
        });
        if(orderIds.length==0){
            this.showErrorText(Utility.noBillSelectedText());
            return;
        }
        if(confirm(Utility.moveAllBillToBillQ())) {
            this.showProgressBar();
            this.billService.markBillsAsBill(orderIds).subscribe(result=>{
                this.hideProgressBar();
                this.getBills();
                this.getStatues();
            },error=>{
                this.handleError(error);
            });
        }
    }
    markPurchaseOrdersAsDelete(){
        var orderIds:number[]=[];
        this.bills.forEach(it=>{
            if(it.selected){
                orderIds.push(it.id);
            }
            
        });
        if(orderIds.length==0){
            this.showErrorText(Utility.noBillSelectedText());
            return;
        }
        if(confirm(Utility.deleteAllBillQ())) {
            this.showProgressBar();
            this.billService.markBillsAsDelete(orderIds).subscribe(result=>{
                this.hideProgressBar();
                this.getBills();
                this.getStatues();
            },error=>{
                this.handleError(error);
            });
        }
    }
    showPrintButton(){
        return this.bills.filter(u=>u.selected).length==1;
    }
    onPrint(){
        var quote=this.bills.filter(u=>u.selected)[0];
        this.print(this.printService,quote.id,PrintDocumentType.Bill,quote.orderNumber);
    }
    showWaitingForApprovalButton(){
        return this.filter.purchaseOrderStatusId==ExepnseStatusEnum.Draft && this.bills.filter(u=>u.selected).length>0 && this.permission.approvaPayPurchaseSale;
    }
    showApprovalButton(){
        return (this.filter.purchaseOrderStatusId==ExepnseStatusEnum.WaitingForApproval || this.filter.purchaseOrderStatusId==ExepnseStatusEnum.Draft) && this.bills.filter(u=>u.selected).length>0&& this.permission.approvaPayPurchaseSale;
    }
    showBillButton(){
        return this.filter.purchaseOrderStatusId==ExepnseStatusEnum.Approved && this.bills.filter(u=>u.selected).length>0&& this.permission.approvaPayPurchaseSale;
    }
    showDeleteButton(){
        return (this.filter.purchaseOrderStatusId==ExepnseStatusEnum.WaitingForApproval || this.filter.purchaseOrderStatusId==ExepnseStatusEnum.Draft) && this.bills.filter(u=>u.selected).length>0&& this.permission.approvaPayPurchaseSale;
    }
    getTotalSelectedItems(){
        return this.bills.filter(u=>u.selected).length
    }
    getTotalSelectedAmount(){
        var totalTotal:number=0;
        this.bills.forEach(it=>{
            if(it.selected){
                totalTotal+=it.totalIncludeTax*it.baseCurrencyExchangeRate
            }
        })
        return totalTotal;
    }
    
}

