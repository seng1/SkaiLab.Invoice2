import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClient, HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { CustomFormsModule } from 'ng2-validation'
import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { NavBarComponent } from './nav-bar/nav-bar-component';
import { TopBarComponent } from './top-bar/top-bar-component';
import { FooterComponent } from './footer/footer-component';
import { BaseService } from './service/base-service';
import { UserService } from './service/user-service';
import { CompanyComponent } from './company/company-component';
import { OrganisationTypeService } from './service/organisation-type-service';
import { OrganisationService } from './service/organisation-service';
import { CurrecyService } from './service/currency-service';
import { CurrencyComponent } from './currency/currency-component';
import { NewCurrencyComponent } from './currency/new-currency-component';
import { UpdateCurrencyComponent } from './currency/update-currency-component';
import { VendorComponent } from './vendor/vendor-component';
import { VendorService } from './service/vendor-service';
import { NewVendorComponent } from './vendor/new-vendor-component';
import { UpdateVendorComponent } from './vendor/update-vendor-component';
import { CustomerComponent } from './customer/customer-component';
import { CustomerService } from './service/customer-service';
import { NewCustomerComponent } from './customer/new-customer-component';
import { UpdateCustomerComponent } from './customer/update-customer-component';
import { PagerService } from './service/page-service';
import { TaxRateComponent } from './tax-rate/tax-rate-component';
import { TaxService } from './service/tax-service';
import { NewTaxRateComponent } from './tax-rate/new-tax-component';
import { UpdateTaxRateComponent } from './tax-rate/update-tax-component';
import { ProductComponent } from './product/product-component';
import { ProductService } from './service/product-service';
import { NewProductComponent } from './product/new-product-component';
import { UpdateProductComponent } from './product/update-product-component';
import { PurchaseService } from './service/purchase-service';
import { PurchaseComponent } from './purchase/purchase-component';
import { PurchaseOrderStatusService } from './service/purchase-status-service';
import { SelectDropDownModule } from 'ngx-select-dropdown/dist/ngx-select-dropdown.module';
import { AngularDateTimePickerModule } from 'angular2-datetimepicker';
import { NewPurchaseComponent } from './purchase/new-purchase-component';
import { ErrorModalComponent } from './modal/error-component';
import { LocationService } from './service/location-service';
import { LocationComponent } from './location/location-component';
import { NewLocationComponent } from './location/new-location-component';
import { UpdateLocationComponent } from './location/update-location-component';
import { UpdatePurchaseComponent } from './purchase/update-purchase-component';
import { FileService } from './service/file-service';
import { ExpenseService } from './service/expense-service';
import { VendorBillComponent } from './vendor-bill/vendor-bill-component';
import { BillService } from './service/bill-service';
import { NewBillComponent } from './vendor-bill/new-bill-component';
import { UpdateBillComponent } from './vendor-bill/update-bill-component';
import { VendorExpenseService } from './service/vendor-expense-service';
import { VendorCreditService } from './service/vendor-credit-service';
import { QuoteService } from './service/quote-service';
import { QuoteComponent } from './quote/quote-component';
import { NewQuoteComponent } from './quote/new-quote-component';
import { UpdateQuoteComponent } from './quote/update-quote-component';
import { NewInvoiceFromQuoteComponent } from './invoice/new-invoice-quote-component';
import { InvoiceService } from './service/invoice-service';
import { LoadingComponent } from './loading/loading-component';
import { InvoiceComponent } from './invoice/invoice-component';
import { NewInvoiceComponent } from './invoice/new-invoice-component';
import { UpdateInvoiceFromQuoteComponent } from './invoice/update-invoice-component';
import { CustomerCreditService } from './service/customer-credit-service';
import { PrintComponent } from './print/print-component';
import { PrintService } from './service/print-service';
import { InvoiceSettingComponent } from './invoice-setting/invoice-setting-component';
import { OrganisationInvoiceSettingService } from './service/organisation-invoice-setting-service';
import { PurchaseOverViewComponent } from './purchase-overview/purchase-overview-component';
import { SaleOverViewComponent } from './invoice/invoice-overview-component';
import { PrintReceiptComponent } from './print/print-receipt-component';
import { GenderService } from './service/gender-service';
import { SalaryTypeService } from './service/salary-type-service';
import { MaritalStatusService } from './service/marital-status-service';
import { EmployeeService } from './service/employee-service';
import { EmployeeComponent } from './employee/employee-component';
import { NewEmployeeComponent } from './employee/new-employee-component';
import { UpdateEmployeeComponent } from './employee/update-employee-component';
import { AuthorizeGuard } from 'src/api-authorization/authorize.guard';
import { AuthorizeInterceptor } from 'src/api-authorization/authorize.interceptor';
import { ApiAuthorizationModule } from 'src/api-authorization/api-authorization.module';
import { NewCompanyComponent } from './company/new-company-component';
import { AngularMultiSelectModule } from 'angular2-multiselect-dropdown';
import { NgxChartsModule }from '@swimlane/ngx-charts';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ChartService } from './service/chart-service';
import { ChartsModule, ThemeService } from 'ng2-charts';
import {TranslateLoader, TranslateModule} from '@ngx-translate/core';
import {TranslateHttpLoader} from '@ngx-translate/http-loader';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ReportComponent } from './report/report-component';
import { ReportDetailComponent } from './report/report-detail-component';
import { ProfitAndLostSummaryReportComponent } from './report/profit-lost-summary-component';
import { ReportService } from './service/report-service';
import { ProfitAndLostDetailReportComponent } from './report/profit-lost-detail-component';
import { CustomerDetailBalanceReportComponent } from './sale-report/customer-detail-balance-component';
import { CustomerSummaryBalanceReportComponent } from './sale-report/customer-summary-balance-component';
import { CustomerInvoiceReportComponent } from './sale-report/customer-invoice-component';
import { AgedReceivablesSummaryComponent } from './sale-report/age-receivable-summary-component';
import { ProductAndServiceSaleSummaryReportComponent } from './report/product-summary-component';
import { ProductAndServiceSaleDetailReportComponent } from './report/product-sale-detail-component';
import { InventoryBalanceReportComponent } from './report/inventory-balance-component';
import { InventoryBalanceDetailReportComponent } from './report/inventory-balance-detail-component';
import { PayrollComponent } from './payroll/payroll-component';
import { CountryService } from './service/country-service';
import { PayrollService } from './service/payroll-service';
import { PayrollNoneTaxComponent } from './payroll/payroll-no-tax.component';
import { TaxMonthlyReportComponent } from './report/tax-monthly-component';
import { MenuService } from './service/menu-service';
import { UserComponent } from './user/user-component';
import { OrganisationUserService } from './service/organisation-user-service';
import { NewUserComponent } from './user/new-user-component';
import { UpdateUserComponent } from './user/update-user-component';
import { CloseDateComponent } from './modal/close-date-component';
import { LoginBarComponent } from './login-bar/login-bar-component';
import { MyProfileComponent } from './my-profile/my-profile-component';
import { GlobaltranslateService } from './service/global-translate-service';
import { LicenseExpireModalComponent } from './modal/license-expire-component';
import { BuyLicenseModalComponent } from './modal/buy-license-component';
import { PlanService } from './service/planservice';
import { LoadingStaticPositionComponent } from './loading/loading-static-position-component';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    NavBarComponent,
    TopBarComponent,
    FooterComponent,
    CompanyComponent,
    CurrencyComponent,
    NewCurrencyComponent,
    UpdateCurrencyComponent,
    VendorComponent,
    NewVendorComponent,
    UpdateVendorComponent,
    CustomerComponent,
    NewCustomerComponent,
    UpdateCustomerComponent,
    TaxRateComponent,
    NewTaxRateComponent,
    UpdateTaxRateComponent,
    ProductComponent,
    NewProductComponent,
    UpdateProductComponent,
    PurchaseComponent,
    NewPurchaseComponent,
    ErrorModalComponent,
    LocationComponent,
    NewLocationComponent,
    UpdateLocationComponent,
    UpdatePurchaseComponent,
    VendorBillComponent,
    NewBillComponent,
    UpdateBillComponent,
    QuoteComponent,
    NewQuoteComponent,
    UpdateQuoteComponent,
    NewInvoiceFromQuoteComponent,
    LoadingComponent,
    InvoiceComponent,
    NewInvoiceComponent,
    UpdateInvoiceFromQuoteComponent,
    PrintComponent,
    InvoiceSettingComponent,
    PurchaseOverViewComponent,
    SaleOverViewComponent,
    PrintReceiptComponent,
    EmployeeComponent,
    NewEmployeeComponent,
    UpdateEmployeeComponent,
    NewCompanyComponent,
    ReportComponent,
    ReportDetailComponent,
    ProfitAndLostSummaryReportComponent,
    ProfitAndLostDetailReportComponent,
    CustomerDetailBalanceReportComponent,
    CustomerSummaryBalanceReportComponent,
    CustomerInvoiceReportComponent,
    AgedReceivablesSummaryComponent,
    ProductAndServiceSaleSummaryReportComponent,
    ProductAndServiceSaleDetailReportComponent,
    InventoryBalanceReportComponent,
    InventoryBalanceDetailReportComponent,
    PayrollComponent,
    PayrollNoneTaxComponent,
    ReportDetailComponent,
    TaxMonthlyReportComponent,
    UserComponent,
    NewUserComponent,
    UpdateUserComponent,
    CloseDateComponent,
    LoginBarComponent,
    MyProfileComponent,
    LicenseExpireModalComponent,
    BuyLicenseModalComponent,
    LoadingStaticPositionComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    BrowserAnimationsModule,
    HttpClientModule,
    FormsModule,
    CustomFormsModule,
    TranslateModule.forRoot({
      loader: {
          provide: TranslateLoader,
          useFactory: HttpLoaderFactory,
          deps: [HttpClient]
      }
  }),
    NgbModule,
    AngularMultiSelectModule,
    FormsModule,
    ApiAuthorizationModule,
    ChartsModule,
    SelectDropDownModule,
    AngularDateTimePickerModule,
    NgxChartsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full',canActivate: [AuthorizeGuard] },
      { path: 'company', component: CompanyComponent, canActivate:[AuthorizeGuard]},
      { path: 'currencies', component: CurrencyComponent, canActivate:[AuthorizeGuard]},
      { path: 'new-currency', component: NewCurrencyComponent, canActivate:[AuthorizeGuard]},
      { path: 'update-currency/:id', component: UpdateCurrencyComponent, canActivate:[AuthorizeGuard] },
      { path: 'vendor', component: VendorComponent, canActivate:[AuthorizeGuard]},
      { path: 'vendor-new', component: NewVendorComponent, canActivate:[AuthorizeGuard]},
      { path: 'vendor-update/:id', component: UpdateVendorComponent, canActivate:[AuthorizeGuard] },
      { path: 'customer', component: CustomerComponent, canActivate:[AuthorizeGuard]},
      { path: 'customer-new', component: NewCustomerComponent, canActivate:[AuthorizeGuard]},
      { path: 'customer-update/:id', component: UpdateCustomerComponent , canActivate:[AuthorizeGuard]},
      { path: 'taxrate', component: TaxRateComponent, canActivate:[AuthorizeGuard]},
      { path: 'taxrate-new', component: NewTaxRateComponent, canActivate:[AuthorizeGuard]},
      { path: 'taxrate-update/:id', component: UpdateTaxRateComponent, canActivate:[AuthorizeGuard] },
      { path: 'product', component: ProductComponent, canActivate:[AuthorizeGuard]},
      { path: 'product-new', component: NewProductComponent, canActivate:[AuthorizeGuard]},
      { path: 'product-update/:id', component: UpdateProductComponent, canActivate:[AuthorizeGuard] },
      { path: 'location', component: LocationComponent, canActivate:[AuthorizeGuard]},
      { path: 'location-new', component: NewLocationComponent, canActivate:[AuthorizeGuard]},
      { path: 'location-update/:id', component: UpdateLocationComponent, canActivate:[AuthorizeGuard]},
      { path: 'invoicesetting', component: InvoiceSettingComponent, canActivate:[AuthorizeGuard]},
      { path: 'order', component: PurchaseComponent, canActivate:[AuthorizeGuard]},
      { path: 'order-new', component: NewPurchaseComponent, canActivate:[AuthorizeGuard]},
      { path: 'order-update/:id', component: UpdatePurchaseComponent, canActivate:[AuthorizeGuard]},
      { path: 'vendor-bill', component: VendorBillComponent, canActivate:[AuthorizeGuard]},
      { path: 'vendor-bill-new', component: NewBillComponent, canActivate:[AuthorizeGuard]},
      { path: 'vendor-bill-update/:id', component: UpdateBillComponent, canActivate:[AuthorizeGuard]},
      { path: 'quote', component: QuoteComponent, canActivate:[AuthorizeGuard]},
      { path: 'quote-new', component: NewQuoteComponent, canActivate:[AuthorizeGuard]},
      { path: 'quote-update/:id', component: UpdateQuoteComponent, canActivate:[AuthorizeGuard]},
      { path: 'invoice', component: InvoiceComponent, canActivate:[AuthorizeGuard]},
      { path: 'invoice-newquote/:id', component: NewInvoiceFromQuoteComponent, canActivate:[AuthorizeGuard]},
      { path: 'invoice-new', component: NewInvoiceComponent, canActivate:[AuthorizeGuard]},
      { path: 'invoice-update/:id', component: UpdateInvoiceFromQuoteComponent, canActivate:[AuthorizeGuard]},
      { path: 'purchaseoverview', component: PurchaseOverViewComponent, canActivate:[AuthorizeGuard]},
      { path: 'saleoverview', component: SaleOverViewComponent, canActivate:[AuthorizeGuard]},
      { path: 'employee', component: EmployeeComponent, canActivate:[AuthorizeGuard]},
      { path: 'employee-new', component: NewEmployeeComponent, canActivate:[AuthorizeGuard]},
      { path: 'employee-update/:id', component: UpdateEmployeeComponent, canActivate:[AuthorizeGuard]},
      { path: 'payroll', component: PayrollComponent, canActivate:[AuthorizeGuard]},
      { path: 'payroll-nonetax', component: PayrollNoneTaxComponent, canActivate:[AuthorizeGuard]},
      { path: 'report', component: ReportComponent, canActivate:[AuthorizeGuard]},
      { path: 'report-detail', component: ReportDetailComponent, canActivate:[AuthorizeGuard]},
      { path: 'report-profitandlostsumarry', component: ProfitAndLostSummaryReportComponent, canActivate:[AuthorizeGuard]},
      { path: 'report-profitandlostdetail', component: ProfitAndLostDetailReportComponent, canActivate:[AuthorizeGuard]},
      { path: 'report-customerbalancedetail', component: CustomerDetailBalanceReportComponent, canActivate:[AuthorizeGuard]},
      { path: 'report-customerbalancesummary', component: CustomerSummaryBalanceReportComponent, canActivate:[AuthorizeGuard]},
      { path: 'report-customerinvoice', component: CustomerInvoiceReportComponent, canActivate:[AuthorizeGuard]},
      { path: 'report-agedreceivablessummary', component: AgedReceivablesSummaryComponent, canActivate:[AuthorizeGuard]},
      { path: 'report-productandservicesalesummary', component: ProductAndServiceSaleSummaryReportComponent, canActivate:[AuthorizeGuard]},
      { path: 'report-productandservicesaledetail', component: ProductAndServiceSaleDetailReportComponent, canActivate:[AuthorizeGuard]},
      { path: 'report-inventorybalance', component: InventoryBalanceReportComponent, canActivate:[AuthorizeGuard]},
      { path: 'report-inventorybalancedetail', component: InventoryBalanceDetailReportComponent, canActivate:[AuthorizeGuard]},
      { path: 'report-taxmonthly', component: TaxMonthlyReportComponent, canActivate:[AuthorizeGuard]},
      { path: 'user', component: UserComponent, canActivate:[AuthorizeGuard]},
      { path: 'user-new', component: NewUserComponent, canActivate:[AuthorizeGuard]},
      { path: 'user-update/:email', component: UpdateUserComponent, canActivate:[AuthorizeGuard]},
      { path: 'myprofile', component: MyProfileComponent, canActivate:[AuthorizeGuard]},
    ])
  ],
  providers: [
    BaseService,
    UserService,
    OrganisationTypeService,
    OrganisationService,
    CurrecyService,
    VendorService,
    CustomerService,
    PagerService,
    TaxService,
    ProductService,
    PurchaseService,
    PurchaseOrderStatusService,
    LocationService,
    FileService,
    ExpenseService,
    BillService,
    VendorExpenseService,
    VendorCreditService,
    QuoteService,
    InvoiceService,
    CustomerCreditService,
    PrintService,
    OrganisationInvoiceSettingService,
    GenderService,
    SalaryTypeService,
    MaritalStatusService,
    EmployeeService,
    ChartService,
    ThemeService,
    ReportService,
    CountryService,
    PayrollService,
    MenuService,
    OrganisationUserService,
    GlobaltranslateService ,
    PlanService,
    {
    provide:HTTP_INTERCEPTORS,
    useClass:AuthorizeInterceptor,
    multi:true
  }],
  bootstrap: [AppComponent] ,
  entryComponents: [
    ErrorModalComponent,
    PrintComponent,
    PrintReceiptComponent,
    NewCompanyComponent,
    CloseDateComponent,
    LicenseExpireModalComponent,
    BuyLicenseModalComponent
  ]
})
export class AppModule { }
export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(http);
}