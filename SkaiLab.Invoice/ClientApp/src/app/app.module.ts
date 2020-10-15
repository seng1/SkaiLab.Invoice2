import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { NavBarComponent } from './nav-bar/nav-bar-component';
import { TopBarComponent } from './top-bar/top-bar-component';
import { FooterComponent } from './footer/footer-component';
import { BaseService } from './service/base-service';
import { UserService } from './service/user-service';
import { AuthInterceptor } from './auth/authInterceptor';
import { LoginComponent } from './login/login-component';
import { AuthGuard } from './auth/auth.guard';
import { CompanyComponent } from './company/company-component';
import { ParentComponent } from './parentComponent';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    NavBarComponent,
    TopBarComponent,
    FooterComponent,
    LoginComponent,
    CompanyComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full',canActivate:[AuthGuard] },
      { path: 'user/login', component: LoginComponent},
      { path: 'company', component: CompanyComponent, canActivate:[AuthGuard]},
    ])
  ],
  providers: [BaseService,UserService,ParentComponent,{
    provide:HTTP_INTERCEPTORS,
    useClass:AuthInterceptor,
    multi:true
  }],
  bootstrap: [AppComponent]
})
export class AppModule { }
