import { Component, ElementRef, OnInit, ViewChild } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { TranslateService } from "@ngx-translate/core";
import { InventoryHistoryDetail } from "../models/inventoryHistory";
import { Product } from "../models/product";
import { InventoryHistoryFilter } from "../models/report-filter";
import { OrganisationService } from "../service/organisation-service";
import { ReportService } from "../service/report-service";
import { UserService } from "../service/user-service";
import { ParentReportComponent } from "./parent-report-component";

@Component({
    selector: 'inventory-balance-detail-component',
    templateUrl: './inventory-balance-detail-component.html',
})
export class InventoryBalanceDetailReportComponent extends ParentReportComponent implements OnInit {
    @ViewChild('content', { static: true }) content: ElementRef;
    productName:string;
    productId:any=0;
    locationBalances:Product[]=[];
    filter: InventoryHistoryFilter = new InventoryHistoryFilter();
    loadingBalance:boolean=true;
    inventoryHistoryDetails:InventoryHistoryDetail[]=[];
    hasMoreInventory:boolean=true;
    constructor(private organisationService: OrganisationService ,
        private route: ActivatedRoute, 
        private router: Router, 
        private userService: UserService, 
        private translate: TranslateService,
        private reportService: ReportService) {
        super("Inventory Balance Detail");
        this.ShowBackButton();
        this.setPageTitleFromLocalise(this.translate,"inventoryBalanceDetailReport");
        this.route.queryParamMap.subscribe((parm)=>{
            this.productId=parm.get("productId");
            this.productName=parm.get("productName").toString();

       })
    }
    ngOnInit(): void {
        this.init(this.organisationService, this.userService);
        this.getLocationBalance();
        this.getInventoryHistoryDetails();
    }
    getLocationBalance(){
        this.reportService.GetProductInventoryByLocation(this.productId).subscribe(result=>{
            this.locationBalances=result;
            this.loadingBalance=false;
        })
    }
    printData() {
        this.reportService.generatePdf("Inventory Balance Detail.pdf","letter","p",this.content.nativeElement.innerHTML);
    }
    onPeriodChange() {
        this.filter.calDate();
        this.onApplyFilter();
    }
    onApplyFilter(){
        this.inventoryHistoryDetails=[];
        this.filter.page=1;
        this.hasMoreInventory=true;
        this.getInventoryHistoryDetails();
    }
    getInventoryHistoryDetails(){
        this.showProgressBar();
        this.reportService.getInventoryHistories(this.productId,this.filter).subscribe(result=>{
            this.hideProgressBar();
            if(result.length>0){
                result.forEach(it=>{
                    this.inventoryHistoryDetails.push(it);
                })
                this.filter.page=this.filter.page+1;
            }
            if(result.length==0 || result.length < this.filter.pageSize){
                this.hasMoreInventory=false;
            }
        },err=>{
            this.handleError(err);
        })
    }
}


