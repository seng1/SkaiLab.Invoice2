import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ParentComponent } from '../parentComponent';
import { OrganisationService } from '../service/organisation-service';
import { OrganisationTypeService } from '../service/organisation-type-service';

@Component({
  selector: 'company-component',
  templateUrl: './company-component.html'
})
export class CompanyComponent  implements OnInit {
  constructor(private router:Router,private parentComponent:ParentComponent,private organisationTypeService:OrganisationTypeService,private organisationService:OrganisationService){

  }
  ngOnInit(): void {
    this.organisationTypeService.gets().subscribe(result=>{
      console.log(result);
    });
    this.parentComponent.showProgressBar();
    this.organisationService.get().subscribe(result=>{
      console.log(result);
      this.parentComponent.hideProgressBar();
    },error=>{
      this.parentComponent.hideProgressBar();
    })
  }

}

