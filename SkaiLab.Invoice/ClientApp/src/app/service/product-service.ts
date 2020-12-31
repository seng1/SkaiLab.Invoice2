import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import 'rxjs/add/operator/map';
import { BaseService } from './base-service';
import { ProductFilter } from '../models/filter';
import { Product } from '../models/product';

@Injectable()
export class ProductService {
  constructor(private http: HttpClient,private baseService:BaseService) { }
  gets(filter:ProductFilter):Observable<Product[]>{
    let url = this.baseService.apiUrl + '/product/Gets';
    return this.http.post<Product[]>(url,filter,{headers:this.baseService.getRequestHeader(true)});
  }
  getTotalPages(filter:ProductFilter):Observable<Product[]>{
    let url = this.baseService.apiUrl + '/product/GetTotalPages';
    return this.http.post<Product[]>(url,filter,{headers:this.baseService.getRequestHeader(true)});
  }
  create(product:Product):Observable<Product>{
    let url = this.baseService.apiUrl + '/product/Create';
    return this.http.post<Product>(url,product,{headers:this.baseService.getRequestHeader(true)});
  }
  update(product:Product):Observable<Product>{
    let url = this.baseService.apiUrl + '/product/update';
    return this.http.post<Product>(url,product,{headers:this.baseService.getRequestHeader(true)});
  }
  get(id:any):Observable<Product>{
    let url = this.baseService.apiUrl + '/product/get/'+id;
    return this.http.get<Product>(url,{headers:this.baseService.getRequestHeader(true)});
  }
  getProductsForPurchase():Observable<Product[]>{
    let url = this.baseService.apiUrl + '/product/GetProductsForPurchase';
    return this.http.get<Product[]>(url,{headers:this.baseService.getRequestHeader(true)});
  }
  getProductsForSale():Observable<Product[]>{
    let url = this.baseService.apiUrl + '/product/GetProductsForSale';
    return this.http.get<Product[]>(url,{headers:this.baseService.getRequestHeader(true)});
  }
  getInventoryQty(id:any,locationId:any):Observable<number>{
    let url = this.baseService.apiUrl + '/product/GetInventoryQty/'+id+"/"+locationId;
    return this.http.get<number>(url,{headers:this.baseService.getRequestHeader(true)});
  }
}