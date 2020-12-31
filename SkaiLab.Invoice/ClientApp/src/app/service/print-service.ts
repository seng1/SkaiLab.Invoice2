import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import 'rxjs/add/operator/map';
import { BaseService } from './base-service';
import { PrintData } from '../models/print';

@Injectable()
export class PrintService {
  constructor(private http: HttpClient,private baseService:BaseService) { }
  print(id:any,reportTypeId:any,fileName:string):Observable<PrintData>{
    let url = this.baseService.apiUrl + '/print/print/'+id+"/"+reportTypeId+"?fileName="+fileName;
    return this.http.get<PrintData>(url,{headers:this.baseService.getRequestHeader(true)});
  }
  printReceipt(id:any,fileName:string,purpose:string):Observable<PrintData>{
    let url = this.baseService.apiUrl + '/print/PrintReceipt/'+id+"?fileName="+fileName+"&purpose="+purpose;
    return this.http.get<PrintData>(url,{headers:this.baseService.getRequestHeader(true)});
  }
  printPayslip(id:any,fileName:string):Observable<PrintData>{
    let url = this.baseService.apiUrl + '/print/printPayslip/'+id+"?fileName="+fileName;
    return this.http.get<PrintData>(url,{headers:this.baseService.getRequestHeader(true)});
  }
  printPayslips(month:any,ids:number[]):Observable<PrintData>{
    let url = this.baseService.apiUrl + '/print/printPayslips/'+month;
    return this.http.post<PrintData>(url,ids);
  }
  downloadTax(month:any):Observable<PrintData>{
    let url = this.baseService.apiUrl + '/Download/DownloadTax/'+month;
    return this.http.get<PrintData>(url);
  }

}