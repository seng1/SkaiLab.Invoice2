import { MenuFeature } from "./menu-feature";

export class OrganisationUser{
    organisationId:string;
    roleName:string;
    isInviting:boolean;
    isInvitingExpired:boolean;
    menuFeatures:MenuFeature[];
    isOwner:boolean;
    user:User;
    isAdministrator:boolean;
    constructor(){
        this.organisationId="";
        this.roleName="";
        this.isInviting=true;
        this.isInvitingExpired=false;
        this.menuFeatures=[];
        this.isOwner=false;
        this.user=new User();
        this.isAdministrator=true;
    }
}
export class User{

    firstName:string;
    lastName:string;
    id:string;
    email:string;
    name:string;
    phoneNumber:string;
    constructor(){
        this.firstName="";
        this.lastName="";
        this.id="";
        this.email="";
        this.name="";
        this.phoneNumber="";
    }
}