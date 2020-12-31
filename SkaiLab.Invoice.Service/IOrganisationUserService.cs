using Microsoft.AspNetCore.Identity;
using SkaiLab.Invoice.Dal;
using SkaiLab.Invoice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Service
{
    public interface IOrganisationUserService:IOrganisationService
    {
        List<OrganisationUser> GetOrganisationUsers(string organisationId, string userId);
        List<MenuFeature> GetMenuFeatures();
        Task InviteUserAsync(OrganisationUser organisationUser, string organisationId, string userId, string webUrl);
        Task ResentInviatationAsync(string organisationId, string userId, string email, string webUrl);
        OrganisationUser GetOrganisationUser(string organisationId, string requestUserId, string email);
        void RemoveOrganisationUser(string organisationId, string removeByUserId, string email);
        Task UpdateUserRoleAsync(string organisationId, string updateByUserId, OrganisationUser organisationUser);
        OrganisationUser GetOrganisationUser(string token);
        void ConfirmationInvitation(string organisationId, string email);
        Task ConfirmationInvitationAsync(string organisationId, string email,string password,UserManager<ApplicationUser> userManager);
        User GetUser(string userId);
        void UpdateUser(User user);
        void UpdateUserLanguage(User user);
    }
}
