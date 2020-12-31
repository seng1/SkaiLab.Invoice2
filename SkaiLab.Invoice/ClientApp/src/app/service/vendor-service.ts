import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import 'rxjs/add/operator/map';
import { BaseService } from './base-service';
import { VendorFilter } from '../models/filter';
import { Vendor } from '../models/vendor';

@Injectable()
export class VendorService {
  constructor(private http: HttpClient,private baseService:BaseService) { }
  getVendors(filter:VendorFilter):Observable<Vendor[]>{
    let url = this.baseService.apiUrl + '/vendor/GetVendors';
    return this.http.post<Vendor[]>(url,filter,{headers:this.baseService.getRequestHeader(true)});
  }
  getAllVendors():Observable<Vendor[]>{
    let url = this.baseService.apiUrl + '/vendor/GetAllVendors';
    return this.http.get<Vendor[]>(url,{headers:this.baseService.getRequestHeader(true)});
  }
  getTotalPages(filter:VendorFilter):Observable<VendorFilter>{
    let url = this.baseService.apiUrl + '/vendor/GetTotalPage';
    return this.http.post<VendorFilter>(url,filter,{headers:this.baseService.getRequestHeader(true)});
  }
  add(vendor:Vendor):Observable<Vendor>{
    let url = this.baseService.apiUrl + '/vendor/Add';
    return this.http.post<Vendor>(url,vendor,{headers:this.baseService.getRequestHeader(true)});
  }
  update(vendor:Vendor):Observable<Vendor>{
    let url = this.baseService.apiUrl + '/vendor/update';
    return this.http.post<Vendor>(url,vendor,{headers:this.baseService.getRequestHeader(true)});
  }
  get(id:any):Observable<Vendor>{
    let url = this.baseService.apiUrl + '/vendor/Get/'+id;
    return this.http.get<Vendor>(url,{headers:this.baseService.getRequestHeader(true)});
  }
}