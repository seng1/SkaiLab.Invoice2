import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import 'rxjs/add/operator/map';
import { BaseService } from './base-service';
import { Menu } from '../models/menu';
import { Permission } from '../models/permission';

@Injectable()
export class MenuService {
  constructor(private http: HttpClient,private baseService:BaseService) { }
  gets():Observable<Menu[]>{
    let url = this.baseService.apiUrl + '/Menu/Gets';
    return this.http.get<Menu[]>(url);
  }
  getPermission():Observable<Permission>{
    let url = this.baseService.apiUrl + '/Menu/GetPermission';
    return this.http.get<Permission>(url);
  }
 
}