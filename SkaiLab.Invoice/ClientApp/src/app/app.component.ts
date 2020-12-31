import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { AuthorizeService } from 'src/api-authorization/authorize.service';
import { UserService } from './service/user-service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent implements OnInit {
  title = 'app';
  isAuthenticate:boolean=false;
  constructor(private translate: TranslateService,private authorize: AuthorizeService, private userService: UserService) {
    if(localStorage.getItem("language")==null){
      this.translate.setDefaultLang("en");
    }
    else{
      this.translate.setDefaultLang(localStorage.getItem("language"));
    }
}
ngOnInit(): void {
    this.authorize.isAuthenticated().subscribe(result => {
      if (result) {
        this.userService.GetLoginUser().subscribe(result => {
          if(localStorage.getItem("language")==null || localStorage.getItem("language")!=result.language){
            this.translate.setDefaultLang(result.language);
            localStorage.setItem("language",result.language);
            window.location.reload();
          }
        });
      }
    })
  }
}
