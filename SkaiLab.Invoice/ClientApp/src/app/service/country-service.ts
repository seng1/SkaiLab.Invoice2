import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import 'rxjs/add/operator/map';
import { BaseService } from './base-service';
import { Country } from '../models/country';

@Injectable()
export class CountryService {
  constructor(private http: HttpClient,private baseService:BaseService) { }
  gets():Observable<Country[]>{
    let url = this.baseService.apiUrl + '/Country/Gets';
    return this.http.get<Country[]>(url);
  }
 
}