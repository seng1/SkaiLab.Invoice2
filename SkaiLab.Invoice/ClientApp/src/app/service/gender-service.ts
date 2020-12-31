import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import 'rxjs/add/operator/map';
import { BaseService } from './base-service';
import { Gender } from '../models/gender';

@Injectable()
export class GenderService {
  constructor(private http: HttpClient,private baseService:BaseService) { }
  gets():Observable<Gender[]>{
    let url = this.baseService.apiUrl + '/gender/Gets';
    return this.http.get<Gender[]>(url,{headers:this.baseService.getRequestHeader(true)});
  }
}