import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import 'rxjs/add/operator/map';
import { BaseService } from './base-service';
import { PurchaseFilter } from '../models/filter';
import { Expense, ExpenseForUpdate, ExpenseStatus } from '../models/expense';
import { PurchaseOrderLookup } from '../models/purchase-order-lookup';

@Injectable()
export class VendorCreditService {
  constructor(private http: HttpClient, private baseService: BaseService) { }
  gets(filter: PurchaseFilter): Observable<Expense[]> {
    filter.fromDate = new Date(filter.fromDate);
    filter.toDate = new Date(filter.toDate);
    let url = this.baseService.apiUrl + '/VendorCredit/Gets';
    return this.http.post<Expense[]>(url, filter, { headers: this.baseService.getRequestHeader(true) });
  }
  getTotalPages(filter: PurchaseFilter): Observable<PurchaseFilter> {
    filter.fromDate = new Date(filter.fromDate);
    filter.toDate = new Date(filter.toDate);
    let url = this.baseService.apiUrl + '/VendorCredit/GetTotalPages';
    return this.http.post<PurchaseFilter>(url, filter, { headers: this.baseService.getRequestHeader(true) });
  }
  getLookupForCreateOrUpdate(): Observable<PurchaseOrderLookup> {
    let url = this.baseService.apiUrl + '/VendorCredit/GetLookupForCreateOrUpdate';
    return this.http.get<PurchaseOrderLookup>(url, { headers: this.baseService.getRequestHeader(true) });
  }
  add(expense: Expense): Observable<Expense> {
    if (expense.deliveryDate != null) {
      expense.deliveryDate = new Date(expense.deliveryDate);
    }
    expense.date = new Date(expense.date);
    let url = this.baseService.apiUrl + '/VendorCredit/Create';
    return this.http.post<Expense>(url, expense, { headers: this.baseService.getRequestHeader(true) });
  }
  getExpenseForUpdate(id: any): Observable<ExpenseForUpdate> {
    let url = this.baseService.apiUrl + '/VendorCredit/GetExpenseForUpdate/' + id;
    return this.http.get<ExpenseForUpdate>(url, { headers: this.baseService.getRequestHeader(true) });
  }
  update(expense: Expense): Observable<Expense> {
    if (expense.deliveryDate != null) {
      expense.deliveryDate = new Date(expense.deliveryDate);
    }
    expense.date = new Date(expense.date);
    let url = this.baseService.apiUrl + '/VendorCredit/update';
    return this.http.post<Expense>(url, expense, { headers: this.baseService.getRequestHeader(true) });
  }
  markAsBill(expense: Expense): Observable<Expense> {
    let url = this.baseService.apiUrl + '/VendorCredit/MarkAsBill';
    return this.http.post<Expense>(url, expense, { headers: this.baseService.getRequestHeader(true) });
  }
  markDelete(expense: Expense): Observable<Expense> {
    if (expense.deliveryDate != null) {
      expense.deliveryDate = new Date(expense.deliveryDate);
    }
    expense.date = new Date(expense.date);
    let url = this.baseService.apiUrl + '/VendorCredit/MarkDelete';
    return this.http.post<Expense>(url, expense, { headers: this.baseService.getRequestHeader(true) });
  }
  marksAsWaitingForApproval(ids: number[]): Observable<Expense> {
    let url = this.baseService.apiUrl + '/VendorCredit/MarksAsWaitingForApproval';
    return this.http.post<Expense>(url, ids, { headers: this.baseService.getRequestHeader(true) });
  }
  marksAsApprove(ids: number[]): Observable<Expense> {
    let url = this.baseService.apiUrl + '/VendorCredit/MarksAsApprove';
    return this.http.post<Expense>(url, ids, { headers: this.baseService.getRequestHeader(true) });
  }
  marksAsBill(ids: number[]): Observable<Expense> {
    let url = this.baseService.apiUrl + '/VendorCredit/MarksAsBill';
    return this.http.post<Expense>(url, ids, { headers: this.baseService.getRequestHeader(true) });
  }
  marksAsDelete(ids: number[]): Observable<Expense> {
    let url = this.baseService.apiUrl + '/VendorCredit/MarksAsDelete';
    return this.http.post<Expense>(url, ids, { headers: this.baseService.getRequestHeader(true) });
  }
  getExpenseStatuses(filter:PurchaseFilter):Observable<ExpenseStatus[]>{
    filter.fromDate=new Date(filter.fromDate);
    filter.toDate=new Date(filter.toDate);
    let url = this.baseService.apiUrl + '/VendorCredit/GetExpenseStatuses';
    return this.http.post<ExpenseStatus[]>(url,filter,{headers:this.baseService.getRequestHeader(true)});
  }
}