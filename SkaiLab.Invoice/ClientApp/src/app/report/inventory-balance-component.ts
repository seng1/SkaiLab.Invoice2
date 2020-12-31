import { Component, ElementRef, OnInit, ViewChild } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { TranslateService } from "@ngx-translate/core";
import { Product } from "../models/product";
import { OrganisationService } from "../service/organisation-service";
import { ReportService } from "../service/report-service";
import { UserService } from "../service/user-service";
import { ParentReportComponent } from "./parent-report-component";

@Component({
    selector: 'inventory-balance-component',
    templateUrl: './inventory-balance-component.html',
})
export class InventoryBalanceReportComponent extends ParentReportComponent implements OnInit {
    @ViewChild('content', { static: true }) content: ElementRef;
    products:Product[]=[];
    searchText:string="";
    constructor(private organisationService: OrganisationService ,
        private route: ActivatedRoute, 
        private router: Router,
        private translate: TranslateService,
        private userService: UserService,
        private reportService: ReportService) {
        super("Inventory Balance");
        this.ShowBackButton();
        this.setPageTitleFromLocalise(this.translate,"inventoryBalanceReport");
      
    }
    ngOnInit(): void {
        this.init(this.organisationService, this.userService);
        this.onSearch();
    }
    onSearch(){
        this.showProgressBar();
        this.reportService.GetProductInventoriesBalance(this.searchText).subscribe(result=>{
            this.products=result;
            this.hideProgressBar();
            console.log(this.products);
        },err=>{
            this.handleError(err);
        })
    }
    printData() {
        this.reportService.generatePdf("Inventory Balance.pdf","letter","p",this.content.nativeElement.innerHTML);
    }
    detailClick(product:Product){
        this.router.navigate(['/report-inventorybalancedetail'], { queryParams: { productId: product.id,productName:product.code+" - "+product.name } });
    }
}


