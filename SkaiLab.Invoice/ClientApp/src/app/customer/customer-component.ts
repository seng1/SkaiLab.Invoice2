import { Component, OnInit, ViewChild } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { Customer } from '../models/customer';
import { CustomerFilter } from '../models/filter';
import { ParentComponent } from '../parentComponent';
import { CustomerService } from '../service/customer-service';
import { MenuService } from '../service/menu-service';
import { PagerService } from '../service/page-service';

@Component({
    selector: 'customer-component',
    templateUrl: './customer-component.html'
})
export class CustomerComponent extends ParentComponent implements OnInit {
    filter: CustomerFilter = new CustomerFilter();
    customers: Customer[] = [];
    constructor(private customerService: CustomerService, 
        private pagerService: PagerService,
        private translate: TranslateService,
        private menuService:MenuService) {
        super("Customers");
        this.ShowBackButton();
        this.setPageTitleFromLocalise(this.translate,"customers");
    }
    ngOnInit(): void {
        this.getCustomers();
        this.getTotalPages();
        this.getPermission(this.menuService);
    }
    onSearch() {
        this.filter.page = 1;
        this.getCustomers();
        this.getTotalPages();
    }
    getCustomers() {
        this.showProgressBar();
        this.customerService.gets(this.filter).subscribe(result => {
            this.customers = result;
            this.hideProgressBar();
        }, err => {
            this.handleError(err);
        })
    }
    getTotalPages() {
        this.customerService.getTotalPages(this.filter).subscribe(result => {
            this.filter = result;
            this.setPage(1);
        })
    }
    pageClick(page:number){
        this.setPage(page);
        this.filter.page=page;
        this.getCustomers();
    }
    setPage(page: number) {
        this.pager = this.pagerService.getPager(this.filter.totalRow, page, this.filter.pageSize);
    }
}

