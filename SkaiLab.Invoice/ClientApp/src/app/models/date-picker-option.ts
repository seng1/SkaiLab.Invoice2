export class DatePickerOption{
    format:string;
    bigBanner:boolean;
    defaultOpen:boolean;
    timePicker:boolean;
    closeOnSelect:boolean;
    constructor(){
        this.format="dd/MM/yyyy";
        this.bigBanner=true;
        this.defaultOpen=false;
        this.timePicker=false;
        this.closeOnSelect=true;
    }
}