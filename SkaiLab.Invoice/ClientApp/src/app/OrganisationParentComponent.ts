import { OnInit } from '@angular/core';
import { ParentComponent } from './parentComponent';
import { OrganisationService } from './service/organisation-service';

export class OrganisationParentComponent extends ParentComponent{
    declareTax:boolean=false;
    constructor(title: string,protected organisationService:OrganisationService) {
       super(title);
       this.getTaxDeclare();
    }
    private getTaxDeclare(){
        this.organisationService.get().subscribe(result=>{
            this.declareTax=result.declareTax;
        })
    }
}
