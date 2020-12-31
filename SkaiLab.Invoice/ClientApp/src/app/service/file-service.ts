import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import 'rxjs/add/operator/map';
import { BaseService } from './base-service';
import { File } from '../models/file';
import { Attachment } from '../models/attachment';

@Injectable()
export class FileService {
  constructor(private http: HttpClient,private baseService:BaseService) { }
  upload(baseString:string,fileName:string):Observable<File>{
    let url = this.baseService.apiUrl + '/File/Upload';
    let file:Attachment=new Attachment();
    file.fileUrl=baseString;
    file.fileName=fileName;
    return this.http.post<File>(url,file,{headers:this.baseService.getRequestHeader(true)});
  }
}