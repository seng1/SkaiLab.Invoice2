import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthorizeService } from 'src/api-authorization/authorize.service';
import { Menu } from '../models/menu';
import { MenuService } from '../service/menu-service';

@Component({
  selector: 'nav-bar-component',
  templateUrl: './nav-bar-component.html'
})
export class NavBarComponent  implements OnInit {
  menus:Menu[]=[];
  menuLoaded:boolean=false;
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
}

