export class OrganisationInvoiceSetting{
    id:string;
    termAndConditionForQuote:string;
    termAndConditionForInvoice:string;
    termAndConditionForPurchaseOrder:string;
    constructor(){
        this.id="";
        this.termAndConditionForInvoice="";
        this.termAndConditionForQuote="";
        this.termAndConditionForPurchaseOrder="";
    }
}