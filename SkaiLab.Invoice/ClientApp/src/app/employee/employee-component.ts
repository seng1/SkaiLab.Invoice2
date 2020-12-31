import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { Employee } from '../models/employee';
import { ParentComponent } from '../parentComponent';
import { EmployeeService } from '../service/employee-service';
@Component({
    selector: 'employee-component',
    templateUrl: './employee-component.html'
})
export class EmployeeComponent extends ParentComponent implements OnInit {
    employees:Employee[]=[];
    searchText:string="";
    constructor(private employeeService: EmployeeService,
        private translate: TranslateService) {
        super("Employees");
        this.ShowBackButton();
        this.setPageTitleFromLocalise(this.translate,"employees");
    }
    ngOnInit(): void {
       this.showProgressBar();
       this.employeeService.gets(this.searchText).subscribe(result=>{
        this.employees=result;
        this.hideProgressBar();
       },err=>{
        this.handleError(err);
       });
    }
 
}

