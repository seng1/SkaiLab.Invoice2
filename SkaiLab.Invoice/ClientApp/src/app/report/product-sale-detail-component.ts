import { Component, ElementRef, OnInit, ViewChild } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { TranslateService } from "@ngx-translate/core";
import { ProductSaleDetail } from "../models/product-sale-detail-report";
import { ReportFilter } from "../models/report-filter";
import { OrganisationService } from "../service/organisation-service";
import { ReportService } from "../service/report-service";
import { UserService } from "../service/user-service";
import { ParentReportComponent } from "./parent-report-component";

@Component({
    selector: 'product-sale-detail-component',
    templateUrl: './product-sale-detail-component.html',
})
export class ProductAndServiceSaleDetailReportComponent extends ParentReportComponent implements OnInit {
    @ViewChild('content', { static: true }) content: ElementRef;
    filter: ReportFilter = new ReportFilter();
    productSaleDetail: ProductSaleDetail = new ProductSaleDetail();
    productName:string;
    productId:any=0;
    constructor(private organisationService: OrganisationService ,
        private route: ActivatedRoute, 
        private router: Router, 
        private userService: UserService, 
        private translate: TranslateService,
        private reportService: ReportService) {
        super("Product & Service Sale Detail");
        this.ShowBackButton();
        this.setPageTitleFromLocalise(this.translate,"productAndServiceSaleDetailReport");
        this.route.queryParamMap.subscribe((parm)=>{
            var filterTypeId=parm.get("fileTypeId");
            this.productId=parm.get("productId");
            this.filter.periods.forEach(it=>{
                if(parseInt(it.id.toString())==parseInt(filterTypeId)){
                    this.filter.periodFilter=it;
                }
                this.filter.calDate();
            })
            this.productName=parm.get("productName").toString();
            this.filter.organisationIds.push(this.userService.getWorkingOrganisationId().toString());
            this.getReport();
       })
    }
    ngOnInit(): void {
        this.init(this.organisationService, this.userService);
        this.getWorkingOrganisation(this.userService);
       
    }
    printData() {
        this.reportService.generatePdf("Product & Service Sale Detail.pdf","letter","p",this.content.nativeElement.innerHTML);
    }
   getReport(){
    this.showProgressBar();
    this.reportService.getProductSaleDetail(this.productId,this.filter).subscribe(result=>{
        this.productSaleDetail=result;
        this.hideProgressBar();
        console.log(this.productSaleDetail);
    },err=>{
        this.handleError(err);
    })
   }
   getTotalPurchaseQty(){
       var total:number=0;
       this.productSaleDetail.purchaseItems.forEach(it=>{
           total+=it.qty;
       })
       return total;
   }
   getPurchaseTotal(){
    var total:number=0;
    this.productSaleDetail.purchaseItems.forEach(it=>{
        total+=it.total;
    })
    return total;
   }
   getTotalSaleQty(){
    var total:number=0;
    this.productSaleDetail.saleItems.forEach(it=>{
        total+=it.qty;
    })
    return total;
   }
   getSaleTotal(){
    var total:number=0;
    this.productSaleDetail.saleItems.forEach(it=>{
        total+=it.total;
    })
    return total;
   }
}


