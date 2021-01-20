using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SkaiLab.Invoice.Dal;
using SkaiLab.Invoice.Models;
using SkaiLab.Invoice.Service;

namespace SkaiLab.Invoice.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class InviteModel : PageModel
    {
        private readonly IOrganisationUserService organisationUserService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IAppResource appResource;
        public InviteModel(IOrganisationUserService organisationUserService,UserManager<ApplicationUser> userManager, IAppResource appResource)
        {
            this.organisationUserService = organisationUserService;
            this.userManager = userManager;
            this.appResource = appResource;
        }
        [BindProperty]
        public InputModel Input { get; set; }
        public OrganisationUser OrganisationUser { get; set; }
        public List<string> Errors { get; set; }
        public string Culture { get; set; }
        public string Name { get; set; }
        public string Token { get; set; }
        public string ReturnUrl { get; set; }
        public async Task<IActionResult> OnGetAsync(string token, string culture = null,string name=null, string returnUrl = null)
        {
            Errors = new List<string>();
            Culture = culture ?? "en-US";
            Name = name;
            Token = token;
            returnUrl = returnUrl ?? Url.Content("~/");
            ReturnUrl = returnUrl;
            OrganisationUser = organisationUserService.GetOrganisationUser(token);
            if (OrganisationUser == null)
            {
                return Redirect(Utils.ReturnToError(Url,HttpContext,"The Url is not valid or already expire."));
            }
            if (OrganisationUser.IsInvitingExpired)
            {
                return Redirect(Utils.ReturnToError(Url, HttpContext, "The Url is not valid or already expire."));
            }
            var userdb =await userManager.FindByEmailAsync(OrganisationUser.User.Email);
            if (userdb != null && OrganisationUser !=null && OrganisationUser.User!=null)
            {
                organisationUserService.ConfirmationInvitation(OrganisationUser.OrganisationId, OrganisationUser.User.Email);
                return RedirectToPage("Login");
            }
            HttpContext.Session.SetString("Token", token);
            return Page();
        }
        public async Task<IActionResult> OnPostAsync(string token, string culture = null, string name = null, string returnUrl = null)
        {
            Errors = new List<string>();
            Culture = culture??"en-US";
            Name = name;
            returnUrl = returnUrl ?? Url.Content("~/");
            ReturnUrl = returnUrl;
            Token = token;
            OrganisationUser = organisationUserService.GetOrganisationUser(token);
            if (ModelState.IsValid)
            {
                await organisationUserService.ConfirmationInvitationAsync(OrganisationUser.OrganisationId, OrganisationUser.User.Email, Input.Password, userManager, Input.PhoneNumber);
                return RedirectToPage("Login");
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
            // If we got this far, something failed, redisplay form
            return Page();
        }
        public class InputModel
        {

            [Required]
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
    }
}
