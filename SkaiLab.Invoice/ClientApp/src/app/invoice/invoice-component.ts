
import { THIS_EXPR } from '@angular/compiler/src/output/output_ast';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { TranslateService } from '@ngx-translate/core';
import { Currency } from '../models/currency';
import { Customer } from '../models/customer';
import { Invoice, InvoiceStatus } from '../models/customer-transaction';
import { DateTypeFilter } from '../models/date-type-filter';
import { InvoiceDateTypeFilterEnum, InvoiceStatusEnum, PrintDocumentType } from '../models/enum';
import { InvoiceFilter } from '../models/filter';
import { Utility } from '../models/utility';
import { ParentComponent } from '../parentComponent';
import { PrintComponent } from '../print/print-component';
import { PrintReceiptComponent } from '../print/print-receipt-component';
import { CustomerService } from '../service/customer-service';
import { InvoiceService } from '../service/invoice-service';
import { MenuService } from '../service/menu-service';
import { OrganisationService } from '../service/organisation-service';
import { PagerService } from '../service/page-service';
import { PrintService } from '../service/print-service';
@Component({
    selector: 'invoice-component',
    templateUrl: './invoice-component.html'
})
export class InvoiceComponent extends ParentComponent implements OnInit {
    invoices:Invoice[]=[];
    statuses:InvoiceStatus[]=[];
    filter:InvoiceFilter=new InvoiceFilter();
    customers:Customer[]=[];
    selectCustomer:Customer=null;
    baseCurrency:Currency=new Currency();
    selected:boolean=false;
    dateTypes:DateTypeFilter[]=[];
    constructor(private invoiceService: InvoiceService,
        private customerService:CustomerService,
        private pagerService:PagerService,
        private route: ActivatedRoute,
        private modalService: NgbModal,
        private menuService:MenuService,
        private printService:PrintService,
        private translate: TranslateService,
        private organisationService:OrganisationService) {
        super("Invoices");
        this.setPageTitleFromLocalise(this.translate,"invoices");
        this.ShowBackButton();
       this.dropdownConfig.displayKey="displayName";
       this.dropdownConfig.searchOnKey="displayName";
       this.dropdownConfig.placeholder=this.allText();
       this.dateTypes=Utility.getInvoiceDateTypeFilter();
       this.filter.dateTypeFilter=this.dateTypes[0];
       this.route.queryParamMap.subscribe((parm)=>{
        var statusId=parm.get("statusId");
        if(statusId!=null){
            if(parseInt(statusId)-InvoiceStatusEnum.OverDue==0){
                this.filter.statusId=InvoiceStatusEnum.WaitingForPayment;
                this.filter.toDate=new Date();
                this.filter.dateTypeFilter=this.dateTypes.filter(u=>u.id==InvoiceDateTypeFilterEnum.DueDate)[0];
                this.filter.fromDate=new Date(1970, 0,1);
            }
            else{
                this.filter.statusId=parseInt(statusId);
            }
            this.onSearch();
        }
        });
  
    }
    onAllSelectedChange(){
        this.invoices.forEach(it=>{
            it.selected=this.selected;
        })
    }
    getStatues(){
        this.invoiceService.getInvoiceStatuses(this.filter).subscribe(result=>{
            this.statuses=result;
        });
        
    }
    onSelectCustomer(event:any){
        this.filter.customerId=this.selectCustomer.id;
        this.onSearch();
    }
    getCustomers(){
        this.customerService.getAll().subscribe(result=>{
            this.customers=result;
        });
    }
    onSearch() {
        this.filter.page = 1;
        this.getInvoices();
        this.getTotalPage();
        this.getStatues();
    }
    onTabStatusClick(status:InvoiceStatus){
        this.filter.statusId=status.id;
        this.onSearch();
    }
    getInvoices(){
        this.showProgressBar();
        this.invoiceService.getInvoices(this.filter).subscribe(result=>{
            this.invoices=result;
            if(this.selected){
                this.onAllSelectedChange();
            }
            console.log(this.invoices);
            this.hideProgressBar();
        },err=>{
            this.handleError(err);
        })
    }
    getTotalPage(){
        this.invoiceService.getTotalPages(this.filter).subscribe(result=>{
            this.filter.totalRow=result.totalRow;
            this.filter.totalPage=result.totalPage;
            this.setPage(1);
            console.log(this.filter);
        })
    }
    pageClick(page:number){
        this.setPage(page);
        this.filter.page=page;
        this.getInvoices();
    }
    setPage(page: number) {
        this.pager = this.pagerService.getPager(this.filter.totalRow, page, this.filter.pageSize);
    }
    ngOnInit(): void {
      this.getInvoices();
      this.getTotalPage();
      this.getStatues();
      this.getCustomers();
      this.organisationService.getBaseCurrency().subscribe(result=>{
        this.baseCurrency=result;
      });
      this.getPermission(this.menuService);
    }
    getTotalSelectedItems(){
        return this.invoices.filter(u=>u.selected).length;
    } 
    getTotalSelectedAmount(){
        let total:number=0;
        this.invoices.forEach(it=>{
            if(it.selected){
                total+=(it.totalIncludeTax*it.baseCurrencyExchangeRate)
            }
        })
        return total;
    }
    showPrintButton(){
        return this.invoices.filter(u=>u.selected).length==1;
    }
    showPayButton(){
        return this.filter.statusId==InvoiceStatusEnum.WaitingForPayment && this.invoices.filter(u=>u.selected).length>0 && this.permission.approvaPayPurchaseSale;
    }
    showPrintReceiptButton(){
        return this.invoices.filter(u=>u.selected).length==1;
    }
    onPrint(){
        let invoice=this.invoices.filter(u=>u.selected)[0];
        this.print(this.printService,invoice.id,PrintDocumentType.Invoice,invoice.number);
       
    }
    pay(){
        if(confirm(Utility.moveAllInvoiceToPaid())) {
            let ids:number[]=[];
            this.invoices.forEach(it=>{
                if(it.selected){
                    ids.push(it.id);
                }
            })
            this.showProgressBar();
            this.invoiceService.payAll(ids).subscribe(result=>{
                this.hideProgressBar();
                this.onSearch();
            },err=>{
                this.handleError(err);
            })
        }
        
    }
    onPrintReceipt(){
        const modalRef = this.modalService.open(PrintReceiptComponent);
        modalRef.componentInstance.init(this.invoices.filter(u=>u.selected)[0],modalRef);
    }
}

