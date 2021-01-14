using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SkaiLab.Invoice.Dal;
using System.Collections.Generic;
using SkaiLab.Invoice.Models;
using System.ComponentModel.DataAnnotations;
using SkaiLab.Invoice.Service;
using System.Linq;

namespace SkaiLab.Invoice.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterConfirmationModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _sender;
        private readonly IAppResource appResource;
        private readonly ICurrencyService _currencyService;
        private readonly IUserService userService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public RegisterConfirmationModel(UserManager<ApplicationUser> userManager, 
            ICurrencyService currencyService,
            IUserService userService,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender sender, IAppResource appResource)
        {
            _userManager = userManager;
            _sender = sender;
            this.appResource = appResource;
            this._currencyService = currencyService;
            this.userService = userService;
            this._signInManager = signInManager;
        }
        [BindProperty]
        public InputModel Input { get; set; }
        public string Email { get; set; }
        public string Id { get; set; }
        public string Culture { get; set; }
        public bool IsTrail { get; set; }
        public List<string> Errors { get; set; }
        public string ReturnUrl { get; set; }
        public List<Currency> Currencies { get; set; }
        public async Task<IActionResult> OnGetAsync(string email, string returnUrl = null,string id=null,string culture=null)
        {
            if (email == null)
            {
                return RedirectToPage("/Index");
            }
            Currencies = _currencyService.GetCurrencies();
            var usd = Currencies.FirstOrDefault(u => u.Code == "USD");
            Currencies.Remove(usd);
            Currencies.Insert(0, usd);
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound($"Unable to load user with email '{email}'.");
            }
            ReturnUrl = returnUrl;
            Email = email;
            Id = id;
            IsTrail = userService.IsTrailUser(id);
            Culture = culture;
            Errors = new List<string>();
            return Page();
        }
        public async Task<IActionResult> OnPostAsync(string email, string returnUrl = null, string id = null, string culture = null)
        {
            Email = email;
            Id = id;
            ReturnUrl = returnUrl;
            Culture = culture;
            IsTrail = userService.IsTrailUser(id);
            Errors = new List<string>();
            Currencies = _currencyService.GetCurrencies();
            var usd = Currencies.FirstOrDefault(u => u.Code == "USD");
            Currencies.Remove(usd);
            Currencies.Insert(0, usd);
            if (ModelState.IsValid)
            {
                var result =await userService.ConfirmRegisterAsync(id, Input.VerificationCode, _userManager, Input.OrganisationName, Input.CurrencyId, Input.TaxDeclare);
                if (!result)
                {
                    ModelState.AddModelError("VerificationCode", "Verification code doesn't correct or already expired");
                }
                else
                {
                    var user1 = await _userManager.FindByIdAsync(id);
                    await _signInManager.SignInAsync(user1, false);
                    if (IsTrail)
                    {
                        return LocalRedirect(Url.Content("~/"));
                    }
                    else
                    {
                        var url = Url.Content("~/");
                        return LocalRedirect(url + "MakePayment?id="+id+ "&culture="+culture);
                    }
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
    public class InputModel
    {

        public InputModel()
        {
        }
        [Required(ErrorMessage = "Verification code is require.")]
        [DataType(DataType.Text)]
        [Display(Name = "Verification code")]
        public string VerificationCode { get; set; }

        [Required(ErrorMessage = "Organisation name.")]
        [DataType(DataType.Text)]
        [Display(Name = "Organisation Name")]
        public string OrganisationName { get; set; }
        [Display(Name = "This organisation declare tax?(it cannot change)?")]
        public bool TaxDeclare { get; set; }
        public int CurrencyId { get; set; }

    }
}
