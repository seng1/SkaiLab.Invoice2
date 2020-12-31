import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { StatusOverview } from '../models/status-overview';
import { ParentComponent } from '../parentComponent';
import { InvoiceService } from '../service/invoice-service';
import { MenuService } from '../service/menu-service';
import { QuoteService } from '../service/quote-service';

@Component({
  selector: 'invoice-overview-component',
  templateUrl: './invoice-overview-component.html'
})
export class SaleOverViewComponent extends ParentComponent implements OnInit {
  invoiceOverviews:StatusOverview[]=[];
  quoteOverviews:StatusOverview[]=[];
  constructor(private router:Router,
    private quoteService:QuoteService,
    private menuService:MenuService,
    private translate: TranslateService,
    private invoiceService:InvoiceService){
    super("Sale Overview");
    this.setPageTitleFromLocalise(this.translate,"saleOverview");
    this.ShowBackButton();
  }
  ngOnInit(): void {
    this.quoteService.getOverView().subscribe(result=>{
        this.quoteOverviews=result;
    },err=>{
        this.handleError(err);
    });
    this.invoiceService.getOverView().subscribe(result=>{
        this.invoiceOverviews=result;
    },err=>{
        this.handleError(err);
    })
    this.getPermission(this.menuService);
  }
  onInvoiceNavigate(overView:StatusOverview){
    this.router.navigate(['/invoice'], { queryParams: { statusId: overView.statusId } })
  }
  onQuoteNavigate(overView:StatusOverview){
    this.router.navigate(['/quote'], { queryParams: { statusId: overView.statusId } })
  }
}

