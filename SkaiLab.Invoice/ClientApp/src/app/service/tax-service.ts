import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import 'rxjs/add/operator/map';
import { BaseService } from './base-service';
import { Tax } from '../models/tax';

@Injectable()
export class TaxService {
    constructor(private http: HttpClient, private baseService: BaseService) { }

    gets(): Observable<Tax[]> {
        let url = this.baseService.apiUrl + '/Tax/Gets';
        return this.http.get<Tax[]>(url, { headers: this.baseService.getRequestHeader(true) });
    }
    get(id:any): Observable<Tax> {
        let url = this.baseService.apiUrl + '/Tax/Get/'+id;
        return this.http.get<Tax>(url, { headers: this.baseService.getRequestHeader(true) });
    }
    create(tax: Tax): Observable<Tax> {
        let url = this.baseService.apiUrl + '/Tax/create';
        return this.http.post<Tax>(url, tax, { headers: this.baseService.getRequestHeader(true) });
    }
    update(tax: Tax): Observable<Tax> {
        let url = this.baseService.apiUrl + '/Tax/update';
        return this.http.post<Tax>(url, tax, { headers: this.baseService.getRequestHeader(true) });
    }
    getTaxesIncludeComponent(): Observable<Tax[]> {
        let url = this.baseService.apiUrl + '/Tax/GetTaxesIncludeComponent';
        return this.http.get<Tax[]>(url, { headers: this.baseService.getRequestHeader(true) });
    }
}