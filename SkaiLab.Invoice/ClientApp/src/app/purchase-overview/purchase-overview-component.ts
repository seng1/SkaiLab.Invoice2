import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { Currency } from '../models/currency';
import { StatusOverview } from '../models/status-overview';
import { ParentComponent } from '../parentComponent';
import { BillService } from '../service/bill-service';
import { MenuService } from '../service/menu-service';
import { OrganisationService } from '../service/organisation-service';
import { PurchaseService } from '../service/purchase-service';

@Component({
  selector: 'purchase-overview-component',
  templateUrl: './purchase-overview-component.html'
})
export class PurchaseOverViewComponent extends ParentComponent implements OnInit {
  baseCurrency:Currency=new Currency();
  billOverviews:StatusOverview[]=[];
  purchaseOverviews:StatusOverview[]=[];
  constructor(private router:Router,
    private organisationService:OrganisationService,
    private purchaseOrderService:PurchaseService,
    private menuService:MenuService,
    private translate: TranslateService,
    private billService:BillService){
    super("Purchase Overview");
    this.ShowBackButton();
    this.setPageTitleFromLocalise(this.translate,"purchaseOverview");
  }
  ngOnInit(): void {
    this.organisationService.getBaseCurrency().subscribe(result=>{
      this.baseCurrency=result;
    })
    this.billService.getOverView().subscribe(result=>{
      this.billOverviews=result;
    },err=>{
      this.handleError(err);
    })
    this.purchaseOrderService.getOverView().subscribe(result=>{
      this.purchaseOverviews=result;
    },err=>{
      this.handleError(err);
    })
    this.getPermission(this.menuService);
  }
  onPurchaseNavigate(overView:StatusOverview){
    this.router.navigate(['/order'], { queryParams: { statusId: overView.statusId } })
  }
  onBillNavigate(overView:StatusOverview){
    this.router.navigate(['/vendor-bill'], { queryParams: { statusId: overView.statusId } })
  }
}

