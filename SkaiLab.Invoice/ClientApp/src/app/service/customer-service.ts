import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import 'rxjs/add/operator/map';
import { BaseService } from './base-service';
import { CustomerFilter } from '../models/filter';
import { Customer } from '../models/customer';

@Injectable()
export class CustomerService {
  constructor(private http: HttpClient,private baseService:BaseService) { }
  gets(filter:CustomerFilter):Observable<Customer[]>{
    let url = this.baseService.apiUrl + '/Customer/GetCustomers';
    return this.http.post<Customer[]>(url,filter,{headers:this.baseService.getRequestHeader(true)});
  }
  getTotalPages(filter:CustomerFilter):Observable<CustomerFilter>{
    let url = this.baseService.apiUrl + '/Customer/GetTotalPages';
    return this.http.post<CustomerFilter>(url,filter,{headers:this.baseService.getRequestHeader(true)});
  }
  add(customer:Customer):Observable<Customer>{
    let url = this.baseService.apiUrl + '/Customer/Add';
    return this.http.post<Customer>(url,customer,{headers:this.baseService.getRequestHeader(true)});
  }
  update(customer:Customer):Observable<Customer>{
    let url = this.baseService.apiUrl + '/Customer/update';
    return this.http.post<Customer>(url,customer,{headers:this.baseService.getRequestHeader(true)});
  }
  get(id:any):Observable<Customer>{
    let url = this.baseService.apiUrl + '/Customer/Get/'+id;
    return this.http.get<Customer>(url,{headers:this.baseService.getRequestHeader(true)});
  }
  getAll():Observable<Customer[]>{
    let url = this.baseService.apiUrl + '/Customer/getAll';
    return this.http.get<Customer[]>(url,{headers:this.baseService.getRequestHeader(true)});
  }
}