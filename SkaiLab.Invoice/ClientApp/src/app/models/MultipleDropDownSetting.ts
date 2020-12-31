export class MultipleDropDownSetting 
{
    singleSelection: boolean;
    text:string;
    selectAllText:string;
    unSelectAllText:string;
    enableSearchFilter: boolean
    classes:string;
    labelKey:string;
    constructor(){
        this.singleSelection= false;
        this.text="Please select";
        this.selectAllText='Select All';
        this.unSelectAllText='UnSelect All';
        this.enableSearchFilter= true;
        this.classes="myclass custom-class";
        this.labelKey="name";
    }
}
