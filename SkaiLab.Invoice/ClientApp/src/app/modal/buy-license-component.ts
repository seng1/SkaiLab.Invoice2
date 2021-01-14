import { Component, OnInit } from '@angular/core';
import { NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { Plan } from '../models/plan';
import { PlanService } from '../service/planservice';

@Component({
    selector: 'buy-license-component',
    templateUrl: './buy-license-component.html',
    styleUrls: ['./buy-license-component.css'],
})
export class BuyLicenseModalComponent  implements OnInit{
    private modalRef: NgbModalRef;
    purchaseUrl:string="";
    yearSelected:boolean=true;
    basicPlan:Plan=new Plan();
    standardPlan:Plan=new Plan();
    preiumPlan:Plan=new Plan();
    showLoading:boolean=false;
    constructor(private planService:PlanService) {
        this.purchaseUrl = document.location.protocol +'//'+ document.location.host+"/MakePayment?culture="+this.getLanguage();
    }
    ngOnInit(): void {
        this.planService.gets().subscribe(result=>{
            console.log(result);
            this.basicPlan=result[0];
            this.standardPlan=result[1];
            this.preiumPlan=result[2];
        })
    }
    close() {
        this.modalRef.dismiss();
    }
    init(modalRef: NgbModalRef,userId:string) {
        this.modalRef = modalRef;
        this.purchaseUrl+="&id="+userId;
    }
    getLanguage(){
        if(localStorage.getItem("language")==null || localStorage.getItem("language")=="en"){
          return "en-US";
        }
        return localStorage.getItem("language");
      }
      onSubscriptionTypeChange(){
          this.yearSelected=!this.yearSelected;
      }
      onBuy(plan:Plan){
          this.showLoading=true;
          this.planService.createUserPlan(plan.id,this.yearSelected).subscribe(result=>{
            window.location.href=this.purchaseUrl;
          },err=>{
              alert(err);
          })
      }
}
