using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
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
    public class ExternalLoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailSender;
        private readonly ILogger<ExternalLoginModel> _logger;
        private readonly IOrganisationService _organisationService;
        private readonly ICurrencyService _currencyService;
        public List<Currency> Currencies { get; set; }
        public ExternalLoginModel(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<ExternalLoginModel> logger,
            IOrganisationService organisationService,
              ICurrencyService currencyService,
            IEmailService emailSender)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _emailSender = emailSender;
            _organisationService = organisationService;
            _currencyService = currencyService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ProviderDisplayName { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
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

        public IActionResult OnGetAsync()
        {
            this.Currencies = _currencyService.GetCurrencies();
            return RedirectToPage("./Login");
        }

        public IActionResult OnPost(string provider, string returnUrl = null)
        {
            this.Currencies = _currencyService.GetCurrencies();
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string remoteError = null)
        {
            this.Currencies = _currencyService.GetCurrencies();
            returnUrl = returnUrl ?? Url.Content("~/");
            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToPage("./Login", new {ReturnUrl = returnUrl });
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Error loading external login information.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor : true);
            if (result.Succeeded)
            {
                _logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);
                return LocalRedirect(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return RedirectToPage("./Lockout");
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ReturnUrl = returnUrl;
                ProviderDisplayName = info.ProviderDisplayName;
                if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                {
                    Input = new InputModel
                    {
                        Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                    };
                    if (info.Principal.HasClaim(c => c.Type == ClaimTypes.GivenName))
                    {
                        Input.FirstName = info.Principal.FindFirstValue(ClaimTypes.Surname);
                    }
                    if (info.Principal.HasClaim(c => c.Type == ClaimTypes.GivenName))
                    {
                        Input.LastName = info.Principal.FindFirstValue(ClaimTypes.GivenName);
                    }
                    if (info.Principal.HasClaim(c => c.Type == ClaimTypes.MobilePhone))
                    {
                        Input.PhoneNumber = info.Principal.FindFirstValue(ClaimTypes.MobilePhone);
                    }
                }
                return Page();
            }
        }

        public async Task<IActionResult> OnPostConfirmationAsync(string returnUrl = null)
        {
            this.Currencies = _currencyService.GetCurrencies();
            returnUrl = returnUrl ?? Url.Content("~/");
            // Get the information about the user from the external login provider
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Error loading external login information during confirmation.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = Input.Email, Email = Input.Email };

                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);

                        var userId = await _userManager.GetUserIdAsync(user);
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Surname, Input.FirstName),
                            new Claim(ClaimTypes.GivenName, Input.LastName),
                            new Claim(ClaimTypes.MobilePhone, Input.PhoneNumber)
                        };
                        await _userManager.AddClaimsAsync(user, claims);
                        var organisation = new Organsation
                        {
                            BussinessRegistrationNumber = "",
                            Contact = new Contact
                            {
                                PhoneNumber = Input.PhoneNumber,
                                Address = "",
                                ContactName = Input.FirstName + " " + Input.LastName,
                                Email = Input.Email,
                                Website = "",
                                Id = Guid.NewGuid().ToString()
                            },
                            DeclareTax = Input.TaxDeclare,
                            LegalLocalName = Input.OrganisationDisplayName,
                            LegalName = Input.OrganisationDisplayName,
                            Description = "",
                            DisplayName = Input.OrganisationDisplayName,
                            LineBusiness = "",
                            LogoUrl = "",
                            OrganisationTypeId = 1,
                            TaxNumber = "",
                            OrganisationBaseCurrency = new OrganisationBaseCurrency
                            {
                                BaseCurrencyId = Input.BaseCurrencyId,
                                TaxCurrencyId = Input.BaseCurrencyId
                            }
                        };
                        var baseCurrency = Currencies.FirstOrDefault(u => u.Id == Input.BaseCurrencyId);
                        if (Input.TaxDeclare && baseCurrency.Code != "KHR")
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
                            values: new { area = "Identity", userId = userId, code = code },
                            protocol: Request.Scheme);

                        await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.",true);

                        // If account confirmation is required, we need to show the link if we don't have a real email sender
                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            return RedirectToPage("./RegisterConfirmation", new { Email = Input.Email });
                        }

                        await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);

                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ProviderDisplayName = info.ProviderDisplayName;
            ReturnUrl = returnUrl;
            return Page();
        }
    }
}
