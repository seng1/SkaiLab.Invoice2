export class Menu{
    name:string;
    iconClass:string;
    isActive:boolean;
    menuItems:Menu[];
    routeLink:string;
    constructor(){
        this.name="";
        this.iconClass="";
        this.isActive=false;
        this.menuItems=[];
        this.routeLink="";
    }
}