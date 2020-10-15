import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ParentComponent } from '../parentComponent';
import { OrganisationTypeService } from '../service/organisation-type-service';

@Component({
  selector: 'company-component',
  templateUrl: './company-component.html'
})
export class CompanyComponent  implements OnInit {
  constructor(private router:Router,private parentComponent:ParentComponent,private organisationTypeService:OrganisationTypeService){

  }
  ngOnInit(): void {
    this.organisationTypeService.gets().subscribe(result=>{
      console.log(result);
    });
  }

}

