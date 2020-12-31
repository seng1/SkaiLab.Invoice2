import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import 'rxjs/add/operator/map';
import { BaseService } from './base-service';
import { PieChart } from '../models/chart';
import { DashboardFilter } from '../models/filter';

@Injectable()
export class ChartService {
  constructor(private http: HttpClient,private baseService:BaseService) { }
  getProfiteAndLost(filter:DashboardFilter):Observable<PieChart>{
    let url = this.baseService.apiUrl + '/Chart/GetProfiteAndLost';
    return this.http.post<PieChart>(url,filter,{headers:this.baseService.getRequestHeader(true)});
  }
  getIncome(filter:DashboardFilter):Observable<PieChart>{
    let url = this.baseService.apiUrl + '/Chart/GetIncome';
    return this.http.post<PieChart>(url,filter,{headers:this.baseService.getRequestHeader(true)});
  }
  getExpense(filter:DashboardFilter):Observable<PieChart>{
    let url = this.baseService.apiUrl + '/Chart/GetExpense';
    return this.http.post<PieChart>(url,filter,{headers:this.baseService.getRequestHeader(true)});
  }
}