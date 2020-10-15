import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import 'rxjs/add/operator/map';
import { BaseService } from './base-service';
import { OrganisationType } from '../models/organisationType';

@Injectable()
export class OrganisationTypeService {
  constructor(private http: HttpClient,private baseService:BaseService) { }
  gets():Observable<OrganisationType[]>{
    let url = this.baseService.apiUrl + '/OrganisationType/gets';
    return this.http.get<OrganisationType[]>(url,{headers:this.baseService.getRequestHeader(true)});
  }
}