import { Utility } from "./utility";

export class DropdownConfig{
    displayKey:string;
    search:boolean;
    height:string;
    placeholder:string;
    customComparator:any;
    limitTo:number;
    moreText:string;
    noResultsFound:string;
    searchPlaceholder:string;
    searchOnKey:string;
    clearOnSelection:boolean;
    inputDirection:string;
    constructor(){
        this.displayKey="name";
        this.search=true;
        this.height="auto";
        this.customComparator=()=>{};
        this.limitTo=0;
        this.moreText="more";
        this.noResultsFound="No results found!";
        this.searchPlaceholder="Search";
        this.searchOnKey="name";
        this.clearOnSelection=true;
        this.inputDirection="ltr";
        if(Utility.isKhmer()){
            this.searchPlaceholder="ស្វែងរក";
            this.noResultsFound="រកមិនឃើញលទ្ធផល!";
            this.moreText="ច្រើនទៀត";
        }
    }
}