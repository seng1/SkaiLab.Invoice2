import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { Location } from '../models/location';
import { ParentComponent } from '../parentComponent';
import { LocationService } from '../service/location-service';

@Component({
    selector: 'new-location-component',
    templateUrl: './new-location-component.html'
})
export class NewLocationComponent extends ParentComponent implements OnInit {
    @ViewChild('f', { static: true }) form: NgForm;
    location: Location = new Location();
    constructor(private router: Router,
        private locationService: LocationService,
        private translate: TranslateService
        ) {
        super("New Location");
        this.ShowBackButton();
        this.setPageTitleFromLocalise(this.translate,"newLocation");
    }
    ngOnInit(): void {
    }
    onSave(isClose) {
        if (this.form.invalid) {
            return;
        }
        this.showProgressBar();
        this.locationService.add(this.location).subscribe(result=>{
            this.hideProgressBar();
            if(isClose){
                this.router.navigate(['/location'])
            }
            else{
                this.location=new Location();
            }
        },err=>{
            this.handleError(err);
        })
    }
}
