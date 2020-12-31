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
        public InviteModel(IOrganisationUserService organisationUserService,UserManager<ApplicationUser> userManager)
        {
            this.organisationUserService = organisationUserService;
            this.userManager = userManager;
        }
        [BindProperty]
        public InputModel Input { get; set; }
        public OrganisationUser OrganisationUser { get; set; }
        public async Task<IActionResult> OnGetAsync(string token)
        {
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
            if (userdb != null)
            {
                organisationUserService.ConfirmationInvitation(OrganisationUser.OrganisationId, OrganisationUser.User.Email);
                return RedirectToPage("Login");
            }
            HttpContext.Session.SetString("Token", token);
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            OrganisationUser = organisationUserService.GetOrganisationUser(HttpContext.Session.GetString("Token"));
            if (ModelState.IsValid)
            {
                await  organisationUserService.ConfirmationInvitationAsync(OrganisationUser.OrganisationId, OrganisationUser.User.Email, Input.Password,userManager);
                return RedirectToPage("Login");
            }
            // If we got this far, something failed, redisplay form
            return Page();
        }
        public class InputModel
        {

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
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
