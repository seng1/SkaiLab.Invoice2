import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { InventoryHistory } from '../models/inventoryHistory';
import { Product } from '../models/product';
import { Tax } from '../models/tax';
import { LocationService } from '../service/location-service';
import { ProductService } from '../service/product-service';
import { TaxService } from '../service/tax-service';
import { Location } from "../models/location";
import { OrganisationParentComponent } from '../OrganisationParentComponent';
import { OrganisationService } from '../service/organisation-service';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'new-product-component',
  templateUrl: './new-product-component.html'
})
export class NewProductComponent extends OrganisationParentComponent implements OnInit {
  @ViewChild('f', { static: true }) form: NgForm;
  
  product:Product=new Product();
  taxes:Tax[]=[];
  locations:Location[]=[];
  constructor(private router: Router, 
    organisationService:OrganisationService,
    private productService:ProductService,
    private taxService:TaxService,
    private translate: TranslateService,
    private locationService:LocationService ) {
    super("New Product",organisationService);
    this.ShowBackButton();
    this.setPageTitleFromLocalise(this.translate,"newProductOrService");
  }
  ngOnInit(): void {
      this.showProgressBar();
      this.taxService.gets().subscribe(result=>{
        this.taxes=result;
        let c =new Tax();
        c.id=null;
        c.name=this.selectText();
        this.taxes.splice(0, 0, c);
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
    if (this.form.invalid) {
        return;
    }
    this.showProgressBar();
    this.productService.create(this.product).subscribe(result=>{
        this.hideProgressBar();
        if(isClose){
            this.router.navigate(['/product'])
        }
        else{
            this.product=new Product();
        }
    },err=>{
        this.handleError(err);
    })
  }
  onTrackingInventoryChange(){
    if(this.product.trackInventory&& this.product.inventoryHistories.length==0){
      this.showProgressBar();
      this.locationService.gets().subscribe(result=>{
        result.forEach(it=>{
          let inventoryHistory:InventoryHistory=new InventoryHistory();
          inventoryHistory.locationId=it.id;
          inventoryHistory.location=it;
          inventoryHistory.unitPrice=this.product.productPurchaseInformation.price;
          this.product.inventoryHistories.push(inventoryHistory);
        });
        this.hideProgressBar();
      },err=>{
        this.handleError(err);
      })
    }
  }
  onNameChange(){
    this.product.productSaleInformation.title=this.product.name;
    this.product.productPurchaseInformation.title=this.product.name;
   }
}

