import { Component, OnInit, ViewChild } from "@angular/core";
import { NgForm } from "@angular/forms";
import { Router } from "@angular/router";
import { Login } from "../models/login";
import { UserService } from "../service/user-service";

@Component({
    selector: 'login-component',
    templateUrl: './login-component.html'
  })
  export class LoginComponent implements OnInit {
    login:Login=new Login();
    @ViewChild('f', { static: true }) form: NgForm;
    constructor(private router:Router,private userService:UserService){
        
    }
    ngOnInit() {
    }
    onLogin(){
        console.log(this.login);
        if(this.form.invalid){
            return;
        }
        this.userService.login(this.login).subscribe(result=>{
            this.userService.saveToStorage(result);
            this.router.navigate(['/'])
        });
    }
  }
  