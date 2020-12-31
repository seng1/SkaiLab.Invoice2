import { Component, OnInit, ViewChild } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { ProductFilter } from '../models/filter';
import { Product } from '../models/product';
import { ParentComponent } from '../parentComponent';
import { MenuService } from '../service/menu-service';
import { PagerService } from '../service/page-service';
import { ProductService } from '../service/product-service';

@Component({
    selector: 'product-component',
    templateUrl: './product-component.html'
})
export class ProductComponent extends ParentComponent implements OnInit {
    filter: ProductFilter = new ProductFilter();
    products: Product[] = [];
    constructor(private productService: ProductService,
         private pagerService: PagerService,
         private translate: TranslateService,
         private menuService:MenuService) {
        super("Products");
        this.ShowBackButton();
        this.setPageTitleFromLocalise(this.translate,"productsOrServices");
    }
    ngOnInit(): void {
        this.getProducts();
        this.getTotalPages();
        this.getPermission(this.menuService);
    }
    getProducts() {
        this.showProgressBar();
        this.productService.gets(this.filter).subscribe(result => {
            this.products = result;
            this.hideProgressBar();
        }, err => {
            this.handleError(err);
        })
    }
    getTotalPages() {
        this.productService.getTotalPages(this.filter).subscribe(result => {

        });
    }
    pageClick(page: number) {
        this.setPage(page);
        this.filter.page = page;
        this.getProducts();
    }
    setPage(page: number) {
        this.pager = this.pagerService.getPager(this.filter.totalRow, page, this.filter.pageSize);
    }
    onSearch() {
        this.filter.page = 1;
        this.getProducts();
        this.getTotalPages();
    }
}


