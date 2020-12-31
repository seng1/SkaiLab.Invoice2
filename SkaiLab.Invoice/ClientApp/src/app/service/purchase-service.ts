import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import 'rxjs/add/operator/map';
import { BaseService } from './base-service';
import { PurchaseOrder } from '../models/purchaseOrder';
import { PurchaseFilter } from '../models/filter';
import { PurchaseOrderForUpdate, PurchaseOrderLookup } from '../models/purchase-order-lookup';
import { ExpenseStatus } from '../models/expense';
import { StatusOverview } from '../models/status-overview';

@Injectable()
export class PurchaseService {
  constructor(private http: HttpClient,private baseService:BaseService) { }
  gets(filter:PurchaseFilter):Observable<PurchaseOrder[]>{
    filter.fromDate=new Date(filter.fromDate);
    filter.toDate=new Date(filter.toDate);
    let url = this.baseService.apiUrl + '/PurchaseOrder/Gets';
    return this.http.post<PurchaseOrder[]>(url,filter,{headers:this.baseService.getRequestHeader(true)});
  }
  getExpenseStatuses(filter:PurchaseFilter):Observable<ExpenseStatus[]>{
    filter.fromDate=new Date(filter.fromDate);
    filter.toDate=new Date(filter.toDate);
    let url = this.baseService.apiUrl + '/PurchaseOrder/GetExpenseStatuses';
    return this.http.post<ExpenseStatus[]>(url,filter,{headers:this.baseService.getRequestHeader(true)});
  }
  getTotalPages(filter:PurchaseFilter):Observable<PurchaseFilter>{
    
    let url = this.baseService.apiUrl + '/PurchaseOrder/GetTotalPages';
    return this.http.post<PurchaseFilter>(url,filter,{headers:this.baseService.getRequestHeader(true)});
  }
  getPurchaseLookupForCreateOrUpdate():Observable<PurchaseOrderLookup>{
    let url = this.baseService.apiUrl + '/PurchaseOrder/getPurchaseLookupForCreateOrUpdate';
    return this.http.get<PurchaseOrderLookup>(url,{headers:this.baseService.getRequestHeader(true)});
  }
  create(order:PurchaseOrder):Observable<PurchaseOrder>{
    if(order.deliveryDate!=null){
      order.deliveryDate=new Date(order.deliveryDate);
    }
    order.date=new Date(order.date);
    let url = this.baseService.apiUrl + '/PurchaseOrder/create';
    return this.http.post<PurchaseOrder>(url,order,{headers:this.baseService.getRequestHeader(true)});
  }
  createPurchaseOrderNumber():Observable<PurchaseOrder>{
    let url = this.baseService.apiUrl + '/PurchaseOrder/CreatePurchaseOrderNumber';
    return this.http.get<PurchaseOrder>(url,{headers:this.baseService.getRequestHeader(true)});
  }
  getForUpdate(id:any):Observable<PurchaseOrderForUpdate>{
    let url = this.baseService.apiUrl + '/PurchaseOrder/GetForUpdate/'+id;
    return this.http.get<PurchaseOrderForUpdate>(url,{headers:this.baseService.getRequestHeader(true)});
  }
  update(order:PurchaseOrder):Observable<PurchaseOrder>{
    if(order.deliveryDate!=null){
      order.deliveryDate=new Date(order.deliveryDate);
    }
    order.date=new Date(order.date);
    let url = this.baseService.apiUrl + '/PurchaseOrder/update';
    return this.http.post<PurchaseOrder>(url,order,{headers:this.baseService.getRequestHeader(true)});
  }
  markAsBill(order:PurchaseOrder):Observable<PurchaseOrder>{
    let url = this.baseService.apiUrl + '/PurchaseOrder/MarkAsBill';
    return this.http.post<PurchaseOrder>(url,order,{headers:this.baseService.getRequestHeader(true)});
  }
  markDelete(order:PurchaseOrder):Observable<PurchaseOrder>{
    let url = this.baseService.apiUrl + '/PurchaseOrder/MarkDelete';
    return this.http.post<PurchaseOrder>(url,order,{headers:this.baseService.getRequestHeader(true)});
  }
  markPurchaseOrdersAsWaitingForApproval(orders:number[]):Observable<PurchaseOrder>{
    let url = this.baseService.apiUrl + '/PurchaseOrder/markPurchaseOrdersAsWaitingForApproval';
    return this.http.post<PurchaseOrder>(url,orders,{headers:this.baseService.getRequestHeader(true)});
  }
  markPurchaseOrdersAsApprove(orders:PurchaseOrder[]):Observable<PurchaseOrder>{
    let url = this.baseService.apiUrl + '/PurchaseOrder/MarkPurchaseOrdersAsApprove';
    return this.http.post<PurchaseOrder>(url,orders,{headers:this.baseService.getRequestHeader(true)});
  }
  markPurchaseOrdersAsBill(orders:number[]):Observable<PurchaseOrder>{
    let url = this.baseService.apiUrl + '/PurchaseOrder/MarkPurchaseOrdersAsBill';
    return this.http.post<PurchaseOrder>(url,orders,{headers:this.baseService.getRequestHeader(true)});
  }
  markPurchaseOrdersAsDelete(orders:number[]):Observable<PurchaseOrder>{
    let url = this.baseService.apiUrl + '/PurchaseOrder/MarkPurchaseOrdersAsDelete';
    return this.http.post<PurchaseOrder>(url,orders,{headers:this.baseService.getRequestHeader(true)});
  }
  getOverView(): Observable<StatusOverview[]> {
    let url = this.baseService.apiUrl + '/PurchaseOrder/GetOverView';
    return this.http.get<StatusOverview[]>(url, { headers: this.baseService.getRequestHeader(true) });
  }
}