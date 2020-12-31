import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import 'rxjs/add/operator/map';
import { BaseService } from './base-service';
import { Invoice, InvoiceStatus } from '../models/customer-transaction';
import { InvoiceFilter } from '../models/filter';
import { StatusOverview } from '../models/status-overview';
import { Attachment } from '../models/attachment';
@Injectable()
export class InvoiceService {
  constructor(private http: HttpClient, private baseService: BaseService) { }
  generateInvoiceFromQuote(id: any): Observable<Invoice> {
    let url = this.baseService.apiUrl + '/Invoice/GenerateInvoiceFromQuote/'+id;
    return this.http.get<Invoice>(url, { headers: this.baseService.getRequestHeader(true) });
  }
  GenerateInvoiceNumber(invoice: Invoice): Observable<Invoice> {
    invoice.date=new Date(invoice.date);
    if(invoice.dueDate!=null){
        invoice.dueDate=new Date(invoice.dueDate);
    }
    let newInvoice:Invoice=new Invoice();
    newInvoice.customerId=0;
    newInvoice.date=invoice.date;
    newInvoice.isTaxIncome=invoice.isTaxIncome;
    let url = this.baseService.apiUrl + '/Invoice/GenerateInvoiceNumber';
    return this.http.post<Invoice>(url,newInvoice, { headers: this.baseService.getRequestHeader(true) });
  }
  createInvoiceFromQuote(invoice: Invoice): Observable<Invoice> {
    invoice.date=new Date(invoice.date);
    if(invoice.dueDate!=null){
        invoice.dueDate=new Date(invoice.dueDate);
    }
    let url = this.baseService.apiUrl + '/Invoice/CreateInvoiceFromQuote';
    return this.http.post<Invoice>(url,invoice, { headers: this.baseService.getRequestHeader(true) });
  }
  create(invoice: Invoice): Observable<Invoice> {
    invoice.date=new Date(invoice.date);
    if(invoice.dueDate!=null){
        invoice.dueDate=new Date(invoice.dueDate);
    }
    let url = this.baseService.apiUrl + '/Invoice/Create';
    return this.http.post<Invoice>(url,invoice, { headers: this.baseService.getRequestHeader(true) });
  }
  getInvoices(filter: InvoiceFilter): Observable<Invoice[]> {
    if(filter.fromDate!=null){
      filter.fromDate=new Date(filter.fromDate);
    }
    if(filter.toDate!=null){
      filter.toDate=new Date(filter.toDate);
    }
    let url = this.baseService.apiUrl + '/Invoice/GetInvoices';
    return this.http.post<Invoice[]>(url,filter, { headers: this.baseService.getRequestHeader(true) });
  }
  getInvoiceStatuses(filter: InvoiceFilter): Observable<InvoiceStatus[]> {
    if(filter.fromDate!=null){
      filter.fromDate=new Date(filter.fromDate);
    }
    if(filter.toDate!=null){
      filter.toDate=new Date(filter.toDate);
    }
    let url = this.baseService.apiUrl + '/Invoice/GetInvoiceStatuses';
    return this.http.post<InvoiceStatus[]>(url,filter, { headers: this.baseService.getRequestHeader(true) });
  }
  getTotalPages(filter: InvoiceFilter): Observable<InvoiceFilter> {
    if(filter.fromDate!=null){
      filter.fromDate=new Date(filter.fromDate);
    }
    if(filter.toDate!=null){
      filter.toDate=new Date(filter.toDate);
    }
    let url = this.baseService.apiUrl + '/Invoice/GetTotalPages';
    return this.http.post<InvoiceFilter>(url,filter, { headers: this.baseService.getRequestHeader(true) });
  }
  getInvoice(id: any): Observable<Invoice> {
    let url = this.baseService.apiUrl + '/Invoice/GetInvoice/'+id;
    return this.http.get<Invoice>(url, { headers: this.baseService.getRequestHeader(true) });
  }
  getOverView(): Observable<StatusOverview[]> {
    let url = this.baseService.apiUrl + '/Invoice/GetOverView';
    return this.http.get<StatusOverview[]>(url, { headers: this.baseService.getRequestHeader(true) });
  }
  pay(id:any): Observable<Invoice> {
    let url = this.baseService.apiUrl + '/Invoice/Pay/'+id;
    return this.http.get<Invoice>(url, { headers: this.baseService.getRequestHeader(true) });
  }
  payAll(ids:number[]): Observable<Invoice> {
    let url = this.baseService.apiUrl + '/Invoice/PayAll';
    return this.http.post<Invoice>(url,ids, { headers: this.baseService.getRequestHeader(true) });
  }
  uploadFile(id:any,attachment:Attachment): Observable<Attachment> {
    let url = this.baseService.apiUrl + '/Invoice/UploadFile/'+id;
    return this.http.post<Attachment>(url,attachment, { headers: this.baseService.getRequestHeader(true) });
  }
  changeOfficialDocument(id:any,fileUrl:string):Observable<File>{
    let url = this.baseService.apiUrl + '/Invoice/ChangeOfficialDocument/'+id+"?fileUrl="+fileUrl;
    return this.http.get<File>(url,{headers:this.baseService.getRequestHeader(true)});
  }
}
