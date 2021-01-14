using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
        private readonly IEmailService _emailSender;
        private readonly ICurrencyService _currencyService;
        private readonly IAppResource appResource;
        private readonly IUserService userService;
        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ICurrencyService currencyService,
            IAppResource appResource,
            IUserService userService,
            IEmailService emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _currencyService = currencyService;
            this.appResource = appResource;
            this.userService = userService;

        }
        public List<Currency> Currencies { get; set; }
        [BindProperty]
        public InputModel Input { get; set; }
        public List<string> Errors { get; set; }
        public string ReturnUrl { get; set; }
        public string Culture { get; set; }
        public string OfferType { get; set; }
        public int PlanId { get; set; }
        public string SubscriptionType { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {

            public InputModel()
            {
            }
            [Required(ErrorMessage = "Email is require.")]
            [EmailAddress]
            public string Email { get; set; }
            [Required(ErrorMessage = "Name is require.")]
            [DataType(DataType.Text)]
            [Display(Name = "Name")]
            public string Name { get; set; }
            [Required(ErrorMessage = "Password is require.")]
            [StringLength(100, ErrorMessage = "The Password must be at least 6 and at max 100 characters long.")]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
            [Required(ErrorMessage = "Phone number is require.")]
            [DataType(DataType.Text)]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

        }

        public async Task OnGetAsync(string returnUrl = null, string culture = null, string offerType = null, int planId = 0, string subscriptionType = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            this.Currencies = _currencyService.GetCurrencies();
            Culture = culture;
            OfferType = offerType;
            SubscriptionType = subscriptionType;
            PlanId = planId;
            Errors = new List<string>();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null, string culture = null, int planId = 0, string offerType = null, string subscriptionType = null)
        {
            Currencies = _currencyService.GetCurrencies();
            returnUrl = returnUrl ?? Url.Content("~/");
            Culture = culture;
            OfferType = offerType;
            SubscriptionType = subscriptionType;
            Errors = new List<string>();
            PlanId = planId;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = Input.Email, Email = Input.Email };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.MobilePhone, Input.PhoneNumber),
                        new Claim(ClaimTypes.Name, Input.Name)
                    };
                    user = await _userManager.FindByEmailAsync(Input.Email);
                    await _userManager.AddClaimsAsync(user, claims);
                    var code =await userService.CreateUser(user, _userManager, planId, subscriptionType == "monthly" ? 1 : 2, offerType == "Trail");
                    await userService.SendNewCodeAsync(user.Id);
                    return RedirectToPage("RegisterConfirmation", new { id = user.Id, email = user.Email, culture = Culture });
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            if (!ModelState.IsValid)
            {
                foreach (var r in ModelState.Values)
                {
                    foreach (var t in r.Errors)
                    {
                        Errors.Add(appResource.GetResource(t.ErrorMessage));
                    }
                }
            }
            return Page();
        }
    }
}
