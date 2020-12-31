import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import 'rxjs/add/operator/map';
import { BaseService } from './base-service';
import { ExpenseStatus } from '../models/expense';

@Injectable()
export class PurchaseOrderStatusService {
    constructor(private http: HttpClient,private baseService:BaseService) { }
    gets():Observable<ExpenseStatus[]>{
      let url = this.baseService.apiUrl + '/PurchaseOrderStatus/Gets';
      return this.http.get<ExpenseStatus[]>(url,{headers:this.baseService.getRequestHeader(true)});
    }
}