import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { AuthorizeService } from 'src/api-authorization/authorize.service';
import { User } from '../models/user';
import { ParentComponent } from '../parentComponent';
import { UserService } from '../service/user-service';

@Component({
  selector: 'my-profile-component',
  templateUrl: './my-profile-component.html'
})
export class MyProfileComponent extends ParentComponent implements OnInit {
 @ViewChild('f', { static: true }) form: NgForm;
  user: User = new User();
  oldName:string="";
  constructor(private userService: UserService,private authorizeService: AuthorizeService) {
    super("My Profile");
    this.ShowBackButton();
  }
  ngOnInit(): void {
    this.showProgressBar();
    this.userService.GetLoginUser().subscribe(result=>{
        this.user=result;
        this.hideProgressBar();
        this.oldName=this.user.name;
    },err=>{
        this.handleError(err);
    })
  }
  nameChange(){
      this.user.name=this.user.firstName+" "+this.user.lastName;
  }


  onSave(){
    if (this.form.invalid) {
        return;
    }
    this.showProgressBar();
    this.userService.updateLoginProfile(this.user).subscribe(result=>{
        this.hideProgressBar();
        if(this.oldName!=this.user.name){
            window.location.reload();
        }
    },err=>{
        this.handleError(err);
    })
  }
}

