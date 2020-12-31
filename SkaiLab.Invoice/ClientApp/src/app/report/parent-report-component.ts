import { Currency } from "../models/currency";
import { Organisation } from "../models/organisation";
import { ParentComponent } from "../parentComponent";
import { OrganisationService } from "../service/organisation-service";
import { UserService } from "../service/user-service";

export class ParentReportComponent extends ParentComponent{
  organisations: any[] = [];
  selectedOrganisations: Organisation[] = [];
  currency: Currency = new Currency();
  printDate:Date=new Date();
  organisation:Organisation=new Organisation();
  init(organisationService:OrganisationService,userService:UserService){
    organisationService.getOrganisationsWithSameBaseCurrency().subscribe(result => {
      this.organisations = result;
      this.organisations.forEach(it => {
        if (it.id == userService.getWorkingOrganisationId().toString()) {
          this.selectedOrganisations.push(it);
        }
      })
    })
    organisationService.getBaseCurrency().subscribe(result => {
      this.currency = result;
    });
    userService.getWorkingOrganisation()
  }
  getWorkingOrganisation(userService:UserService){
    userService.getWorkingOrganisation().subscribe(result=>{
      this.organisation=result;
    })
  }
  getSelectedOrganisationNames(){
    var names="";
    this.selectedOrganisations.forEach(it=>{
      if(names!=""){
        names+=";";
      }
      names+=it.displayName;
    })
    return names;
  }
  
}