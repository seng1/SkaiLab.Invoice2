import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import 'rxjs/add/operator/map';
import { BaseService } from './base-service';
import { CustomerCreditFilter } from '../models/filter';
import { CustomerCredit, CustomerCreditStatus } from '../models/customer-transaction';

@Injectable()
export class CustomerCreditService {
  constructor(private http: HttpClient, private baseService: BaseService) { }
  gets(filter: CustomerCreditFilter): Observable<CustomerCredit[]> {
    filter.fromDate = new Date(filter.fromDate);
    filter.toDate = new Date(filter.toDate);
    let url = this.baseService.apiUrl + '/CustomerCredit/Gets';
    return this.http.post<CustomerCredit[]>(url, filter, { headers: this.baseService.getRequestHeader(true) });
  }
  getTotalPages(filter: CustomerCreditFilter): Observable<CustomerCreditFilter> {
    filter.fromDate = new Date(filter.fromDate);
    filter.toDate = new Date(filter.toDate);
    let url = this.baseService.apiUrl + '/CustomerCredit/GetTotalPages';
    return this.http.post<CustomerCreditFilter>(url, filter, { headers: this.baseService.getRequestHeader(true) });
  }
  getStatuses(filter: CustomerCreditFilter): Observable<CustomerCreditStatus[]> {
    filter.fromDate = new Date(filter.fromDate);
    filter.toDate = new Date(filter.toDate);
    let url = this.baseService.apiUrl + '/CustomerCredit/GetStatuses';
    return this.http.post<CustomerCreditStatus[]>(url, filter, { headers: this.baseService.getRequestHeader(true) });
  }
  CreateNumber(invoice: CustomerCredit): Observable<CustomerCredit> {
    invoice.date=new Date(invoice.date);
    if(invoice.dueDate!=null){
        invoice.dueDate=new Date(invoice.dueDate);
    }
    let newInvoice:CustomerCredit=new CustomerCredit();
    newInvoice.customerId=0;
    newInvoice.date=invoice.date;
    newInvoice.isTaxIncome=invoice.isTaxIncome;
    let url = this.baseService.apiUrl + '/CustomerCredit/CreateNumber';
    return this.http.post<CustomerCredit>(url,newInvoice, { headers: this.baseService.getRequestHeader(true) });
  }
}