import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import 'rxjs/add/operator/map';
import { BaseService } from './base-service';
import { Login } from '../models/login';
import { Token } from '../models/token';
import { User } from '../models/user';

@Injectable()
export class UserService {
  constructor(private http: HttpClient,private baseService:BaseService) { }

 login(login:Login): Observable<Token> {
    let url = this.baseService.apiUrl + '/account/login';
    return this.http.post<Token>(url,login,{headers:this.baseService.getRequestHeader(false)});
  }
  saveToStorage(token:Token){
    localStorage.setItem("token",token.accessToken);
    var date=new Date().setHours(new Date().getHours()+token.validHour);
    localStorage.setItem("expireDate",date.toString());
  }
  
  getUser(){
    let url = this.baseService.apiUrl + '/account/GetUser';
    return this.http.get<User>(url,{headers:this.baseService.getRequestHeader(true)});
  }
  isAuthenticate():boolean{
    if(localStorage.getItem("token")==null){
      return false;
    }
    if(localStorage.getItem("expireDate")==null){
      return false;
    }
    var date=new Date(localStorage.getItem("expireDate"));
      if(date<new Date()){
        return false;
      }
      return true;
    }
}