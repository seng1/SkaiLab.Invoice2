import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import 'rxjs/add/operator/map';
import { BaseService } from './base-service';
import { OrganisationUser } from '../models/organisation-user';
import { MenuFeature } from '../models/menu-feature';

@Injectable()
export class OrganisationUserService {
  constructor(private http: HttpClient,private baseService:BaseService) { }
  gets():Observable<OrganisationUser[]>{
    let url = this.baseService.apiUrl + '/OrganisationUser/GetOrganisationUsers';
    return this.http.get<OrganisationUser[]>(url);
  }
  getMenuFeatures():Observable<MenuFeature[]>{
    let url = this.baseService.apiUrl + '/OrganisationUser/GetMenuFeatures';
    return this.http.get<MenuFeature[]>(url);
  }
  inviteUser(organisationUser:OrganisationUser):Observable<OrganisationUser>{
    let url = this.baseService.apiUrl + '/OrganisationUser/inviteUser';
    return this.http.post<OrganisationUser>(url,organisationUser);
  }
  resentInviatation(email:any):Observable<OrganisationUser>{
    let url = this.baseService.apiUrl + '/OrganisationUser/ResentInviatation?email='+email;
    return this.http.get<OrganisationUser>(url);
  }
  getOrganisationUser(email:any):Observable<OrganisationUser>{
    let url = this.baseService.apiUrl + '/OrganisationUser/GetOrganisationUser?email='+email;
    return this.http.get<OrganisationUser>(url);
  }
  removeUser(email:any):Observable<OrganisationUser>{
    let url = this.baseService.apiUrl + '/OrganisationUser/RemoveUser?email='+email;
    return this.http.get<OrganisationUser>(url);
  }
  updateUserRole(organisationUser:OrganisationUser):Observable<OrganisationUser>{
    let url = this.baseService.apiUrl + '/OrganisationUser/UpdateUserRole';
    return this.http.post<OrganisationUser>(url,organisationUser);
  }
}