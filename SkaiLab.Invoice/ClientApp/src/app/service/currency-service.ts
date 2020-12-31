import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import 'rxjs/add/operator/map';
import { BaseService } from './base-service';
import { Currency } from '../models/currency';
import { NewCurrency } from '../models/new-currency';

@Injectable()
export class CurrecyService {
  constructor(private http: HttpClient,private baseService:BaseService) { }
  getOrganisationCurrencies():Observable<Currency[]>{
    let url = this.baseService.apiUrl + '/Currency/GetOrganisationCurrencies';
    return this.http.get<Currency[]>(url,{headers:this.baseService.getRequestHeader(true)});
  }
  getCurrenciesWithoutOrganisation():Observable<Currency[]>{
    let url = this.baseService.apiUrl + '/Currency/GetCurrenciesWithoutOrganisation';
    return this.http.get<Currency[]>(url,{headers:this.baseService.getRequestHeader(true)});
  }
  getCurrenciesWithoutNote():Observable<Currency[]>{
    let url = this.baseService.apiUrl + '/Currency/GetCurrenciesWithoutNote';
    return this.http.get<Currency[]>(url,{headers:this.baseService.getRequestHeader(true)});
  }
  getCurrenciesForCreate():Observable<NewCurrency>{
    let url = this.baseService.apiUrl + '/Currency/GetCurrenciesForCreate';
    return this.http.get<NewCurrency>(url,{headers:this.baseService.getRequestHeader(true)});
  }
  create(currency:Currency):Observable<Currency>{
    let url = this.baseService.apiUrl + '/Currency/Create';
    return this.http.post<Currency>(url,currency,{headers:this.baseService.getRequestHeader(true)});
  }
  getCurrencyWithExchangeRate(id:any):Observable<Currency>{
    let url = this.baseService.apiUrl + '/Currency/GetCurrencyWithExchangeRate/'+id;
    return this.http.get<Currency>(url,{headers:this.baseService.getRequestHeader(true)});
  }
  updateExchangeRate(currency:Currency):Observable<Currency>{
    let url = this.baseService.apiUrl + '/Currency/UpdateExchangeRate';
    return this.http.post<Currency>(url,currency,{headers:this.baseService.getRequestHeader(true)});
  }
  getCurrenciesWithExchangeRate():Observable<Currency[]>{
    let url = this.baseService.apiUrl + '/Currency/GetCurrenciesWithExchangeRate';
    return this.http.get<Currency[]>(url,{headers:this.baseService.getRequestHeader(true)});
  }
}