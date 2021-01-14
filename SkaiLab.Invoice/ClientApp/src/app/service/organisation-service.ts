import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import 'rxjs/add/operator/map';
import { BaseService } from './base-service';
import { Organisation } from '../models/organisation';
import { Currency } from '../models/currency';
import { ApiResult } from '../models/api-result';

@Injectable()
export class OrganisationService {
  constructor(private http: HttpClient,private baseService:BaseService) { }
  get():Observable<Organisation>{
    let url = this.baseService.apiUrl + '/Organisation/get';
    return this.http.get<Organisation>(url,{headers:this.baseService.getRequestHeader(true)});
  }
  update(organisation:Organisation):Observable<Organisation>{
    let url = this.baseService.apiUrl + '/Organisation/update';
    return this.http.post<Organisation>(url,organisation,{headers:this.baseService.getRequestHeader(true)});
  }
  getBaseCurrency():Observable<Currency>{
    let url = this.baseService.apiUrl + '/Organisation/GetBaseCurrency';
    return this.http.get<Currency>(url,{headers:this.baseService.getRequestHeader(true)});
  }
  getTaxCurrency():Observable<Currency>{
    let url = this.baseService.apiUrl + '/Organisation/GetTaxCurrency';
    return this.http.get<Currency>(url,{headers:this.baseService.getRequestHeader(true)});
  }
  add(organisation:Organisation):Observable<ApiResult>{
    let url = this.baseService.apiUrl + '/Organisation/Add';
    return this.http.post<ApiResult>(url,organisation,{headers:this.baseService.getRequestHeader(true)});
  }
  changeWorkingOrganisation(id:any):Observable<Organisation>{
    let url = this.baseService.apiUrl + '/Organisation/changeWorkingOrganisation/'+id;
    return this.http.get<Organisation>(url);
  }
  getOrganisationsWithSameBaseCurrency():Observable<Organisation[]>{
    let url = this.baseService.apiUrl + '/Organisation/GetOrganisationsWithSameBaseCurrency';
    return this.http.get<Organisation[]>(url,{headers:this.baseService.getRequestHeader(true)});
  }
}