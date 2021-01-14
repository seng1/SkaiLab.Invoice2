import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import 'rxjs/add/operator/map';
import { BaseService } from './base-service';
import { Plan } from '../models/plan';

@Injectable()
export class PlanService {
  constructor(private http: HttpClient,private baseService:BaseService) { }
  gets():Observable<Plan[]>{
    let url = this.baseService.apiUrl + '/plan/GetPlans';
    return this.http.get<Plan[]>(url);
  }
  createUserPlan(planId:any,isYearLyPay:boolean):Observable<Plan>{
    let url = this.baseService.apiUrl + '/plan/CreateUserPlan/'+planId+"?isYearLyPay="+isYearLyPay;
    return this.http.get<Plan>(url);
  }
}