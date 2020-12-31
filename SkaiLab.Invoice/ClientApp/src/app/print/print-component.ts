import { Component} from '@angular/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { PrintService } from '../service/print-service';
import {$} from 'jquery';
@Component({
    selector: 'print-component',
    templateUrl: './print-component.html'
})
export class PrintComponent {
    title:string;
    report:SafeResourceUrl;
    private modalRef:NgbModalRef;
    constructor(private printService:PrintService,private sanitized: DomSanitizer) {
    }
    init(reportId:number,printDocumentTypeId:number,modalRef:NgbModalRef,title:string){
        this.modalRef=modalRef;
        this.title=title;
        this.printService.print(reportId,printDocumentTypeId,title).subscribe(result=>{
            this.report=this.sanitized.bypassSecurityTrustResourceUrl(result.result);;
        },err=>{
            alert(err);
        });
    }
    printReceipt(reportId:number,modalRef:NgbModalRef,title:string,purpose:string){
        this.modalRef=modalRef;
        this.title=title;
        this.printService.printReceipt(reportId,title,purpose).subscribe(result=>{
            this.report=this.sanitized.bypassSecurityTrustResourceUrl(result.result);;
        },err=>{
            alert(err);
        });
    }
    close(){
        this.modalRef.close();
    }
}
