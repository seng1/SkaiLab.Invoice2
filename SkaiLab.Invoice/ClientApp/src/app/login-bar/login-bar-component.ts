import { DOCUMENT } from '@angular/common';
import { Component, Inject, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { TranslateService } from '@ngx-translate/core';
import { AuthorizeService } from 'src/api-authorization/authorize.service';
import { NewCompanyComponent } from '../company/new-company-component';
import { Permission } from '../models/permission';
import { User } from '../models/user';
import { UserLicenseInformation } from '../models/user-license-information';
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
  purchaseUrl:string="";
  userLicenseInformation:UserLicenseInformation=new UserLicenseInformation();
  constructor(private authorize: AuthorizeService,
    private translate: TranslateService, 
    private userService: UserService,
    private modalService: NgbModal,
    @Inject(DOCUMENT) private document: Document,
      private menuService:MenuService) {
      this.purchaseUrl = document.location.protocol +'//'+ document.location.host+"/MakePayment?id=";
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
        this.userService.getUserLicenseInformation().subscribe(result=>{
          this.userLicenseInformation=result;
          this.purchaseUrl = document.location.protocol +'//'+ document.location.host+"/MakePayment?id="+result.userId+"&culture="+this.getLanguage();
        });
      }
    })
  }
  getLanguage(){
    if(localStorage.getItem("language")==null || localStorage.getItem("language")=="en"){
      return "en-US";
    }
    return localStorage.getItem("language");
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
  onHideTrail(){
    this.userLicenseInformation.isTrail=false;
  }
  onHideCompleteLicense(){
    this.userLicenseInformation.isUserCompleteLicense=true;
  }
  onCreateCompany(){
    const modalRef = this.modalService.open(NewCompanyComponent, { size: 'xl' as 'lg' });
    modalRef.componentInstance.init(modalRef);
  }
}

