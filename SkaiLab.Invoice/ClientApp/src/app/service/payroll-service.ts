import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import 'rxjs/add/operator/map';
import { BaseService } from './base-service';
import { PayrollMonthNoneTax, PayrollMonthTax } from '../models/payroll';

@Injectable()
export class PayrollService {
  constructor(private http: HttpClient,private baseService:BaseService) { }
 
  getMonthTax(month:any):Observable<PayrollMonthTax>{
    let url = this.baseService.apiUrl + '/Payroll/GetMonthTax?month='+month;
    return this.http.get<PayrollMonthTax>(url);
  }
  createOrUpdatePayrollTax(month:any,payroll:PayrollMonthTax):Observable<PayrollMonthTax>{
    let url = this.baseService.apiUrl + '/Payroll/CreateOrUpdatePayrollTax/'+month;
    return this.http.post<PayrollMonthTax>(url,payroll);
  }
  getGetPayrollMonthNoneTax(month:any):Observable<PayrollMonthNoneTax>{
    let url = this.baseService.apiUrl + '/Payroll/GetGetPayrollMonthNoneTax?month='+month;
    return this.http.get<PayrollMonthNoneTax>(url);
  }
  createOrUpdatePayrollNoneTax(month:any,payroll:PayrollMonthNoneTax):Observable<PayrollMonthNoneTax>{
    let url = this.baseService.apiUrl + '/Payroll/CreateOrUpdatePayrollNoneTax/'+month;
    return this.http.post<PayrollMonthTax>(url,payroll);
  }
}