import { Data } from "@angular/router";
import { DateTypeFilter } from "./date-type-filter";
import { BillDateTypeFilterEnum, DashboardPeriodEnum, ExepnseStatusEnum, InvoiceDateTypeFilterEnum, PurchaseOrderDateTypeFilterEnum, QuoteEnum, QuoteFilterEnum } from "./enum";
import { DashboardPeriodFilter } from "./filter";

export class Utility {
    static getPurchaseDateTypeFilter(): DateTypeFilter[] {
        let result: DateTypeFilter[] = [];
        if(this.isKhmer()){
            result.push(new DateTypeFilter(PurchaseOrderDateTypeFilterEnum.All, "កាលបរិច្ឆេទណាមួយ"));
            result.push(new DateTypeFilter(PurchaseOrderDateTypeFilterEnum.Date, "កាលបរិច្ឆេទលើកឡើង"));
            result.push(new DateTypeFilter(PurchaseOrderDateTypeFilterEnum.DeliveryDate, "កាលបរិច្ឆេទដឹកជញ្ជូន"));
        }
        else{
            result.push(new DateTypeFilter(PurchaseOrderDateTypeFilterEnum.All, "Any date"));
            result.push(new DateTypeFilter(PurchaseOrderDateTypeFilterEnum.Date, "Date raised"));
            result.push(new DateTypeFilter(PurchaseOrderDateTypeFilterEnum.DeliveryDate, "Delivery date"));
        }
        return result;
    }
   static  isKhmer():boolean{
        if(localStorage.getItem("language")==null || localStorage.getItem("language")!="km-KH"){
            return false;
        }
        return true;
    }
    static getBillDateTypeFilter(): DateTypeFilter[] {
        let result: DateTypeFilter[] = [];
        if(this.isKhmer()){
            result.push(new DateTypeFilter(BillDateTypeFilterEnum.All, "កាលបរិច្ឆេទណាមួយ"));
            result.push(new DateTypeFilter(BillDateTypeFilterEnum.Date, "កាលបរិច្ឆេទលើកឡើង"));
            result.push(new DateTypeFilter(BillDateTypeFilterEnum.DueDate, "កាលបរិច្ឆេទ​កំណត់"));
        }
        else{
            result.push(new DateTypeFilter(BillDateTypeFilterEnum.All, "Any date"));
            result.push(new DateTypeFilter(BillDateTypeFilterEnum.Date, "Date raised"));
            result.push(new DateTypeFilter(BillDateTypeFilterEnum.DueDate, "Due date"));
        }
        return result;
    }
    static getQuoteDateTypeFilter(): DateTypeFilter[] {
        let result: DateTypeFilter[] = [];
        if(this.isKhmer()){
            result.push(new DateTypeFilter(QuoteFilterEnum.All, "កាលបរិច្ឆេទណាមួយ"));
            result.push(new DateTypeFilter(QuoteFilterEnum.Date, "កាលបរិច្ឆេទលើកឡើង"));
            result.push(new DateTypeFilter(QuoteFilterEnum.Expire, "ថ្ងៃ​ផុតកំណត់"));
        }
        else{
            result.push(new DateTypeFilter(QuoteFilterEnum.All, "Any date"));
            result.push(new DateTypeFilter(QuoteFilterEnum.Date, "Date raised"));
            result.push(new DateTypeFilter(QuoteFilterEnum.Expire, "Expiry date"));
        }
        
        return result;
    }
    static getInvoiceDateTypeFilter(): DateTypeFilter[] {
        let result: DateTypeFilter[] = [];
        if(this.isKhmer()){
            result.push(new DateTypeFilter(InvoiceDateTypeFilterEnum.All, "កាលបរិច្ឆេទណាមួយ"));
            result.push(new DateTypeFilter(InvoiceDateTypeFilterEnum.Date, "កាលបរិច្ឆេទលើកឡើង"));
            result.push(new DateTypeFilter(InvoiceDateTypeFilterEnum.DueDate, "កាលបរិច្ឆេទ​កំណត់"));
        }
        else{
            result.push(new DateTypeFilter(InvoiceDateTypeFilterEnum.All, "Any date"));
            result.push(new DateTypeFilter(InvoiceDateTypeFilterEnum.Date, "Date raised"));
            result.push(new DateTypeFilter(InvoiceDateTypeFilterEnum.DueDate, "Due date"));
        }
        return result;
    }
    static getDahboardPeriod(): DashboardPeriodFilter[] {
        let result: DashboardPeriodFilter[] = [];
        if(this.isKhmer()){
            result.push(new DashboardPeriodFilter(DashboardPeriodEnum.Last30Day, "៣០ ថ្ងៃចុងក្រោយ"));
            result.push(new DashboardPeriodFilter(DashboardPeriodEnum.ThisMonth, "ខែ​នេះ"));
            result.push(new DashboardPeriodFilter(DashboardPeriodEnum.ThisQuarter, "ត្រីមាសនេះ"));
            result.push(new DashboardPeriodFilter(DashboardPeriodEnum.ThisYear, "ឆ្នាំ​នេះ"));
            result.push(new DashboardPeriodFilter(DashboardPeriodEnum.LastMonth, "ខែមុន"));
            result.push(new DashboardPeriodFilter(DashboardPeriodEnum.LastQuater, "ត្រីមាសមុន"));
            result.push(new DashboardPeriodFilter(DashboardPeriodEnum.LastYear, "ឆ្នាំមុន"));
        }
        else{
            result.push(new DashboardPeriodFilter(DashboardPeriodEnum.Last30Day, "Last 30 days"));
        result.push(new DashboardPeriodFilter(DashboardPeriodEnum.ThisMonth, "This month"));
        result.push(new DashboardPeriodFilter(DashboardPeriodEnum.ThisQuarter, "This quarter"));
        result.push(new DashboardPeriodFilter(DashboardPeriodEnum.ThisYear, "This year"));
        result.push(new DashboardPeriodFilter(DashboardPeriodEnum.LastMonth, "Last month"));
        result.push(new DashboardPeriodFilter(DashboardPeriodEnum.LastQuater, "Last quarter"));
        result.push(new DashboardPeriodFilter(DashboardPeriodEnum.LastYear, "Last year"));
        }

       
        return result;
    }
    static getReportPerid():DashboardPeriodFilter[]{
        let result =this.getDahboardPeriod();
        if(this.isKhmer()){
            result.push( new DashboardPeriodFilter(DashboardPeriodEnum.Custom, "កែតម្រូរ"));
        }
        else{
            result.push( new DashboardPeriodFilter(DashboardPeriodEnum.Custom, "Custom"));
        }
      
        return result;
    }
    static beginningOfMonth(myDate: Date): Date {
        let date = new Date(myDate);
        date.setDate(1)
        date.setHours(16);
        date.setMinutes(10);
        date.setSeconds(1);
        return date;
    }
    static endOfMonth(myDate: Date): Date {
        let date = new Date(myDate);
        date.setMonth(date.getMonth() + 1)
        date.setDate(0);
        date.setHours(23);
        date.setMinutes(59);
        date.setSeconds(50);
        return date;
    }
    static getBeginQuaterFromDate(myDate: Date): Date {
       myDate.setDate(1);
        if (myDate.getMonth() <= 2) {
            myDate.setFullYear(myDate.getFullYear()-1);
            myDate.setMonth(0);
            myDate.setDate(1)
            return Utility.beginningOfMonth(myDate);
        }
        if (myDate.getMonth() <= 5) {
            myDate.setMonth(3);
            return Utility.beginningOfMonth(myDate);
        }
        if (myDate.getMonth() <= 8) {
            myDate.setMonth(6);
            return Utility.beginningOfMonth(myDate);
        }
        myDate.setMonth(9);
        return Utility.beginningOfMonth(myDate);
    }
    static getPreviousQuaterFromDate(myDate: Date): Date {
       var date=this.getBeginQuaterFromDate(myDate);
       date.setMonth(date.getMonth()-3);
       return date;
    }
    static totalText():string{
        if(this.isKhmer()){
            return "សរុប";
        }
        return "Total";
    }
    static subTotalText():string{
        if(this.isKhmer()){
            return "សរុបរង";
        }
        return "Sub Total";
    }
    static exchangeRateText():string{
        if(this.isKhmer()){
            return "អត្រា​ប្តូ​រ​ប្រាក់";
        }
        return "Exchange Rate";
    }
    static totalInKHRText():string{
        if(this.isKhmer()){
            return "អត្រា​ប្តូ​រ​ប្រាក់";
        }
        return "Total in KHR";
    }
    static selectProductToAddText():string{
        if(this.isKhmer()){
            return "អត្រា​ប្តូ​រ​ប្រាក់";
        }
        return "សូមជ្រើសរើសទំនិញដើម្បីបន្ថែម";
    }
    static customerRequireText():string{
        if(this.isKhmer()){
            return "អតិថិជនត្រូវតែមាន";
        }
        return "Customer is require";
    }
    static dateRequireText():string{
        if(this.isKhmer()){
            return "កាលបរិច្ឆេទត្រូវតែមាន";
        }
        return "Date is require";
    }
    static quoteItemRequireText():string{
        if(this.isKhmer()){
            return "Quote item ត្រូវតែមាន";
        }
        return "Quote item is require";
    }
    static quantityGreaterThenText(lineNumber:number, greater:number){
        if(this.isKhmer()){
            return "បរិមាណនៅក្នុងលេខជួរ " +lineNumber +" ត្រូវតែធំជាង"+greater;
        }
        return "quantity in line number "+lineNumber+" must greater then "+greater;
    }
    static unitPriceGreaterThenText(lineNumber:number, greater:number){
        if(this.isKhmer()){
            return "បរិមាណនៅក្នុងលេខជួរ " +lineNumber +" ត្រូវតែធំជាង"+greater;
        }
        return "Unit price in line "+lineNumber+" must greater then "+greater;
    }
    static discountRateMustBetween(lineNumber:number,from:number, to:number){
        if(this.isKhmer()){
            return "អត្រាបញ្ចុះតម្លៃក្នុងជួរ " +lineNumber +" ត្រូវតែចន្លោះ "+from + " និង "+to;
        }
        return "Discount rate in line number "+lineNumber+" must between  "+from +" and "+to;
    }
    static lineItemRequireText(){
        if(this.isKhmer()){
            return "ធាតុបន្ទាត់ត្រូវតែមាន";
        }
        return "Line item is require";
    }
    static locationInLineRequireText(lineNumber:number){
        if(this.isKhmer()){
            return "ទីតាំងធាតុនៅក្នុងលេខជួរ "+lineNumber + " ត្រូវតែមាន";
        }
        return "Item location in line number "+lineNumber + " is require.";
    }
    static vendorRequireText():string{
        if(this.isKhmer()){
            return "អ្នកលក់ត្រូវតែមាន";
        }
        return "Vendor is require";
    }
    static descriptionRequire():string
    {
        if(this.isKhmer()){
            return "បរិយាយត្រូវតែមាន";
        }
        return "Description is require";
    }
    static pleaseSelectQuoteText(){
        if(this.isKhmer()){
            return "សូមជ្រើសរើសquote";
        }
        return "Please select quote";
    }
    static moveAllQuoteToDeclineQ(){
        if(this.isKhmer()){
            return "តើអ្នកប្រាកដជាចង់ដកQuoteទាំងអស់នេះទៅបដិសេធឬ?";
        }
        return "Are you sure to move all these quotes to decline?";
    }
    static moveAllQuoteToDeleteQ(){
        if(this.isKhmer()){
            return "តើអ្នកប្រាកដជាលុបquoteទាំងអស់នេះឬ?";
        }
        return "Are you sure to delete all these quotes?";
    }
    static moveAllInvoiceToPaid(){
        if(this.isKhmer()){
            return "តើអ្នកប្រាកដជាចង់ផ្លាស់វិក្កយបត្រនេះទៅបង់ប្រាក់?";
        }
        return "Are you sure to move this invoice to paid?";
    }
    static noPurchaseSelectedText():string{
        if(this.isKhmer()){
            return "គ្មានការបញ្ជាទិញដែលបានជ្រើសរើសទេ";
        }
        return "No purchase order selected";
    }
    static movePurchasesToBillText():string{
        if(this.isKhmer()){
            return "តើអ្នកប្រាកដជាចង់ផ្លាស់ទីការបញ្ជាទិញនេះទៅបង់ប្រាក់រួចមែនទេ?";
        }
        return "Are you sure to move this purchase orders to bill?";
    }
    static deleteAllPurchaseText():string{
        if(this.isKhmer()){
            return "តើអ្នកប្រាកដជាលុបការបញ្ជាទិញនេះឬ?";
        }
        return "Are you sure to delete this purchase orders?";
    }
    static appovePurchaseText():string{
        if(this.isKhmer()){
            return "យល់ព្រមការបញ្ជាទិញ";
        }
        return "Approve Purchase order";
    }
   static noBillSelectedText():string{
        if(this.isKhmer()){
            return "គ្មានប៊ីលត្រូវបានជ្រើសរើស";
        }
        return "No bill selected";
    }
    static moveAllBillToBillQ():string{
        if(this.isKhmer()){
            return "តើអ្នកប្រាកដជាចង់ផ្លាស់ទីប៊ីលទំាងនេះទៅបង់ប្រាក់រួចមែនទេ?";
        }
        return "Are you sure to move this bill to billed?";
    }
    static deleteAllBillQ():string{
        if(this.isKhmer()){
            return "តើអ្នកប្រាកដជាលុបប៊ីលទាំងនេះឬ?";
        }
        return "Are you sure to delete this bills?";
    }
    static approveBillText():string{
        if(this.isKhmer()){
            return "យល់ព្រមការប៊ីល";
        }
        return "Approve Bill";
    }
    static deleteThisBillQ():string{
        if(this.isKhmer()){
            return "យល់ព្រមការប៊ីល";
        }
        return "Approve Bill";
    }
    static deleteQ(number:string){
        if(this.isKhmer()){
            return "តើអ្នកប្រាកដជាលុប "+number+"?";
        }
        return "Are you sure to delete this "+number+"?";
    }
   static selectText():string{
        if(this.isKhmer()){
            return "-- សូមជ្រើសរើស --";
        }
        return "-- Please Select --";
    }
}
export class QuoteUtility {
    static showPrintButton(statusId: number): boolean {
        return statusId != QuoteEnum.Declined;
    }
    static showSaveDrawButton(statusId: number): boolean {
        return statusId == QuoteEnum.Draft;
    }
    static showCreateInvoiceButton(statusId: number): boolean {
        return statusId == QuoteEnum.Accepted;
    }
    static allowSave(statusId: number): boolean {
        return statusId == QuoteEnum.Draft;
    }
}
export class PurchaseOrderUtility {
    static allowSubmitForApproval(statusId: number) {
        return statusId == ExepnseStatusEnum.Draft;
    }
    static allowSaveDraw(statusId: number) {
        return statusId == ExepnseStatusEnum.Draft;
    }
    static allowApprove(statusId: number) {
        return statusId == ExepnseStatusEnum.Draft || statusId == ExepnseStatusEnum.WaitingForApproval;
    }
    static allowSaveAndPrint(statusId: number) {
        return statusId == ExepnseStatusEnum.Draft || statusId == ExepnseStatusEnum.WaitingForApproval;
    }
    static allowDelete(statusId: number) {
        return statusId == ExepnseStatusEnum.Draft || statusId == ExepnseStatusEnum.WaitingForApproval;
    }
    static allowBill(statusId: number) {
        return statusId == ExepnseStatusEnum.Approved;
    }
}
export class BillUtility {
    static allowSubmitForApproval(statusId: number) {
        return statusId == ExepnseStatusEnum.Draft;
    }
    static allowSaveDraw(statusId: number) {
        return statusId == ExepnseStatusEnum.Draft;
    }
    static allowApprove(statusId: number) {
        return statusId == ExepnseStatusEnum.Draft || statusId == ExepnseStatusEnum.WaitingForApproval;
    }
    static allowSaveAndPrint(statusId: number) {
        return statusId == ExepnseStatusEnum.Draft || statusId == ExepnseStatusEnum.WaitingForApproval;
    }
    static allowDelete(statusId: number) {
        return statusId == ExepnseStatusEnum.Draft || statusId == ExepnseStatusEnum.WaitingForApproval;
    }
    static allowBill(statusId: number) {
        return statusId == ExepnseStatusEnum.Approved;
    }
}
export class ExpenseUtility {
    static allowSubmitForApproval(statusId: number) {
        return statusId == ExepnseStatusEnum.Draft;
    }
    static allowSaveDraw(statusId: number) {
        return statusId == ExepnseStatusEnum.Draft;
    }
    static allowApprove(statusId: number) {
        return statusId == ExepnseStatusEnum.Draft || statusId == ExepnseStatusEnum.WaitingForApproval;
    }
    static allowSaveAndPrint(statusId: number) {
        return statusId == ExepnseStatusEnum.Draft || statusId == ExepnseStatusEnum.WaitingForApproval;
    }
    static allowDelete(statusId: number) {
        return statusId == ExepnseStatusEnum.Draft || statusId == ExepnseStatusEnum.WaitingForApproval;
    }
    static allowBill(statusId: number) {
        return statusId == ExepnseStatusEnum.Approved;
    }
}