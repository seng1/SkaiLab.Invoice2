import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthorizeService } from 'src/api-authorization/authorize.service';
import { Menu } from '../models/menu';
import { Utility } from '../models/utility';
import { MenuService } from '../service/menu-service';

@Component({
  selector: 'nav-bar-component',
  templateUrl: './nav-bar-component.html'
})
export class NavBarComponent  implements OnInit {
  menus:Menu[]=[];
  menuLoaded:boolean=false;
  isExpand:boolean =false;
  constructor(private router:Router,private menuService:MenuService,private authorize: AuthorizeService){

  }
  ngOnInit(): void {
    this.authorize.isAuthenticated().subscribe(result=> {
      if(result){
        this.menuService.gets().subscribe(result=>{
          this.menus=result;
          this.menuLoaded=true;
        })
      }
    })
  }
  getParentMenuClass(menu:Menu):string{
    if(menu.menuItems.length==0 && menu.isActive){
       return "active pointer";
    }
    if(menu.menuItems.length>0 && menu.isActive){
      return "menu-item-has-children dropdown active pointer";
    }
    if(menu.menuItems.length>0){
      return "menu-item-has-children dropdown pointer";
    }
    return "nav-link-color pointer";
  }
  getSubmitMenuClass(menu:Menu):string{
    if(menu.isActive){
      return "active pointer";
    }
    return "nav-link-color pointer";
  }
  onNavigate(menu:Menu,parentMenu:Menu){
    if(menu.routeLink.length>0){
      this.menus.forEach(it=>{
        it.isActive=false;
        it.menuItems.forEach(t=>{
          t.isActive=false;
        })
      })
      menu.isActive=true;
      if(parentMenu!=null){
        parentMenu.isActive=true;
      }
      this.router.navigate([menu.routeLink])
    }
   
  }
  getSubMenuClass(index:number):string{
    if(index<this.menus.length-1){
      return "sub-menu children dropdown-menu";
    }
    if(Utility.isKhmer()){
      return "sub-menu children dropdown-menu khmer-sub-nav-last-menu";
    }
    return "sub-menu children dropdown-menu sub-nav-last-menu";
  }
  showSafariNoExpand():boolean{
    if(Utility.isSafari){
      if(localStorage.getItem("isNavExpand")==null || localStorage.getItem('isNavExpand')=="0"){
        return true;
      }
    }
    return false;
  }
}

