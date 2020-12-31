
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { Currency } from '../models/currency';
import { Customer } from '../models/customer';
import { DateTypeFilter } from '../models/date-type-filter';
import { PrintDocumentType, QuoteEnum, QuoteFilterEnum } from '../models/enum';
import { QuoteFilter } from '../models/filter';
import { QuoteStatus } from '../models/quote';
import { Quote } from '../models/quote';
import { Utility } from '../models/utility';
import { ParentComponent } from '../parentComponent';
import { CustomerService } from '../service/customer-service';
import { MenuService } from '../service/menu-service';
import { OrganisationService } from '../service/organisation-service';
import { PagerService } from '../service/page-service';
import { PrintService } from '../service/print-service';
import { QuoteService } from '../service/quote-service';

@Component({
    selector: 'quote-component',
    templateUrl: './quote-component.html'
})
export class QuoteComponent extends ParentComponent implements OnInit {
    filter: QuoteFilter = new QuoteFilter();
    quotes: Quote[] = [];
    customers:Customer[]=[];
    statuses:QuoteStatus[]=[];
    selectCustomer:Customer=null;
    selected:boolean=false;
    baseCurrency:Currency=new Currency();
    dateTypes:DateTypeFilter[]=[];
    constructor(private quoteService: QuoteService
        ,private pagerService: PagerService,
        private organisationService:OrganisationService,
        private router: Router,
        private route: ActivatedRoute,
        private menuService:MenuService
        ,private printervice:PrintService,
        private translate: TranslateService
        ,private customerService:CustomerService) {
        super("Quotes");
        this.setPageTitleFromLocalise(this.translate,"quotes");
        this.ShowBackButton();
       this.dropdownConfig.displayKey="displayName";
       this.dropdownConfig.searchOnKey="displayName";
       this.dropdownConfig.placeholder=this.allText();
       this.dateTypes=Utility.getQuoteDateTypeFilter();
       this.filter.dateTypeFilter=this.dateTypes[0];
       this.route.queryParamMap.subscribe((parm)=>{
        var statusId=parm.get("statusId");
        if(statusId!=null){
            if(parseInt(statusId)-QuoteEnum.Expire==0){
                this.filter.statusId=QuoteEnum.Draft;
                this.filter.toDate=new Date();
                this.filter.dateTypeFilter=this.dateTypes.filter(u=>u.id==QuoteFilterEnum.Expire)[0];
                this.filter.fromDate=null;

            }
            else{
                this.filter.statusId=parseInt(statusId);
            }
           
            this.onSearch();
        }
   })
    }
    ngOnInit(): void {
        this.getQuotes();
        this.getTotalPages();
        this.customerService.getAll().subscribe(result=>{
            this.customers=result;
        });
       this.getStatues();
       this.organisationService.getBaseCurrency().subscribe(result=>{
           this.baseCurrency=result;
       })
       this.getPermission(this.menuService);
    }
    
    getStatues(){
        this.quoteService.getQuoteStatuses(this.filter).subscribe(result=>{
            this.statuses=result;
        });
    }
    onTabStatusClick(status:QuoteStatus){
        this.filter.statusId=status.id;
        this.onSearch();
    }
    onSearch() {
        this.filter.page = 1;
        this.getQuotes();
        this.getTotalPages();
    }
    onAllSelectedChange(){
        this.quotes.forEach(it=>{
            it.selected=this.selected;
        })
    }
    getQuotes() {
        this.showProgressBar();
        this.quoteService.gets(this.filter).subscribe(result => {
            this.quotes = result;
            if(this.selected){
                this.quotes.forEach(it=>{
                    it.selected=this.selected;
                })
            }
            this.hideProgressBar();
        }, err => {
            this.handleError(err);
        })
    }
    getTotalPages() {
        this.quoteService.getTotalPages(this.filter).subscribe(result => {
            this.filter.totalRow=result.totalRow;
            this.filter.totalPage=result.totalPage;
            this.setPage(1);
        })
    }
    pageClick(page:number){
        this.setPage(page);
        this.filter.page=page;
        this.getQuotes();
    }
    setPage(page: number) {
        this.pager = this.pagerService.getPager(this.filter.totalRow, page, this.filter.pageSize);
    }
    showPrintButton(){
        return this.quotes.filter(u=>u.selected).length==1;
    }
    showAcceptButton(){
        return this.filter.statusId==QuoteEnum.Draft && this.quotes.filter(u=>u.selected).length>0 && this.permission.approvaPayPurchaseSale;
    }
    showDeclineButton(){
        return this.filter.statusId==QuoteEnum.Draft && this.quotes.filter(u=>u.selected).length>0&& this.permission.approvaPayPurchaseSale;
    }
    showCreateInvoiceButton(){
        return this.filter.statusId==QuoteEnum.Accepted && this.quotes.filter(u=>u.selected).length==1&& this.permission.readWritePurchaseSale;
    }
    showDeleteButton(){
        return (this.filter.statusId==QuoteEnum.Accepted || this.filter.statusId==QuoteEnum.Draft) && this.quotes.filter(u=>u.selected).length>0&& this.permission.approvaPayPurchaseSale;
    }
    getTotalSelectedItems(){
        return this.quotes.filter(u=>u.selected).length
    }
    getTotalSelectedAmount(){
        var totalTotal:number=0;
        this.quotes.forEach(it=>{
            if(it.selected){
                totalTotal+=it.totalIncludeTax*it.baseCurrencyExchangeRate
            }
        })
        return totalTotal;
    }
    onAcceptAll(){
        var ids:number[]=[];
        this.quotes.forEach(it=>{
            if(it.selected){
                ids.push(it.id);
            }
        })
        if(ids.length==0){
            this.showErrorText(Utility.pleaseSelectQuoteText());
            return;
        }
        this.showProgressBar();
        this.quoteService.acceptAll(ids).subscribe(result=>{
            this.filter.page=1;
            this.hideProgressBar();
            this.getQuotes();
            this.getStatues();
        },err=>{
            this.handleError(err);
        })

    }
    onDeclineAll(){
        if(confirm(Utility.moveAllQuoteToDeclineQ())) {
            var ids:number[]=[];
            this.quotes.forEach(it=>{
                if(it.selected){
                    ids.push(it.id);
                }
            })
            if(ids.length==0){
                this.showErrorText(Utility.pleaseSelectQuoteText());
                return;
            }

            this.showProgressBar();
            this.quoteService.declineAll(ids).subscribe(result=>{
                this.filter.page=1;
                this.hideProgressBar();
                this.getQuotes();
                this.getStatues();
            },error=>{
                this.handleError(error);
            });
        }
    }
    onDeleteAll(){
        if(confirm(Utility.moveAllQuoteToDeleteQ())) {
            var ids:number[]=[];
            this.quotes.forEach(it=>{
                if(it.selected){
                    ids.push(it.id);
                }
            })
            if(ids.length==0){
                this.showErrorText(Utility.pleaseSelectQuoteText());
                return;
            }

            this.showProgressBar();
            this.quoteService.deleteAll(ids).subscribe(result=>{
                this.filter.page=1;
                this.hideProgressBar();
                this.getQuotes();
                this.getStatues();
            },error=>{
                this.handleError(error);
            });
        }
    }
    onCreateInvoice(){
        var quote:Quote=this.quotes.filter(u=>u.selected)[0];
        this.router.navigate(['/invoice-newquote',quote.id])
    }
    onPrint(){
        var quote=this.quotes.filter(u=>u.selected)[0];
        this.print(this.printervice,quote.id,PrintDocumentType.Quote,quote.number);
    }
}

