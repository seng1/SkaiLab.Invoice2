import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import 'rxjs/add/operator/map';
import { BaseService } from './base-service';
import { Employee } from '../models/employee';

@Injectable()
export class EmployeeService {
  constructor(private http: HttpClient,private baseService:BaseService) { }
  get(id:any):Observable<Employee>{
    let url = this.baseService.apiUrl + '/Employee/get/'+id;
    return this.http.get<Employee>(url,{headers:this.baseService.getRequestHeader(true)});
  }
  gets(searchText:string):Observable<Employee[]>{
    let url = this.baseService.apiUrl + '/Employee/gets?searchText='+searchText;
    return this.http.get<Employee[]>(url,{headers:this.baseService.getRequestHeader(true)});
  }
  add(employee:Employee):Observable<Employee>{
    let url = this.baseService.apiUrl + '/Employee/Add';
    if(employee.dateOfBirth!=null){
        employee.dateOfBirth=new Date(employee.dateOfBirth);
    }
    return this.http.post<Employee>(url,employee,{headers:this.baseService.getRequestHeader(true)});
  }
  update(employee:Employee):Observable<Employee>{
    let url = this.baseService.apiUrl + '/Employee/update';
    if(employee.dateOfBirth!=null){
        employee.dateOfBirth=new Date(employee.dateOfBirth);
    }
    return this.http.post<Employee>(url,employee,{headers:this.baseService.getRequestHeader(true)});
  }
}