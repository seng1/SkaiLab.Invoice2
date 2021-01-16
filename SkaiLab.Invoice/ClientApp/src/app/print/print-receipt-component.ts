import { Component} from '@angular/core';
import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { Invoice } from '../models/customer-transaction';
import { PrintService } from '../service/print-service';
import $ from 'jquery';
import { Utility } from '../models/utility';
@Component({
    selector: 'print-receipt-component',
    templateUrl: './print-receipt-component.html'
})
export class PrintReceiptComponent {
    private modalRef:NgbModalRef;
    purpose:string="";
    invoice:Invoice=new Invoice();
    constructor(private modalService: NgbModal,private printService:PrintService) {

    }
    init(invoice:Invoice,modalRef:NgbModalRef){
        this.modalRef=modalRef;
        this.invoice=invoice;
        this.purpose="Payment for invoice "+this.invoice.number;
    }
    close(){
        this.modalRef.close();
    }
    print(){
        this.modalRef.close();
        $("#preloader").show();
        this.printService.printReceipt(this.invoice.id,"Receipt-"+this.invoice.number,this.purpose).subscribe(result=>{
           Utility.download_file(result.result);
            $("#preloader").hide();
        },err=>{
            $("#preloader").hide();
        })
        
    }
}
