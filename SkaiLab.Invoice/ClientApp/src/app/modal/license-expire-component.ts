import { Component } from '@angular/core';
import { NgbModalRef } from '@ng-bootstrap/ng-bootstrap';

@Component({
    selector: 'license-expire-component',
    templateUrl: './license-expire-component.html',
})
export class LicenseExpireModalComponent {
    private modalRef: NgbModalRef;
    purchaseUrl:string="";
    constructor() {
        this.purchaseUrl = document.location.protocol +'//'+ document.location.host+"/MakePayment?culture="+this.getLanguage();
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
}
