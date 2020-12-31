import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import 'rxjs/add/operator/map';
import { BaseService } from './base-service';
import { SalaryType } from '../models/salary-type';

@Injectable()
export class SalaryTypeService {
  constructor(private http: HttpClient,private baseService:BaseService) { }
  gets():Observable<SalaryType[]>{
    let url = this.baseService.apiUrl + '/SalaryType/Gets';
    return this.http.get<SalaryType[]>(url,{headers:this.baseService.getRequestHeader(true)});
  }
}