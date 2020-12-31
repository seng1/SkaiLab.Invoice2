import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import 'rxjs/add/operator/map';
import { BaseService } from './base-service';
import { OrganisationInvoiceSetting } from '../models/organisation-invoice-setting';

@Injectable()
export class OrganisationInvoiceSettingService {
  constructor(private http: HttpClient,private baseService:BaseService) { }
  get():Observable<OrganisationInvoiceSetting>{
    let url = this.baseService.apiUrl + '/OrganisationInvoiceSetting/get';
    return this.http.get<OrganisationInvoiceSetting>(url,{headers:this.baseService.getRequestHeader(true)});
  }
  save(data:OrganisationInvoiceSetting):Observable<OrganisationInvoiceSetting>{
    let url = this.baseService.apiUrl + '/OrganisationInvoiceSetting/save';
    return this.http.post<OrganisationInvoiceSetting>(url,data,{headers:this.baseService.getRequestHeader(true)});
  }
}