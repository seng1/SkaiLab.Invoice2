export class Permission{
    readWritePurchaseSale:boolean;
    readPurchaseSale:boolean;
    approvaPayPurchaseSale:boolean;
    manageOrganisactionSetting:boolean;
    manageAndInviteUser:boolean;
    payroll:boolean;
    report:boolean;
    constructor(){
        this.readWritePurchaseSale=false;
        this.readPurchaseSale=false;
        this.approvaPayPurchaseSale=false;
        this.manageOrganisactionSetting=false;
        this.manageAndInviteUser=false;
        this.payroll=false;
        this.report=false;
    }
}