import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import 'rxjs/add/operator/map';
import { BaseService } from './base-service';
import { Organisation } from '../models/organisation';

@Injectable()
export class OrganisationService {
  constructor(private http: HttpClient,private baseService:BaseService) { }
  get():Observable<Organisation>{
    let url = this.baseService.apiUrl + '/Organisation/get';
    return this.http.get<Organisation>(url,{headers:this.baseService.getRequestHeader(true)});
  }
}