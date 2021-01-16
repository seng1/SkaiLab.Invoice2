import { TranslateService } from '@ngx-translate/core';
import $ from 'jquery';
import { DatePickerOption } from './models/date-picker-option';
import { DropdownConfig } from './models/dropdown-config';
import { MultipleDropDownSetting } from './models/MultipleDropDownSetting';
import { Page } from './models/page';
import { Permission } from './models/permission';
import { Utility } from './models/utility';
import { MenuService } from './service/menu-service';
import { PrintService } from './service/print-service';

export class ParentComponent {
    protected id: any = "";
    pager: Page = new Page();
    private ALERT_TITLE: string = "Invoice";
    private ALERT_BUTTON_TEXT: string = "Close";
    dropdownConfig: DropdownConfig = new DropdownConfig();
    datePickerOption: DatePickerOption = new DatePickerOption();
    multipleDropDownSetting: MultipleDropDownSetting = new MultipleDropDownSetting();
    permission: Permission = new Permission();
    dropdownSettings = {};
    constructor(title: string) {
        this.hideProgressBar();
        this.setPageTitle(title);
        this.hideBackButton();
    }
    hideProgressBar() {
        Utility.hideProgressBar();
    }
    showProgressBar() {
        Utility.showProgressBar();
    }
    setPageTitle(title: string) {
        $("#title").html(title);
        $("title").html("Skailab | " + localStorage.getItem("organisationName") + " | " + title);
    }
    hideBackButton() {
        $("#backButton").hide();
    }
    getPermission(menuService: MenuService) {
        menuService.getPermission().subscribe(result => {
            this.permission = result;
        });
    }
    openFileFromUrl(fileUrl:string){
        Utility.download_file(fileUrl);
    }
    ShowBackButton() {
        $("#backButton").show();
    }
    handleError(error: any) {
        this.hideProgressBar();
        if (error.error != undefined && error.error.errorText != undefined) {
            this.showErrorText(error.error.errorText);
            return;
        }
        if (error.error != undefined && error.status != undefined) {
            this.showErrorText("Error: " + error.status + ": " + error.error.split('at')[0].replace("System.Exception:", ""));
            return;
        }

        this.showErrorText(error.split('at')[0]);
    }
    showErrorText(errrorText: string) {
        this.showErrorTexts([errrorText]);
    }
    getTotalAmount(baseCurrencyAmount: number, toCurrencyExchangeRate): number {
        return Math.round((baseCurrencyAmount * (1 / toCurrencyExchangeRate)) * 100) / 100;
    }
    isKhmer():boolean{
        return Utility.isKhmer();
    }
    showErrorTexts(errrorText: string[]) {
        let d = document;
        if (d.getElementById("modalContainer")) {
            document.getElementsByTagName("body")[0].removeChild(document.getElementById("modalContainer"));
        }
        if (d.getElementById("modalContainer")) return;
        let mObj = d.getElementsByTagName("body")[0].appendChild(d.createElement("div"));
        mObj.id = "modalContainer";
        mObj.style.height = d.documentElement.scrollHeight + "px";
        let alertObj = mObj.appendChild(d.createElement("div"));
        alertObj.id = "alertBox";
        let parentWidth = $(d.getElementsByTagName("body")).width();
        let elementWidth = $(alertObj).width();
        alertObj.style.left = (parentWidth / 2 - elementWidth / 2) + "px";
        alertObj.style.top = (200) + "px";
        let h1 = alertObj.appendChild(d.createElement("h1"));
        h1.appendChild(d.createTextNode(this.ALERT_TITLE));
        let ul = alertObj.appendChild(d.createElement("ul"));
        ul.style.marginTop = "10px";
        ul.style.marginLeft = "20px";
        ul.style.marginRight = "10px";
        ul.style.marginBottom = "10px";
        ul.className = "text-danger";
        errrorText.forEach(it => {
            let msg = ul.appendChild(d.createElement("li"));
            msg.innerHTML = it;
        });
        let btn = alertObj.appendChild(d.createElement("a"));
        btn.id = "closeBtn";
        btn.className = "pointer"
        btn.appendChild(d.createTextNode(this.ALERT_BUTTON_TEXT));
        btn.focus();
        btn.onclick = function () {
            document.getElementsByTagName("body")[0].removeChild(document.getElementById("modalContainer"));
            return false;
        }
        alertObj.style.display = "block";

    }
    print(printService: PrintService, id: any, reportTypeId: any, fileName: string) {
        this.showProgressBar();
        printService.print(id, reportTypeId, fileName).subscribe(result => {
            this.hideProgressBar();
            this.openFileFromUrl(result.result);
        }, err => {
            this.handleError(err);
        })
    }
    setPageTitleFromLocalise(translate: TranslateService, key: string) {
        translate.get("textTranslate."+key).subscribe(res => {
            this.setPageTitle(res);
        });
    }
    allText():string{
        if(this.isKhmer()){
            return "ទាំងអស់";
        }
        return "All"
    }
    selectText():string{
      return Utility.selectText();
    }
    searchProductOrServiceText():string{
        if(this.isKhmer()){
            return "ស្វែងរកទំនិញ / សេវាកម្ម ...";
        }
        return "Search product/service...";
    }
}
