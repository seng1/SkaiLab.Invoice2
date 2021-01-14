export class ApiResult{
    code:number;
    errorText:string;
    isSccuess:boolean;
    userId:string;
    constructor(){
        this.code=0;
        this.errorText="";
        this.isSccuess=true;
        this.userId="";
    }
}