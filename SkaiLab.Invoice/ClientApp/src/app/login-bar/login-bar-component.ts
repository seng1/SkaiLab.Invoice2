import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { AuthorizeService } from 'src/api-authorization/authorize.service';
import { Permission } from '../models/permission';
import { User } from '../models/user';
import { MenuService } from '../service/menu-service';
import { UserService } from '../service/user-service';

@Component({
  selector: 'login-bar-component',
  templateUrl: './login-bar-component.html',
  styleUrls: ['./login-bar-component.css']
})
export class LoginBarComponent implements OnInit {
  loaded: boolean = false;
  user: User = new User();
  permission:Permission=new Permission();
  constructor(private authorize: AuthorizeService,private translate: TranslateService, private userService: UserService,private menuService:MenuService) {
   
  }
  ngOnInit(): void {
    this.authorize.isAuthenticated().subscribe(result => {
      if (result) {
        this.userService.GetLoginUser().subscribe(result => {
          this.user = result;
          this.loaded = true;
        })
        this.menuService.getPermission().subscribe(result=>{
          this.permission=result;
        })
      }
    })
  }
  getDisplayName(): String {
   let name:String="";
    if (this.user.name != null && this.user.name.length > 0) {
      name=this.user.name.split(' ')[0][0].toUpperCase();
      if(this.user.name.includes(' ')){
        name+=this.user.name.split(' ')[1][0].toUpperCase();
      }
      return name;
    }
    if(this.user.email!=null && this.user.email.length>0){
      return this.user.email.substring(0,1)[0][0].toUpperCase();
    }
    return name;
  }
  enClick(){
    this.user.language="en";
    this.changeLanguage();
  }
  khClick(){
    this.user.language="km-KH";
    this.changeLanguage();
  }
  changeLanguage(){
    this.translate.setDefaultLang(this.user.language);
    localStorage.setItem("language",this.user.language);
    this.userService.updateUserLanguage(this.user).subscribe(result=>{
      window.location.reload();
    });
  }
}

