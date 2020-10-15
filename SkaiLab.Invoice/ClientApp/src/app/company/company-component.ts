import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ParentComponent } from '../parentComponent';

@Component({
  selector: 'company-component',
  templateUrl: './company-component.html'
})
export class CompanyComponent  implements OnInit {
  constructor(private router:Router,private parentComponent:ParentComponent){

  }
  ngOnInit(): void {

  }

}

