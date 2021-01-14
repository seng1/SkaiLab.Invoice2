using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using SkaiLab.Invoice.Dal;
using SkaiLab.Invoice.Service;
using SkaiLab.Invoice.Models;

namespace SkaiLab.Invoice.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailSender;
        private readonly IAppResource appResource;
        private readonly IUserService userService;

        public ForgotPasswordModel(UserManager<ApplicationUser> userManager, 
            IEmailService emailSender,
            IAppResource appResource,
            IUserService userService)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            this.appResource = appResource;
            this.userService = userService;
        }

        [BindProperty]
        public InputModel Input { get; set; }
        public List<string> Errors { get; set; }
        public string Culture { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }
        public async Task OnGetAsync(string culture = null)
        {
            culture = culture ?? "en-US";
            Culture = culture;
            Errors = new List<string>();
        }

        public async Task<IActionResult> OnPostAsync(string culture = null)
        {
            culture = culture ?? "en-US";
            Culture = culture;
            Errors = new List<string>();
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, appResource.GetResource("User with thi email doesn't exist"));
                    Errors.Add(appResource.GetResource("User with thi email doesn't exist"));
                    return Page();
                }

                if (!await _userManager.IsEmailConfirmedAsync(user))
                {

                    await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    await userService.SendNewCodeAsync(user.Id);
                    return RedirectToPage("RegisterConfirmation", new { email = Input.Email, id = user.Id, returnUrl = Url.Content("~/") });
                }
                await _userManager.GeneratePasswordResetTokenAsync(user);
                await userService.SendNewCodeAsync(user.Id);
                return RedirectToPage("./ForgotPasswordConfirmation", new { email = Input.Email, culture = culture, id = user.Id });
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
