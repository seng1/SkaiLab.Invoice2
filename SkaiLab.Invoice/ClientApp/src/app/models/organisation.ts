import { Contact } from "./contact";
import { OrganisationType } from "./organisationType";

export class Organisation{
    id:string;
    displayName:string;
    legalName:string;
    logoUrl:string;
    organisationTypeId?:number;
    lineBusiness:string;
    bussinessRegistrationNumber:string;
    description:string;
    taxNumber:string;
    taxDisplayName:string;
    contactId1?:string;
    contactId2?:string;
    contact1?:Contact;
    contact2?:Contact;
    organisationType?:OrganisationType;
    constructor(){
        this.id="";
        this.displayName="";
        this.legalName="";
        this.logoUrl="";
        this.organisationTypeId=null;
        this.lineBusiness="";
        this.bussinessRegistrationNumber="";
        this.description="";
        this.taxNumber="";
        this.taxDisplayName="";
        this.contactId1=null;
        this.contactId1=null;
        this.contact1=new Contact();
        this.contact2=new Contact();
        this.organisationType=new OrganisationType();
    }
}