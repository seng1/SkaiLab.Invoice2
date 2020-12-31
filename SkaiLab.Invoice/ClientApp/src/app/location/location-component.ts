import { Component, OnInit, ViewChild } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { Location } from '../models/location';
import { ParentComponent } from '../parentComponent';
import { LocationService } from '../service/location-service';

@Component({
    selector: 'location-component',
    templateUrl: './location-component.html'
})
export class LocationComponent extends ParentComponent implements OnInit {
    locations: Location[] = [];
    constructor(private locationService: LocationService,
        private translate: TranslateService
        ) {
        super("Locations");
        this.ShowBackButton();
        this.setPageTitleFromLocalise(this.translate,"locations");
    }
    ngOnInit(): void {
        this.locationService.gets().subscribe(result=>{
            this.locations=result;
            this.hideProgressBar();
        },err=>{
            this.handleError(err);
        })
    }
}
