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
    legalLocalName:string;
    contactId?:string;
    contact?:Contact;
    organisationType?:OrganisationType;
    declareTax:boolean;
    organisationBaseCurrency:OrganisationBaseCurrency;
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
        this.legalLocalName="";
        this.contactId=null;
        this.contact=new Contact();
        this.organisationType=new OrganisationType();
        this.declareTax=true;
        this.organisationBaseCurrency=new OrganisationBaseCurrency();
    }
}
export class OrganisationBaseCurrency{
    baseCurrencyId:number;
    taxCurrencyId:number;
    taxExchangeRate:number;
    constructor(){
        this.baseCurrencyId=null;
        this.taxCurrencyId=null;
        this.taxExchangeRate=4000;
    }
}