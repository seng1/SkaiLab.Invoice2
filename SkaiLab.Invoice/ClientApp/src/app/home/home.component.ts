import { Component, OnInit } from '@angular/core';
import { ParentComponent } from '../parentComponent';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent implements OnInit{
  constructor(private parentComponent:ParentComponent){
    this.parentComponent.hideProgressBar();
  }
  ngOnInit(): void {
  
  }
}
