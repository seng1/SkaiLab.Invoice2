import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { Product } from '../models/product';
import { Tax } from '../models/tax';
import { LocationService } from '../service/location-service';
import { ProductService } from '../service/product-service';
import { TaxService } from '../service/tax-service';
import { Location } from "../models/location";
import { OrganisationParentComponent } from '../OrganisationParentComponent';
import { OrganisationService } from '../service/organisation-service';
import { MenuService } from '../service/menu-service';
import { TranslateService } from '@ngx-translate/core';
@Component({
  selector: 'update-product-component',
  templateUrl: './update-product-component.html'
})
export class UpdateProductComponent extends OrganisationParentComponent implements OnInit {
  @ViewChild('f', { static: true }) form: NgForm;
  product:Product=new Product();
  locations:Location[]=[];
  taxes:Tax[]=[];
  constructor(private router: Router,
    organisationService:OrganisationService,
    private productService:ProductService,
    private taxService:TaxService, 
    private route: ActivatedRoute,
    private menuService:MenuService,
    private translate: TranslateService,
    private locationService:LocationService 
    ) {
    super("Update Product",organisationService);
    this.setPageTitleFromLocalise(this.translate,"updateProductOrService");
    this.ShowBackButton();
    this.router.events.subscribe((evt) => {
        if (evt instanceof NavigationEnd) {
            this.router.navigated = false;
            var param = this.route.snapshot.params;
            this.id = param.id.replace(":", "");
        }
    });
  }
  ngOnInit(): void {
      this.showProgressBar();
      this.taxService.gets().subscribe(result=>{
        this.taxes=result;
        let c =new Tax();
        c.id=null;
        c.name=this.selectText();
        this.taxes.splice(0, 0, c);
      },err=>{
          this.handleError(err);
      });
      this.productService.get(this.id).subscribe(result=>{
        this.product=result;
        this.hideProgressBar();
      },err=>{
        this.handleError(err);
      });
      this.locationService.gets().subscribe(result=>{
        this.locations=result;
        let c =new Location();
        c.id=null;
        c.name=this.selectText();
        this.locations.splice(0, 0, c);
      });
      this.getPermission(this.menuService);
  }
  fileChange(event: any) {
    if (event) {
      let self = this;
      var reader = new FileReader();
      reader.readAsDataURL(event.target.files[0]);
      reader.onload = function () {
        self.product.imageUrl = reader.result.toString();
      };
    }
  };
  onSave(isClose:boolean){
    console.log(this.form);
    if (this.form.invalid) {
        return;
    }
    this.showProgressBar();
    this.productService.update(this.product).subscribe(result=>{
        this.hideProgressBar();
        if(isClose){
            this.router.navigate(['/product'])
        }
        else{
            this.router.navigate(['/product-new'])
        }
    },err=>{
        this.handleError(err);
    })
      
  }
  onNameChange(){
   this.product.productSaleInformation.title=this.product.name;
   this.product.productPurchaseInformation.title=this.product.name;
  }

}

