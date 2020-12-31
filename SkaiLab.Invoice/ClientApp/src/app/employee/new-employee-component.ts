import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { Currency } from '../models/currency';
import { Employee } from '../models/employee';
import { Gender } from '../models/gender';
import { MaritalStatus } from '../models/marital-status';
import { SalaryType } from '../models/salary-type';
import { ParentComponent } from '../parentComponent';
import { CurrecyService } from '../service/currency-service';
import { EmployeeService } from '../service/employee-service';
import { FileService } from '../service/file-service';
import { GenderService } from '../service/gender-service';
import { MaritalStatusService } from '../service/marital-status-service';
import { SalaryTypeService } from '../service/salary-type-service';
import $ from "jquery";
import { CountryService } from '../service/country-service';
import { Country } from '../models/country';
import { TranslateService } from '@ngx-translate/core';
@Component({
    selector: 'new-employee-component',
    templateUrl: './new-employee-component.html'
})
export class NewEmployeeComponent extends ParentComponent implements OnInit {
    @ViewChild('f', { static: true }) form: NgForm;
    employee:Employee=new Employee();
    currencies:Currency[]=[];
    salaryTypes:SalaryType[]=[];
    genders:Gender[]=[];
    maritalStatuses:MaritalStatus[]=[];
    countries:Country[]=[];
    constructor(private employeeService: EmployeeService,
        private currencyService:CurrecyService,
        private salaryTypeService:SalaryTypeService,
        private genderService:GenderService,
        private router: Router,
        private fileService:FileService,
        private translate: TranslateService,
        private countryService:CountryService,
        private maritalStatusService:MaritalStatusService
        ) {
        super("New Employee");
        this.ShowBackButton();
        this.setPageTitleFromLocalise(this.translate,"newEmployee");
      
    }
    ngOnInit(): void {
        this.currencyService.getOrganisationCurrencies().subscribe(result=>{
            this.currencies=result;
            let c =new Currency();
            c.id=null;
            c.name=this.selectText();
            this.currencies.splice(0, 0, c);
        },err=>{
            this.handleError(err);
        })
        this.salaryTypeService.gets().subscribe(result=>{
            this.salaryTypes=result;
            let c =new SalaryType();
            c.id=null;
            c.name=this.selectText();
            this.salaryTypes.splice(0, 0, c);
        },err=>{
            this.handleError(err);
        })
        this.genderService.gets().subscribe(result=>{
            this.genders=result;
            let c =new Gender();
            c.id=null;
            c.name=this.selectText();
            this.genders.splice(0, 0, c);
           
        },err=>{
            this.handleError(err);
        })
        this.maritalStatusService.gets().subscribe(result=>{
            this.maritalStatuses=result;
            let c =new MaritalStatus();
            c.id=null;
            c.name=this.selectText();
            this.maritalStatuses.splice(0, 0, c);
        },err=>{
            this.handleError(err);
        })
        this.countryService.gets().subscribe(result=>{
            this.countries=result;
        })
        $('#dateOfBirth').find('.wc-date-container').children('span').hide();
    }
    fileChange(event: any) {
        if (event) {
          let self = this;
          var reader = new FileReader();
          reader.readAsDataURL(event.target.files[0]);
          reader.onload = function () {
             self.showProgressBar();
             let fileName=event.target.files[0].name;
            self.fileService.upload(reader.result.toString(),fileName).subscribe(result=>{
                self.employee.documentUrl=result.url;
                self.hideProgressBar();
             },err=>{
                 self.handleError(err);
             })
          };
        }
      };
    onSave(isClose:boolean){
        if (this.form.invalid) {
            return;
        }
        this.showProgressBar();
        this.employeeService.add(this.employee).subscribe(result=>{
            this.hideProgressBar();
            if(isClose){
                this.router.navigate(['/employee'])
            }
            else{
                this.employee=new Employee();
            }
        },err=>{
            this.handleError(err);
        })
      }
      errorCurrency(){
         return this.form.submitted && this.employee.currencyId==null;
      }
      errorGender(){
        return this.form.submitted && this.employee.genderId==null;
     }
     errorMaritalStatusGender(){
        return this.form.submitted && this.employee.maritalStatusId==null;
     }
     erroSalaryType(){
        return this.form.submitted && this.employee.salaryTypeId==null;
     }
     onNameChange(){
         this.employee.displayName=this.employee.firstName + " "+this.employee.lastName;
     }
     onDateOfBirthDateChange(event:any){
        $('#dateOfBirth').find('.wc-date-container').children('span').show();
    }
}

