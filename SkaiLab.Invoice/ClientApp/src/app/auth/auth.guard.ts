import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from "@angular/router";
import { UserService } from "../service/user-service";

@Injectable({
    providedIn:'root'
})
export class AuthGuard implements CanActivate{
    constructor(private router:Router,private userService:UserService){
    }
    canActivate(next: ActivatedRouteSnapshot, state: RouterStateSnapshot):boolean {
        if(this.userService.isAuthenticate()){
            return true;
        }
        else{
            this.router.navigate(['/user/login'])
        }
    }
    
}