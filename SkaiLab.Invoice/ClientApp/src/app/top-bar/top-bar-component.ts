import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { User } from '../models/user';
import { UserService } from '../service/user-service';
import $ from 'jquery';
@Component({
  selector: 'top-bar-component',
  templateUrl: './top-bar-component.html'
})
export class TopBarComponent  implements OnInit {
  user:User=new User();
  constructor(private router:Router,private userService:UserService){
  }
  ngOnInit(): void {
    this.userService.getUser().subscribe(result=>{
      this.user=result;
     })
  }
  signOut(){
    localStorage.removeItem("token");
    localStorage.removeItem("expireDate");
    this.router.navigate(['/user/login'])
  }
  menuToggleClick(){
    var windowWidth = $(window).width();   		 
		if (windowWidth<1010) { 
			$('body').removeClass('open'); 
			if (windowWidth<760){ 
				$('#left-panel').slideToggle(); 
			} else {
				$('#left-panel').toggleClass('open-menu');  
			} 
		} else {
			$('body').toggleClass('open');
			$('#left-panel').removeClass('open-menu');  
		} 
  }
}

