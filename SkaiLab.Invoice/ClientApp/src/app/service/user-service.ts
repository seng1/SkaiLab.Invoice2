import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import 'rxjs/add/operator/map';
import { BaseService } from './base-service';
import { Token } from '../models/token';
import { Organisation } from '../models/organisation';
import { Observable } from 'rxjs';
import { User } from '../models/user';
import { UserLicenseInformation } from '../models/user-license-information';

@Injectable()
export class UserService {
  constructor(private http: HttpClient,private baseService:BaseService) { }
  saveToStorage(token:Token){
    localStorage.setItem("token",token.accessToken);
    var d =new Date();
    d.setHours(d.getHours()+token.validHour);
    localStorage.setItem("expireDate",d.toLocaleString());
  }
  GetOrganisations():Observable<Organisation[]>{
    let url = this.baseService.apiUrl + '/Account/GetOrganisations';
    return this.http.get<Organisation[]>(url,{headers:this.baseService.getRequestHeader(true)});
  }
  getWorkingOrganisation():Observable<Organisation>{
    let url = this.baseService.apiUrl + '/Account/GetWorkingOrganisation';
    return this.http.get<Organisation>(url,{headers:this.baseService.getRequestHeader(true)});
  }
  saveWorkingOrganisation(organisation:Organisation){
    localStorage.setItem("OrganisationId",organisation.id);
    localStorage.setItem("organisationName",organisation.displayName);
  }
  getWorkingOrganisationId(){
    if(localStorage.getItem("OrganisationId")==null){
      return null;
    }
    return localStorage.getItem("OrganisationId");
  }
  GetLoginUser():Observable<User>{
    let url = this.baseService.apiUrl + '/OrganisationUser/GetLoginUser';
    return this.http.get<User>(url);
  }
  updateLoginProfile(user:User):Observable<User>{
    let url = this.baseService.apiUrl + '/OrganisationUser/UpdateLoginProfile';
    return this.http.post<User>(url,user);
  }
  updateUserLanguage(user:User):Observable<User>{
    let url = this.baseService.apiUrl + '/OrganisationUser/UpdateUserLanguage';
    return this.http.post<User>(url,user);
  }
  getUserLicenseInformation():Observable<UserLicenseInformation>{
    let url = this.baseService.apiUrl + '/Account/GetUserLicenseInformation';
    return this.http.get<UserLicenseInformation>(url);
  }
}