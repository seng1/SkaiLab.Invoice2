import { Component, OnInit, ViewChild } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { VendorFilter } from '../models/filter';
import { Vendor } from '../models/vendor';
import { ParentComponent } from '../parentComponent';
import { MenuService } from '../service/menu-service';
import { PagerService } from '../service/page-service';
import { VendorService } from '../service/vendor-service';

@Component({
    selector: 'vendor-component',
    templateUrl: './vendor-component.html'
})
export class VendorComponent extends ParentComponent implements OnInit {
    filter: VendorFilter = new VendorFilter();
    vendors: Vendor[] = [];
    constructor(private vendorService: VendorService, 
        private pagerService: PagerService,
        private translate: TranslateService,
        private  menuService:MenuService) {
        super("Vendors");
        this.ShowBackButton();
        this.setPageTitleFromLocalise(this.translate,"vendors");
    }
    ngOnInit(): void {
        this.getVendors();
        this.getTotalPages();
        this.getPermission(this.menuService);
    }
    onSearch() {
        this.filter.page = 1;
        this.getVendors();
        this.getTotalPages();
    }
    getVendors() {
        this.showProgressBar();
        this.vendorService.getVendors(this.filter).subscribe(result => {
            this.vendors = result;
            this.hideProgressBar();
        }, err => {
            this.handleError(err);
        })
    }
    getTotalPages() {
        this.vendorService.getTotalPages(this.filter).subscribe(result => {
            this.filter = result;
            this.setPage(1);
        })
    }
    pageClick(page:number){
        this.setPage(page);
        this.filter.page=page;
        this.getVendors();
    }
    setPage(page: number) {
        this.pager = this.pagerService.getPager(this.filter.totalRow, page, this.filter.pageSize);
    }
}

