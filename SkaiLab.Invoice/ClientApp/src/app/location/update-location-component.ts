import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { Location } from '../models/location';
import { ParentComponent } from '../parentComponent';
import { LocationService } from '../service/location-service';

@Component({
    selector: 'update-location-component',
    templateUrl: './update-location-component.html'
})
export class UpdateLocationComponent extends ParentComponent implements OnInit {
    @ViewChild('f', { static: true }) form: NgForm;
    location: Location = new Location();
    constructor(private router: Router, 
        private locationService: LocationService, 
        private translate: TranslateService,
        private route: ActivatedRoute) {
        super("Update Location");
        this.ShowBackButton();
        this.setPageTitleFromLocalise(this.translate,"updateLocation");
        this.router.events.subscribe((evt) => {
            if (evt instanceof NavigationEnd) {
                this.router.navigated = false;
                var param = this.route.snapshot.params;
                this.id = param.id.replace(":", "");
            }
        });
    }
    ngOnInit(): void {
        this.showProgressBar();
        this.locationService.get(this.id).subscribe(resutl=>{
            this.location=resutl;
            this.hideProgressBar();
        },err=>{
            this.handleError(err);
        })
    }
    onSave(isClose) {
        if (this.form.invalid) {
            return;
        }
        this.showProgressBar();
        this.locationService.update(this.location).subscribe(result => {
            this.hideProgressBar();
            if (isClose) {
                this.router.navigate(['/location'])
            }
            else {
                this.router.navigate(['/location-new'])
            }
        }, err => {
            this.handleError(err);
        })
    }
}
