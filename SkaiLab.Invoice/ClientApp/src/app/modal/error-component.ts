import { Component } from '@angular/core';
import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'error-component',
  templateUrl: './error-component.html',
})
export class ErrorModalComponent {
  showProgressBar:Boolean=true;
  
  constructor(private modalService: NgbModal) {
    
  }
  setValues(errorTexts:string[]){
    //this.modalRef=modalRef;
  }
  close(){
      //this.modalRef.close();
  }
}
