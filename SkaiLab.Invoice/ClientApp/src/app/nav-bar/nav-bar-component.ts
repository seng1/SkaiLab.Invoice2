import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { User } from '../models/user';
import { UserService } from '../service/user-service';

@Component({
  selector: 'nav-bar-component',
  templateUrl: './nav-bar-component.html'
})
export class NavBarComponent  implements OnInit {
  constructor(private router:Router,private userService:UserService){

  }
  ngOnInit(): void {

  }
}

