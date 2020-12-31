import { Contact } from "./contact";
import { Currency } from "./currency";

export class Vendor{
    id:number;
    organisationId:string;
    legalName:string;
    displayName:string;
    currencyId:number;
    currency:Currency;
    taxNumber:string;
    businessRegistrationNumber:string;
    contactId:string;
    contact:Contact;
    localLegalName:string;
    constructor(){
        this.id=0;
        this.organisationId="";
        this.legalName="";
        this.displayName="";
        this.currencyId=0;
        this.currency=new Currency();
        this.taxNumber="";
        this.businessRegistrationNumber="";
        this.contactId="";
        this.contact=new Contact();
        this.localLegalName="";
    }
}