import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { UserService } from './service/user-service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent implements OnInit {
  title = 'app';
  isAuthenticate:boolean=false;
  constructor(private userService:UserService,private router: Router,
    private route: ActivatedRoute,){
      
      this.router.events.subscribe((evt) => {
        if (evt instanceof NavigationEnd) {
          this.isAuthenticate=this.userService.isAuthenticate();
        }
      });
  }
  ngOnInit(): void {
    this.isAuthenticate=this.userService.isAuthenticate();
  }
}
