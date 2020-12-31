import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { OrganisationUser } from '../models/organisation-user';
import { ParentComponent } from '../parentComponent';
import { OrganisationUserService } from '../service/organisation-user-service';

@Component({
    selector: 'update-user-component',
    templateUrl: './update-user-component.html'
})
export class UpdateUserComponent extends ParentComponent implements OnInit {
    @ViewChild('f', { static: true }) form: NgForm;
    private email:string="";
    organisationUser: OrganisationUser = new OrganisationUser();
    constructor(private organisationUserService: OrganisationUserService,
        private router: Router,
        private translate: TranslateService,
        private route: ActivatedRoute,
        ) {
        super("Update User");
        this.ShowBackButton();
        this.setPageTitleFromLocalise(this.translate,"updateUser");
        this.router.events.subscribe((evt) => {
            if (evt instanceof NavigationEnd) {
                this.router.navigated = false;
                var param = this.route.snapshot.params;
                this.email = param.email.replace(":", "");
                
            }
        });
    }
    ngOnInit(): void {
        this.showProgressBar();
        this.organisationUserService.getOrganisationUser(this.email).subscribe(result=>{
            this.hideProgressBar();
            this.organisationUser=result;
            console.log(this.organisationUser);
        },err=>{
            this.handleError(err);
        })
    }

    onSave() {
        if(!this.organisationUser.isAdministrator){
            let permissionHasCheck:boolean=false;
            this.organisationUser.menuFeatures.forEach(it=>{
                if(it.isCheck){
                    permissionHasCheck=true;
                }
            })
            if(!permissionHasCheck){
                this.showErrorText("Please select permission feature");
                return;
            }
         
        }
        if(this.organisationUser.isAdministrator){
            this.organisationUser.menuFeatures.forEach(it=>{
                it.isCheck=true;
            })
        }
        this.showProgressBar();
        this.organisationUserService.updateUserRole(this.organisationUser).subscribe(result=>{
            this.hideProgressBar();
            this.router.navigate(['/user']);
        },err=>{
            this.handleError(err);
        })
    }
    onRoleChange(isAdministrator: boolean) {
        this.organisationUser.isAdministrator = isAdministrator;
        this.organisationUser.menuFeatures.forEach(it => {
            it.isCheck = isAdministrator;
        })
        if(isAdministrator){
            this.organisationUser.roleName="Administrator";
        }
        else{
            this.organisationUser.roleName="Custom Role";
        }

    }
    onRemove(){
        if(confirm("Are you sure to remove this user?")) {
            this.showProgressBar();
            this.organisationUserService.removeUser(this.organisationUser.user.email).subscribe(result=>{
                this.router.navigate(['/user']);
            },err=>{
                this.handleError(err);
            })
        }
    }
}

