import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import 'rxjs/add/operator/map';
import { BaseService } from './base-service';
import { File } from '../models/file';
import { Attachment } from '../models/attachment';

@Injectable()
export class ExpenseService {
  constructor(private http: HttpClient,private baseService:BaseService) { }
  removeAttachment(id:any,fileUrl:string):Observable<File>{
    let url = this.baseService.apiUrl + '/Expens/RemoveAttachment/'+id+"?fileUrl="+fileUrl;
    return this.http.get<File>(url,{headers:this.baseService.getRequestHeader(true)});
  }
  addAttachment(id:any,fileUrl:string,fileName:string):Observable<File>{
    let file:Attachment=new Attachment();
    file.fileUrl=fileUrl;
    file.fileName=fileName;
    let url = this.baseService.apiUrl + '/Expens/AddAttachment/'+id;
    return this.http.post<File>(url,file,{headers:this.baseService.getRequestHeader(true)});
  }
  changeOfficialDocument(id:any,fileUrl:string):Observable<File>{
    let url = this.baseService.apiUrl + '/Expens/ChangeOfficialDocument/'+id+"?fileUrl="+fileUrl;
    return this.http.get<File>(url,{headers:this.baseService.getRequestHeader(true)});
  }
}