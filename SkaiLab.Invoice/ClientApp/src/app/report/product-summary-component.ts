import { Component, ElementRef, OnInit, ViewChild } from "@angular/core";
import { Router } from "@angular/router";
import { TranslateService } from "@ngx-translate/core";
import { ProductSaleSummary } from "../models/product-sale-summary";
import { ReportFilter } from "../models/report-filter";
import { OrganisationService } from "../service/organisation-service";
import { ReportService } from "../service/report-service";
import { UserService } from "../service/user-service";
import { ParentReportComponent } from "./parent-report-component";

@Component({
    selector: 'product-summary-component',
    templateUrl: './product-summary-component.html',
})
export class ProductAndServiceSaleSummaryReportComponent extends ParentReportComponent implements OnInit {
    @ViewChild('content', { static: true }) content: ElementRef;
    filter: ReportFilter = new ReportFilter();
    productSaleSummaries: ProductSaleSummary[] = [];
    constructor(private organisationService: OrganisationService,
         private router: Router, 
         private translate: TranslateService,
         private userService: UserService, 
         private reportService: ReportService) {
        super("Product & Service Sale Summary");
        this.ShowBackButton();
        this.setPageTitleFromLocalise(this.translate,"productAndServiceSummaryReport");

    }
    ngOnInit(): void {
        this.init(this.organisationService, this.userService);
        this.getWorkingOrganisation(this.userService);
        this.filter.organisationIds.push(this.userService.getWorkingOrganisationId().toString());
        this.onApplyFilter();
    }
    onPeriodChange() {
        this.filter.calDate();
        this.onApplyFilter();
    }
    printData() {
        this.reportService.generatePdf("Product & Service Sale Summary.pdf","letter","l",this.content.nativeElement.innerHTML);
    }
    onApplyFilter() {
        this.showProgressBar();
        this.reportService.getProductSaleSummaries(this.filter).subscribe(result=>{
            this.productSaleSummaries=result;
            this.hideProgressBar();
            console.log(this.productSaleSummaries);
        },err=>{
            this.handleError(err);
        })
       
    }
    getPurchaseQty():number{
        var totalQty:number=0;
        this.productSaleSummaries.forEach(it=>{
           totalQty+=it.purchaseQty;
        })
        return totalQty;
    }
    getPurchaseTotal():number{
        var purchaseTotal:number=0;
        this.productSaleSummaries.forEach(it=>{
            purchaseTotal+=it.purchaseTotal;
        })
        return purchaseTotal;
    }
    getSaleQty():number{
        var totalQty:number=0;
        this.productSaleSummaries.forEach(it=>{
           totalQty+=it.saleQty;
        })
        return totalQty;
    }
    getSaleTotal():number{
        var purchaseTotal:number=0;
        this.productSaleSummaries.forEach(it=>{
            purchaseTotal+=it.saleTotal;
        })
        return purchaseTotal;
    }
    getNetQty():number{
        var totalQty:number=0;
        this.productSaleSummaries.forEach(it=>{
           totalQty+=(it.purchaseQty - it.saleQty);
        })
        return totalQty;
    }
    getNetTotal():number{
        var totalQty:number=0;
        this.productSaleSummaries.forEach(it=>{
           totalQty+=(it.saleTotal - it.purchaseTotal);
        })
        return totalQty;
    }
    onProductClick(productSaleSummary:ProductSaleSummary){
        this.router.navigate(['/report-productandservicesaledetail'], { queryParams: { productId: productSaleSummary.product.id,fileTypeId:this.filter.periodFilter.id,productName:productSaleSummary.product.code+" - "+productSaleSummary.product.name } });
    }
}


