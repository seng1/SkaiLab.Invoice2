using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using SkaiLab.Invoice.Dal;
using SkaiLab.Invoice.Models;
using SkaiLab.Invoice.Service;

namespace SkaiLab.Invoice.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailService _emailSender;
        private readonly IOrganisationService _organisationService;
        private readonly ICurrencyService _currencyService;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IOrganisationService organisationService,
            ILogger<RegisterModel> logger,
            ICurrencyService currencyService,
            IEmailService emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _organisationService = organisationService;
            _currencyService = currencyService;
        }
        public List<Currency> Currencies { get; set; }
        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            public InputModel()
            {
                TaxDeclare = true;
                BaseCurrencyId = 2;
            }
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "First name")]
            public string FirstName { get; set; }
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Last name")]
            public string LastName { get; set; }
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Organisation Display Name")]
            public string OrganisationDisplayName { get; set; }
            [Display(Name = "This organisation declare tax?(it cannot change)?")]
            public bool TaxDeclare { get; set; } 
            public int BaseCurrencyId { get; set; }
           


        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            this.Currencies = _currencyService.GetCurrencies();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            Currencies = _currencyService.GetCurrencies();
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = Input.Email, Email = Input.Email };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    var claims = new List<Claim>();
                    claims.Add(new Claim(ClaimTypes.Surname, Input.FirstName));
                    claims.Add(new Claim(ClaimTypes.GivenName, Input.LastName));
                    claims.Add(new Claim(ClaimTypes.MobilePhone, Input.PhoneNumber));
                    user =await _userManager.FindByEmailAsync(Input.Email);
                    await _userManager.AddClaimsAsync(user, claims);
                    var organisation = new Organsation
                    {
                        BussinessRegistrationNumber="",
                        Contact=new Contact
                        {
                            PhoneNumber=Input.PhoneNumber,
                            Address="",
                            ContactName=Input.FirstName + " "+Input.LastName,
                            Email=Input.Email,
                            Website="",
                            Id=Guid.NewGuid().ToString()
                        },
                        DeclareTax=Input.TaxDeclare,
                        LegalLocalName=Input.OrganisationDisplayName,
                        LegalName=Input.OrganisationDisplayName,
                        Description="",
                        DisplayName=Input.OrganisationDisplayName,
                        LineBusiness="",
                        LogoUrl="",
                        OrganisationTypeId=1,
                        TaxNumber="",
                        OrganisationBaseCurrency=new OrganisationBaseCurrency
                        {
                            BaseCurrencyId=Input.BaseCurrencyId,
                            TaxCurrencyId=Input.BaseCurrencyId
                        }
                    };
                    var baseCurrency = Currencies.FirstOrDefault(u => u.Id == Input.BaseCurrencyId);
                    if(Input.TaxDeclare && baseCurrency.Code != "KHR")
                    {
                        organisation.OrganisationBaseCurrency.TaxCurrencyId = Currencies.FirstOrDefault(u => u.Code == "KHR").Id;
                        organisation.OrganisationBaseCurrency.TaxExchangeRate = 4000;
                    }
                    _organisationService.Create(organisation, user.Id);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email", $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.",true);

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
