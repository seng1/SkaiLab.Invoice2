import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { OrganisationUser } from '../models/organisation-user';
import { ParentComponent } from '../parentComponent';
import { OrganisationUserService } from '../service/organisation-user-service';

@Component({
    selector: 'user-component',
    templateUrl: './user-component.html'
})
export class UserComponent extends ParentComponent implements OnInit {
    constructor(private organisationUserService:OrganisationUserService,private translate: TranslateService){
        super("Users");
        this.ShowBackButton();
        this.setPageTitleFromLocalise(this.translate,"users");
    }
    organisationUsers:OrganisationUser[]=[];
    ngOnInit(): void {
        this.showProgressBar();
        this.organisationUserService.gets().subscribe(result=>{
            this.organisationUsers=result;
            this.hideProgressBar();
            console.log(this.organisationUsers);
        },err=>{
            this.handleError(err);
        });
    }
    onResendInviatation(user:OrganisationUser){
        this.showProgressBar();
        this.organisationUserService.resentInviatation(user.user.email).subscribe(result=>{
            user.isInviting=false;
            this.hideProgressBar();
        },err=>{    
            this.handleError(err);
        })
    }
    onRemove(user:OrganisationUser){
        if(confirm("Are you sure to remove this user?")) {
            this.showProgressBar();
            this.organisationUserService.removeUser(user.user.email).subscribe(result=>{
                this.organisationUsers=this.organisationUsers.filter(u=>u!=user);
                this.hideProgressBar();
            },err=>{
                this.handleError(err);
            })
        }
    }
}

