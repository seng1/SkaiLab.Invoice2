import { HttpInterceptor, HttpRequest, HttpHandler, HttpUserEvent, HttpEvent } from "@angular/common/http";
import 'rxjs/add/operator/do';
import { Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { Observable } from "rxjs";
import { UserService } from "../service/user-service";

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

    constructor(private router: Router,private userService:UserService) { }
    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        if (req.headers.get('No-Auth') == "True")
            return next.handle(req.clone());
        if(this.userService.isAuthenticate()){
            const clonedreq = req.clone({
                headers: req.headers.set("Authorization", "Bearer " + localStorage.getItem('token'))
            });
            return next.handle(clonedreq)
                .do(
                succ => { },
                err => {
                    if (err.status === 401)
                        this.router.navigateByUrl('/user/login');
                }
                );
        }
        else{
            this.router.navigateByUrl('/user/login');
        }
    }
}