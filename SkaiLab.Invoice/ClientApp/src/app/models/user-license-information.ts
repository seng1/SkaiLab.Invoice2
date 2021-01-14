
import { User } from "./user";

export class UserLicenseInformation{
    userId:string;
    user:User;
    planId:number;
    expireDate:Date;
    subscriptionId:number;
    isExpire:boolean;
    isTrail:boolean;
    isUserHasLicense:boolean;
    isUserCompleteLicense:boolean;
    isAlertRenewLicense:boolean;
    constructor(){
       this.userId="";
       this.user=new User();
        this.planId=0;
        this.expireDate=new Date();
        this.subscriptionId=0;
        this.isExpire=false;
        this.isTrail=false;
        this.isUserHasLicense=false;
        this.isUserCompleteLicense=false;
        this.isAlertRenewLicense=false;
    }
}