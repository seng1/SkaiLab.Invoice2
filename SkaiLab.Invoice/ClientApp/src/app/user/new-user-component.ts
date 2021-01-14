import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { OrganisationUser } from '../models/organisation-user';
import { ParentComponent } from '../parentComponent';
import { OrganisationUserService } from '../service/organisation-user-service';

@Component({
    selector: 'new-user-component',
    templateUrl: './new-user-component.html'
})
export class NewUserComponent extends ParentComponent implements OnInit {
    @ViewChild('f', { static: true }) form: NgForm;
    constructor(private organisationUserService: OrganisationUserService,
        private translate: TranslateService,
        private router: Router) {
        super("Invite a User");
        this.ShowBackButton();
        this.setPageTitleFromLocalise(this.translate,"inviteaUser");
    }
    organisationUser: OrganisationUser = new OrganisationUser();
    ngOnInit(): void {
        this.showProgressBar();
        this.organisationUserService.getMenuFeatures().subscribe(result => {
            this.hideProgressBar();
            this.organisationUser.roleName = "Administrator";
            result.forEach(it => {
                it.isCheck = true;
            });
            this.organisationUser.menuFeatures = result;

        }, err => {
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
    onSave() {
        if (this.form.invalid) {
            return;
        }
        if(!this.organisationUser.isAdministrator){
            var hasSelectedFeature:boolean=false;
            this.organisationUser.menuFeatures.forEach(it=>{
                if(it.isCheck){
                    hasSelectedFeature=true;
                }
            })
            if(!hasSelectedFeature){
                this.showErrorText("Please select app feature");
                return;
            }
        }
        this.showProgressBar();
        this.organisationUserService.inviteUser(this.organisationUser).subscribe(result=>{
            this.hideProgressBar();
            this.router.navigate(['/user'])
        },err=>{
            this.handleError(err);
        })
    }
}

