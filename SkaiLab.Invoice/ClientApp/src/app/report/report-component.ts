import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { Organisation } from '../models/organisation';
import { ParentComponent } from '../parentComponent';
import { OrganisationService } from '../service/organisation-service';

@Component({
    selector: 'report-component',
    templateUrl: './report-component.html',
    styleUrls: ['./report-component.css'],
})
export class ReportComponent extends ParentComponent implements OnInit {
    declareTax:boolean=false;
    constructor(private organisationService:OrganisationService,
        private translate: TranslateService
        ) {
        super("Reports");
        this.ShowBackButton();
        this.setPageTitleFromLocalise(this.translate,"reports");
    }
    ngOnInit(): void {
        this.organisationService.get().subscribe(result=>{
            this.declareTax=result.declareTax;
        })
    }
  
}


