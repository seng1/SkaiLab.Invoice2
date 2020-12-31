import { Component, OnInit } from '@angular/core';
import { ParentComponent } from '../parentComponent';
import $ from "jquery";
@Component({
    selector: 'report-detail-component',
    template: '<div id="selector">Hi</div>'
})
export class ReportDetailComponent extends ParentComponent implements OnInit {
    constructor() {
        super("Report Detail");
        this.ShowBackButton();
       // $("#selector").append("<app-home></app-home>");

    }
    ngOnInit(): void {
     
    }
  
}


