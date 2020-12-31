import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { User } from '../models/user';
import { UserService } from '../service/user-service';
import $ from 'jquery';
import { Organisation } from '../models/organisation';
import { AuthorizeService } from 'src/api-authorization/authorize.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ParentComponent } from '../parentComponent';
import { OrganisationService } from '../service/organisation-service';
@Component({
  selector: 'top-bar-component',
  templateUrl: './top-bar-component.html'
})
export class TopBarComponent extends ParentComponent  implements OnInit {
  user:User=new User();
  organisations:Organisation[]=[];
  workingOrganisationId:string="";
  looping:number=0;
  constructor(private router:Router,private authorize: AuthorizeService,private userService:UserService, private modalService: NgbModal,private organisationService:OrganisationService){
    super("");
    this.workingOrganisationId=this.userService.getWorkingOrganisationId();
  }
  ngOnInit(): void {
    this.authorize.isAuthenticated().subscribe(result=> {
      if(result){
        this.getOrganisations();
        this.getWorkingOrganisation();
      }
    })
  }
  getOrganisations(){
    this.userService.GetOrganisations().subscribe(result=>{
      this.organisations=result;
    
     },err=>{
      if(err.status=="401"){
        this.getOrganisations();
     }
     })
  }
  onOrganisationChange(){
    this.showProgressBar();
    this.organisationService.changeWorkingOrganisation(this.workingOrganisationId).subscribe(result=>{
      this.hideProgressBar();
      this.organisations.forEach(it=>{
        if(it.id==this.workingOrganisationId){
          this.userService.saveWorkingOrganisation(it);
        }
        window.location.reload();
      })
    },err=>{
      this.handleError(err);
    })
  }
  getWorkingOrganisation(){
    this.userService.getWorkingOrganisation().subscribe(result=>{
      if(this.workingOrganisationId!=result.id){
        this.workingOrganisationId=result.id;
        this.userService.saveWorkingOrganisation(result);
        window.location.reload();
      }
     },err=>{
      
       if(err.status=="401"){
          this.getWorkingOrganisation();
       }
    });
  }
  menuToggleClick(){
    var windowWidth = $(window).width();   		 
		if (windowWidth<1010) { 
			$('body').removeClass('open'); 
			if (windowWidth<760){ 
				$('#left-panel').slideToggle(); 
			} else {
				$('#left-panel').toggleClass('open-menu');  
			} 
		} else {
			$('body').toggleClass('open');
			$('#left-panel').removeClass('open-menu');  
		} 
  }

}

