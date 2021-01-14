using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using SkaiLab.Invoice.Dal;
using SkaiLab.Invoice.Models;
using SkaiLab.Invoice.Service;

namespace SkaiLab.Invoice.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordConfirmation : PageModel
    {
        private readonly IAppResource appResource;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IUserService userService;
        private readonly SignInManager<ApplicationUser> signInManager;
        public ForgotPasswordConfirmation(IAppResource appResource, 
            UserManager<ApplicationUser> userManager,
             SignInManager<ApplicationUser> signInManager,
            IUserService userService)
        {
            this.appResource = appResource;
            this.userManager = userManager;
            this.userService = userService;
            this.signInManager = signInManager;
        }
        public void OnGet(string email,string culture = null, string returnUrl = null, string id = null)
        {
            Email = email;
            Culture = culture;
            Errors = new List<string>();
            ReturnUrl = returnUrl ?? Url.Content("~/");
            Id = id;
        }
        public async Task<IActionResult> OnPostAsync(string email, string culture = null, string returnUrl = null, string id = null)
        {
            Id = id;
            Email = email;
            Culture = culture;
            Errors = new List<string>();
            ReturnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                if (!userService.IsCodeValid(id, Input.VerificationCode))
                {
                    ModelState.AddModelError(string.Empty, appResource.GetResource("Verification code doesn't correct or already expired"));
                    Errors.Add(appResource.GetResource("Verification code doesn't correct or already expired"));
                    return Page();
                }
                var user = await userManager.FindByIdAsync(id);
                var code = await userManager.GeneratePasswordResetTokenAsync(user);
               // code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var result = await userManager.ResetPasswordAsync(user, code, Input.Password);
                if (!result.Succeeded)
                {
                    foreach(var error in result.Errors)
                    {
                        ModelState.AddModelError(error.Code, appResource.GetResource(error.Description));
                        Errors.Add(appResource.GetResource(error.Description));
                    }
                    return Page();
                }
                return RedirectToPage("./Login", new {culture = culture });
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
        [BindProperty]
        public InputModel Input { get; set; }
        public string Email { get; set; }
        public string Culture { get; set; }
        public string ReturnUrl { get; set; }
        public List<string> Errors { get; set; }
        public string Id { get; set; }
        public class InputModel
        {
            [Required(ErrorMessage = "Verification code is require.")]
            [DataType(DataType.Text)]
            [Display(Name = "Verification code")]
            public string VerificationCode { get; set; }
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
        }
    }
}
