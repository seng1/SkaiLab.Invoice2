import { DOCUMENT } from "@angular/common";
import { HttpHeaders } from "@angular/common/http";
import { Inject } from "@angular/core";

export class BaseService{
     apiUrl:string;
     constructor(@Inject(DOCUMENT) private document: Document) {
        this.apiUrl = document.location.protocol +'//'+ document.location.host+"/api";
    }
     getRequestHeader(isRequireAuthenticate:boolean): HttpHeaders{
        if(isRequireAuthenticate){
            return new HttpHeaders({ 'Content-Type': 'application/json' });
        }
        return new HttpHeaders({ 'Content-Type': 'application/json','No-Auth':'True' });
    }
}