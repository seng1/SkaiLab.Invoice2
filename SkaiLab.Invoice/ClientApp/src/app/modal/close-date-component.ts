import { getInterpolationArgsLength } from '@angular/compiler/src/render3/view/util';
import { Component } from '@angular/core';
import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { Attachment } from '../models/attachment';
import { DatePickerOption } from '../models/date-picker-option';
import { Expense } from '../models/expense';
import { FileService } from '../service/file-service';

@Component({
  selector: 'close-date-component',
  templateUrl: './close-date-component.html',
})
export class CloseDateComponent {
  private modalRef:NgbModalRef;
  title:string="";
  hasCloseDoc:boolean=true;
  closeDate:Date=new Date();
  datePickerOption: DatePickerOption = new DatePickerOption();
  attachment:Attachment=new Attachment();
  showUploading:boolean=false;
  expenses:Expense[]=[];
  constructor(private fileService:FileService) {

  }
  close(){
      this.modalRef.dismiss();
  }
  onGo(){
      if(this.expenses.length>0){
        this.expenses.forEach(it=>{
          it.closeDate=new Date(this.closeDate);
        })
      }
      this.modalRef.close({"closeDate":this.closeDate,"attachment":this.attachment});
  }
  init(modalRef:NgbModalRef,title:string,hasCloseDoc:boolean){   
    this.modalRef=modalRef;
    this.title=title;
    this.hasCloseDoc=hasCloseDoc;
  }
  initExpense(modalRef:NgbModalRef,title:string,exepsens:Expense[]){   
    this.modalRef=modalRef;
    this.title=title;
    this.expenses=exepsens;
    this.hasCloseDoc=true;
  }
  fileChange(event: any) {
    if (event) {
      let self = this;
      var reader = new FileReader();
      reader.readAsDataURL(event.target.files[0]);
      reader.onload = function () {
        let fileName=event.target.files[0].name;
        self.showUploading=true;
        self.fileService.upload(reader.result.toString(),fileName).subscribe(result=>{
            self.attachment.fileUrl=result.url;
            self.attachment.fileName=fileName;
            self.attachment.isFinalOfficalFile=true;
            self.showUploading=false;
         },err=>{
            self.showUploading=false;
         })
      };
    }
  };
  fileExpenseChange(event:any,expense:Expense){
    if (event) {
      let self = this;
      var reader = new FileReader();
      reader.readAsDataURL(event.target.files[0]);
      reader.onload = function () {
        let fileName=event.target.files[0].name;
        expense.loading=true;
        self.fileService.upload(reader.result.toString(),fileName).subscribe(result=>{
            let attachment:Attachment=new Attachment();
            attachment.fileUrl=result.url;
            attachment.fileName=fileName;
            attachment.isFinalOfficalFile=true;
            expense.closeAttachment=attachment;
            expense.loading=false;
         },err=>{
          expense.loading=false;
         })
      };
    }

  }
}
