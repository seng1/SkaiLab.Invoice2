import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { TranslateService } from '@ngx-translate/core';
import { Contact } from '../models/contact';
import { Organisation } from '../models/organisation';
import { OrganisationType } from '../models/organisationType';
import { ParentComponent } from '../parentComponent';
import { OrganisationService } from '../service/organisation-service';
import { OrganisationTypeService } from '../service/organisation-type-service';
import { NewCompanyComponent } from './new-company-component';

@Component({
  selector: 'company-component',
  templateUrl: './company-component.html'
})
export class CompanyComponent extends ParentComponent implements OnInit {
  organisationTypes: OrganisationType[] = [];
  organisation: Organisation = new Organisation();
  @ViewChild('f', { static: true }) form: NgForm;
  constructor(private router: Router, private modalService: NgbModal,  
    private organisationTypeService: OrganisationTypeService, 
    private organisationService: OrganisationService,
    private translate: TranslateService) {
    super("Company");
    this.ShowBackButton();
    this.setPageTitleFromLocalise(this.translate,"organisation");
  }
  ngOnInit(): void {
    this.organisationTypeService.gets().subscribe(result => {
      this.organisationTypes = result;
    });
    this.showProgressBar();
    this.organisationService.get().subscribe(result => {
      this.organisation = result;
      if (this.organisation.contact == null) {
        this.organisation.contact = new Contact();
      }
      this.hideProgressBar();

    }, error => {
      this.handleError(error);
    });
  }
  fileChange(event: any) {
    if (event) {
      let self = this;
      var reader = new FileReader();
      reader.readAsDataURL(event.target.files[0]);
      reader.onload = function () {
        self.organisation.logoUrl = reader.result.toString();
      };
    }
  };
  onSave() {
    if (this.form.invalid) {
      return;
    }
    this.showProgressBar();
    this.organisationService.update(this.organisation).subscribe(result=>{
      this.hideProgressBar();
    },error=>{
      this.handleError(error);
    });
  }
  onNewCompanyClick(){
    const modalRef = this.modalService.open(NewCompanyComponent, { size: 'xl' as 'lg' });
    modalRef.componentInstance.init(modalRef);
}
}

