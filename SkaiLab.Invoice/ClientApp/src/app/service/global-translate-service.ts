import { Injectable } from "@angular/core";
import { TranslateService } from "@ngx-translate/core";


@Injectable()
export class GlobaltranslateService {
  constructor(public translate: TranslateService) {}
  getTranslation(str) {
    const currentLang = this.translate.currentLang; // get current language
    const returnValue = this.translate.translations[currentLang][str]; // get converted string from current language
    if (returnValue === undefined) {
      return this.translate.translations.en_merch[str]; // if value is getting undefined then return default language string, en_merch is default english language file here
    } else {
      return returnValue;
    }
  }
}