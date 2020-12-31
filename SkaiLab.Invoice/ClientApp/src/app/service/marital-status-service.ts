import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import 'rxjs/add/operator/map';
import { BaseService } from './base-service';
import { MaritalStatus } from '../models/marital-status';

@Injectable()
export class MaritalStatusService {
  constructor(private http: HttpClient,private baseService:BaseService) { }
  gets():Observable<MaritalStatus[]>{
    let url = this.baseService.apiUrl + '/MaritalStatus/Gets';
    return this.http.get<MaritalStatus[]>(url,{headers:this.baseService.getRequestHeader(true)});
  }
}