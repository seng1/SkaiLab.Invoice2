import { Country } from "./country";
import { Currency } from "./currency";
import { Gender } from "./gender";
import { MaritalStatus } from "./marital-status";
import { SalaryType } from "./salary-type";

export class Employee{
    id:number;
    firstName:string;
    lastName:string;
    displayName:string;
    dateOfBirth?:Date;
    genderId:number;
    gender:Gender;
    phoneNumber:string;
    email:string;
    address:string;
    maritalStatusId:number;
    maritalStatus:MaritalStatus;
    salaryTypeId:number;
    salaryType:SalaryType;
    isResidentEmployee:boolean;
    iDOrPassportNumber:string;
    documentUrl:string;
    organisationId:string;
    salary:number;
    numberOfChild:number;
    currencyId:number;
    currency:Currency;
    isActive:boolean;
    jobTitle:string;
    countryId:number;
    country:Country;
    isConfederationThatHosts:boolean;
    constructor(){
        this.id=0;
        this.firstName="";
        this.lastName="";
        this.displayName="";
        this.dateOfBirth=null;
        this.genderId=null;
        this.gender=new Gender();
        this.phoneNumber="";
        this.email="";
        this.address="";
        this.maritalStatusId=null;
        this.maritalStatus=new MaritalStatus();
        this.salaryTypeId=null;
        this.salaryType=new SalaryType();
        this.isResidentEmployee=true;
        this.iDOrPassportNumber="";
        this.documentUrl="";
        this.organisationId="";
        this.salary=0;
        this.numberOfChild=0;
        this.currencyId=0;
        this.currency=new Currency();
        this.isActive=true;
        this.jobTitle="";
        this.countryId=39;
        this.isConfederationThatHosts=false;
       
    }
}