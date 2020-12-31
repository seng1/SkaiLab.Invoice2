import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import 'rxjs/add/operator/map';
import { BaseService } from './base-service';
import { Quote, QuoteForUpdateOrCreate, QuoteStatus } from '../models/quote';
import { QuoteFilter } from '../models/filter';
import {File} from '../models/file';
import { StatusOverview } from '../models/status-overview';
import { Attachment } from '../models/attachment';
@Injectable()
export class QuoteService {
  constructor(private http: HttpClient, private baseService: BaseService) { }
  gets(filter: QuoteFilter): Observable<Quote[]> {
    filter.fromDate = new Date(filter.fromDate);
    filter.toDate = new Date(filter.toDate);
    let url = this.baseService.apiUrl + '/Quote/Gets';
    return this.http.post<Quote[]>(url, filter, { headers: this.baseService.getRequestHeader(true) });
  }
  getTotalPages(filter: QuoteFilter): Observable<QuoteFilter> {
    filter.fromDate = new Date(filter.fromDate);
    filter.toDate = new Date(filter.toDate);
    let url = this.baseService.apiUrl + '/Quote/GetTotalPages';
    return this.http.post<QuoteFilter>(url, filter, { headers: this.baseService.getRequestHeader(true) });
  }
  getQuoteStatuses(filter: QuoteFilter): Observable<QuoteStatus[]> {
    filter.fromDate = new Date(filter.fromDate);
    filter.toDate = new Date(filter.toDate);
    let url = this.baseService.apiUrl + '/Quote/GetQuoteStatuses';
    return this.http.post<QuoteStatus[]>(url, filter, { headers: this.baseService.getRequestHeader(true) });
  }
  getLookupForCreae(): Observable<QuoteForUpdateOrCreate> {
    let url = this.baseService.apiUrl + '/Quote/GetLookupForCreae';
    return this.http.get<QuoteForUpdateOrCreate>(url, { headers: this.baseService.getRequestHeader(true) });
  }
  add(quote: Quote): Observable<Quote> {
    quote.date=new Date(quote.date);
    if(quote.expireDate!=null){
      quote.expireDate=new Date(quote.expireDate);
    }
    let url = this.baseService.apiUrl + '/Quote/Add';
    return this.http.post<Quote>(url, quote, { headers: this.baseService.getRequestHeader(true) });
  }
  update(quote: Quote): Observable<Quote> {
    quote.date=new Date(quote.date);
    if(quote.expireDate!=null){
      quote.expireDate=new Date(quote.expireDate);
    }
    let url = this.baseService.apiUrl + '/Quote/update';
    return this.http.post<Quote>(url, quote, { headers: this.baseService.getRequestHeader(true) });
  }
  getForUpdate(id:any): Observable<QuoteForUpdateOrCreate> {
    let url = this.baseService.apiUrl + '/Quote/GetForUpdate/'+id;
    return this.http.get<QuoteForUpdateOrCreate>(url, { headers: this.baseService.getRequestHeader(true) });
  }
  removeAttachment(id:any,fileUrl:string):Observable<File>{
    let url = this.baseService.apiUrl + '/Quote/RemoveAttachment/'+id+"?fileUrl="+fileUrl;
    return this.http.get<File>(url,{headers:this.baseService.getRequestHeader(true)});
  }
  addAttachment(id:any,fileUrl:string,fileName:string):Observable<File>{
    let file:Attachment=new Attachment();
    file.fileUrl=fileUrl;
    file.fileName=fileName;
    let url = this.baseService.apiUrl + '/Quote/AddAttachment/'+id;
    return this.http.post<File>(url,file,{headers:this.baseService.getRequestHeader(true)});
  }
  acceptAll(ids:number[]):Observable<Quote>{
    let url = this.baseService.apiUrl + '/Quote/AcceptAll';
    return this.http.post<Quote>(url,ids,{headers:this.baseService.getRequestHeader(true)});
  }
  declineAll(ids:number[]):Observable<Quote>{
    let url = this.baseService.apiUrl + '/Quote/DeclineAll';
    return this.http.post<Quote>(url,ids,{headers:this.baseService.getRequestHeader(true)});
  }
  deleteAll(ids:number[]):Observable<Quote>{
    let url = this.baseService.apiUrl + '/Quote/DeleteAll';
    return this.http.post<Quote>(url,ids,{headers:this.baseService.getRequestHeader(true)});
  }
  getOverView(): Observable<StatusOverview[]> {
    let url = this.baseService.apiUrl + '/Quote/GetOverView';
    return this.http.get<StatusOverview[]>(url, { headers: this.baseService.getRequestHeader(true) });
  }
  changeOfficialDocument(id:any,fileUrl:string):Observable<File>{
    let url = this.baseService.apiUrl + '/Quote/ChangeOfficialDocument/'+id+"?fileUrl="+fileUrl;
    return this.http.get<File>(url,{headers:this.baseService.getRequestHeader(true)});
  }
}