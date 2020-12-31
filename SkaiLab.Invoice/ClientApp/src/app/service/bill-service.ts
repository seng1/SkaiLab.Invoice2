import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import 'rxjs/add/operator/map';
import { BaseService } from './base-service';
import { PurchaseFilter } from '../models/filter';
import { Expense, ExpenseForUpdate, ExpenseStatus } from '../models/expense';
import { PurchaseOrderLookup } from '../models/purchase-order-lookup';
import { StatusOverview } from '../models/status-overview';

@Injectable()
export class BillService {
  constructor(private http: HttpClient, private baseService: BaseService) { }
  gets(filter: PurchaseFilter): Observable<Expense[]> {
    filter.fromDate = new Date(filter.fromDate);
    filter.toDate = new Date(filter.toDate);
    let url = this.baseService.apiUrl + '/bill/Gets';
    return this.http.post<Expense[]>(url, filter, { headers: this.baseService.getRequestHeader(true) });
  }
  getTotalPages(filter: PurchaseFilter): Observable<PurchaseFilter> {
    filter.fromDate = new Date(filter.fromDate);
    filter.toDate = new Date(filter.toDate);
    let url = this.baseService.apiUrl + '/bill/GetTotalPages';
    return this.http.post<PurchaseFilter>(url, filter, { headers: this.baseService.getRequestHeader(true) });
  }
  getExpenseStatuses(filter:PurchaseFilter):Observable<ExpenseStatus[]>{
    filter.fromDate=new Date(filter.fromDate);
    filter.toDate=new Date(filter.toDate);
    let url = this.baseService.apiUrl + '/bill/GetExpenseStatuses';
    return this.http.post<ExpenseStatus[]>(url,filter,{headers:this.baseService.getRequestHeader(true)});
  }
  getBillLookupForCreateOrUpdate(): Observable<PurchaseOrderLookup> {
    let url = this.baseService.apiUrl + '/bill/GetBillLookupForCreateOrUpdate';
    return this.http.get<PurchaseOrderLookup>(url, { headers: this.baseService.getRequestHeader(true) });
  }
  getOverView(): Observable<StatusOverview[]> {
    let url = this.baseService.apiUrl + '/bill/GetOverView';
    return this.http.get<StatusOverview[]>(url, { headers: this.baseService.getRequestHeader(true) });
  }
  
  add(expense: Expense): Observable<Expense> {
    if (expense.deliveryDate != null) {
      expense.deliveryDate = new Date(expense.deliveryDate);
    }
    expense.date = new Date(expense.date);
    let url = this.baseService.apiUrl + '/bill/Create';
    return this.http.post<Expense>(url, expense, { headers: this.baseService.getRequestHeader(true) });
  }
  getExpenseForUpdate(id: any): Observable<ExpenseForUpdate> {
    let url = this.baseService.apiUrl + '/bill/GetExpenseForUpdate/' + id;
    return this.http.get<ExpenseForUpdate>(url, { headers: this.baseService.getRequestHeader(true) });
  }
  update(expense: Expense): Observable<Expense> {
    if (expense.deliveryDate != null) {
      expense.deliveryDate = new Date(expense.deliveryDate);
    }
    expense.date = new Date(expense.date);
    let url = this.baseService.apiUrl + '/bill/update';
    return this.http.post<Expense>(url, expense, { headers: this.baseService.getRequestHeader(true) });
  }
  markAsBill(expense: Expense): Observable<Expense> {
    let url = this.baseService.apiUrl + '/bill/MarkAsBill';
    return this.http.post<Expense>(url, expense, { headers: this.baseService.getRequestHeader(true) });
  }
  markDelete(expense: Expense): Observable<Expense> {
    if (expense.deliveryDate != null) {
      expense.deliveryDate = new Date(expense.deliveryDate);
    }
    expense.date = new Date(expense.date);
    let url = this.baseService.apiUrl + '/bill/MarkDelete';
    return this.http.post<Expense>(url, expense, { headers: this.baseService.getRequestHeader(true) });
  }
  markBillsAsWaitingForApproval(ids: number[]): Observable<Expense> {
    let url = this.baseService.apiUrl + '/bill/MarkBillsAsWaitingForApproval';
    return this.http.post<Expense>(url, ids, { headers: this.baseService.getRequestHeader(true) });
  }
  markBillsOrdersAsApprove(ids: Expense[]): Observable<Expense> {
    let url = this.baseService.apiUrl + '/bill/MarkBillsOrdersAsApprove';
    return this.http.post<Expense>(url, ids, { headers: this.baseService.getRequestHeader(true) });
  }
  markBillsAsBill(ids: number[]): Observable<Expense> {
    let url = this.baseService.apiUrl + '/bill/MarkBillsAsBill';
    return this.http.post<Expense>(url, ids, { headers: this.baseService.getRequestHeader(true) });
  }
  markBillsAsDelete(ids: number[]): Observable<Expense> {
    let url = this.baseService.apiUrl + '/bill/MarkBillsAsDelete';
    return this.http.post<Expense>(url, ids, { headers: this.baseService.getRequestHeader(true) });
  }
}