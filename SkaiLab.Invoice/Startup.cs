using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using SkaiLab.Invoice.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using SkaiLab.Invoice.Dal;
using SkaiLab.Invoice.Dal.Data;
using SkaiLab.Invoice.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using SkaiLab.Invoice.Service.Subscription;

namespace SkaiLab.Invoice
{
    public class Startup
    {
        
        public IWebHostEnvironment Environment { get; }
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;

        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLocalization(
                opts =>
                {
                    opts.ResourcesPath = "Resources";
                });
            var supportedCultures = new[]
            {
                new CultureInfo("km-KH"),
                new CultureInfo("en-US"),
            };
            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture("en-US");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                options.RequestCultureProviders = new List<IRequestCultureProvider>
            {
                new QueryStringRequestCultureProvider(),
                new CookieRequestCultureProvider()
            };
            });
            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(Configuration["Option:ConnectionString"].ToString()));
            services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
               .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddIdentityServer()
                    .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();
            services.Configure<JwtBearerOptions>(
             IdentityServerJwtConstants.IdentityServerJwtBearerScheme,
             options =>
             {
                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuer = false,
                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("mast12345")),
                     ValidateIssuerSigningKey = true,
                     //ValidIssuers = new[]
                     //{
                     //   "http://localhost:5000",
                     //   "http://127.0.0.1:5000",
                     //}
                 };

             });

            services.AddAuthentication()
                .AddIdentityServerJwt();
            services.AddAuthentication().AddFacebook(facebookOptions =>
            {
                facebookOptions.AppId = Configuration["Option:Facebook:ClientId"];
                facebookOptions.AppSecret = Configuration["Option:Facebook:Secret"];
            });
            services.AddAuthentication().AddMicrosoftAccount(microsoftOptions =>
            {
                microsoftOptions.ClientId = Configuration["Option:Microsoft:ClientId"];
                microsoftOptions.ClientSecret = Configuration["Option:Microsoft:Secret"];
            });
            services.AddControllersWithViews().AddRazorRuntimeCompilation().
                AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization();

            services.AddRazorPages();
            services.AddOptions();
            services.AddSession();
            services.AddMvc().AddSessionStateTempDataProvider(); ;
            services.Configure<Option>(Configuration.GetSection("Option"));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IDataContext, DataContext>();
            services.AddSingleton<IService, Service.Service>();
            services.AddSingleton<IOrganisationService, OrganisationService>();
            services.AddSingleton<IOrganisationTypeService, OrganisationTypeService>();
            services.AddSingleton<ICurrencyService, CurrencyService>();
            services.AddSingleton<IVendorService, VendorService>();
            services.AddSingleton<ICustomerService, CustomerService>();
            services.AddSingleton<ITaxService, TaxService>();
            services.AddSingleton<IProductService, ProductService>();
            services.AddSingleton<IPurchaseOrderService, PurchaseOrderService>();
            services.AddSingleton<IPurchaseOrderStatusService, PurchaseOrderStatusService>();
            services.AddSingleton<ILocationService, LocationService>();
            services.AddSingleton<IExpenseService, ExpenseService>();
            services.AddSingleton<IExpenseService, ExpenseService>();
            services.AddSingleton<IBillService, BillService>();
            services.AddSingleton<IVendorExpenseService, VendorExpenseService>();
            services.AddSingleton<IQuoteService, QuoteService>();
            services.AddSingleton<IInvoiceService, InvoiceService>();
            services.AddSingleton<IHtmlToPdfConverterService, HtmlToPdfConverterService>();
            services.AddSingleton<IOrganisationInvoiceSettingService, OrganisationInvoiceSettingService>();
            services.AddSingleton<IPrintService, PrintService>();
            services.AddSingleton<IGenderService, GenderService>();
            services.AddSingleton<ISalaryTypeService, SalaryTypeService>();
            services.AddSingleton<IMaritalStatusService, MaritalStatusService>();
            services.AddSingleton<IEmployeeService, EmployeeService>();
            services.AddSingleton<IEmailService, EmailService>();
            services.AddSingleton<IPieChartService, PieChartService>();
            services.AddSingleton<IReportService, ReportService>();
            services.AddSingleton<ICountryService, CountryService>();
            services.AddSingleton<IPayrollService, PayrollService>();
            services.AddSingleton<IMenuService, MenuService>();
            services.AddSingleton<IOrganisationUserService, OrganisationUserService>();
            services.AddSingleton<IAppResource, SharedResources>();
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IPaymentService, PaymentService>();
            services.AddSingleton<IPaywayService, PaywayService>();
            services.AddSingleton<IPlanService, PlanService>();
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();
            app.UseRequestLocalization();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
            app.UseRouting();
            app.UseAuthentication();
            app.UseIdentityServer();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}
