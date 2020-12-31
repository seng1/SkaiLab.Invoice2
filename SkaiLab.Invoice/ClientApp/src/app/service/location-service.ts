import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import 'rxjs/add/operator/map';
import { BaseService } from './base-service';
import { Location } from '../models/location';

@Injectable()
export class LocationService {
  constructor(private http: HttpClient,private baseService:BaseService) { }
  gets():Observable<Location[]>{
    let url = this.baseService.apiUrl + '/Location/Gets';
    return this.http.get<Location[]>(url,{headers:this.baseService.getRequestHeader(true)});
  }
  get(id:any):Observable<Location>{
    let url = this.baseService.apiUrl + '/Location/Get/'+id;
    return this.http.get<Location>(url,{headers:this.baseService.getRequestHeader(true)});
  }
  add(location:Location):Observable<Location>{
    let url = this.baseService.apiUrl + '/Location/Create';
    return this.http.post<Location>(url,location,{headers:this.baseService.getRequestHeader(true)});
  }
  update(location:Location):Observable<Location>{
    let url = this.baseService.apiUrl + '/Location/update';
    return this.http.post<Location>(url,location,{headers:this.baseService.getRequestHeader(true)});
  }

  
}