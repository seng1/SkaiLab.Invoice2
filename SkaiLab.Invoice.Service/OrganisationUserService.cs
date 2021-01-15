using Microsoft.AspNetCore.Identity;
using SkaiLab.Invoice.Dal;
using SkaiLab.Invoice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Service
{
    public class OrganisationUserService : OrganisationService, IOrganisationUserService
    {
        private readonly IEmailService emailService;
        public OrganisationUserService(IDataContext context,IEmailService emailService) : base(context)
        {
            this.emailService = emailService;
        }

        public void ConfirmationInvitation(string organisationId, string email)
        {
            using var context = Context();
            var organisationUser = context.OrganisationInvitingUser.FirstOrDefault(u => u.OrganisationId == organisationId && u.Email.ToLower() == email.ToLower());
            var aspUser = context.AspNetUsers.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
            context.OrganisationUser.Add(new Dal.Models.OrganisationUser
            {
                IsOwner = false,
                OrganisationId = organisationId,
                RoleName = organisationUser.RoleName,
                UserId = aspUser.Id
            });
            var organisationInvitingFeatures = context.OrganisationInvitingUserMenuFeature.Where(u => u.OrganisationId == organisationId && u.Email.ToLower() == email.ToLower()).ToList();
            foreach(var or in organisationInvitingFeatures)
            {
                context.OrganisationUserMenuFeature.Add(new Dal.Models.OrganisationUserMenuFeature
                {
                    MenuFeatureId=or.MenuFeatureId,
                    OrganisationId=organisationId,
                    UserId=aspUser.Id
                });
            }
            var workingOrganisation = context.WorkingOrganisation.FirstOrDefault(u => u.UserId == aspUser.Id);
            if (workingOrganisation == null)
            {
                workingOrganisation = new Dal.Models.WorkingOrganisation();
                workingOrganisation.UserId = aspUser.Id;
                context.WorkingOrganisation.Add(workingOrganisation);
            }
            workingOrganisation.OrganisationId = organisationId;
            context.OrganisationInvitingUserMenuFeature.RemoveRange(organisationInvitingFeatures);
            context.OrganisationInvitingUser.Remove(organisationUser);
            context.SaveChanges();
        }

        public async Task ConfirmationInvitationAsync(string organisationId, string email, string password, UserManager<ApplicationUser> userManager,string phoneNumber)
        {
            using var context = Context();
            var organisationUser = context.OrganisationInvitingUser.FirstOrDefault(u => u.OrganisationId == organisationId && u.Email.ToLower() == email.ToLower());
            var user = new ApplicationUser { UserName = email, Email = email };
            var result = await userManager.CreateAsync(user, password);
            var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
            await userManager.ConfirmEmailAsync(user, code);
            await userManager.AddClaimsAsync(user, new List<Claim>
            {
                new Claim(ClaimTypes.Email,email),
                new Claim(ClaimTypes.Name,organisationUser.DisplayName),
                new Claim(ClaimTypes.MobilePhone,phoneNumber)
            });
            context.OrganisationUser.Add(new Dal.Models.OrganisationUser
            {
                IsOwner = false,
                OrganisationId = organisationId,
                RoleName = organisationUser.RoleName,
                UserId = user.Id
            });
            var organisationInvitingFeatures = context.OrganisationInvitingUserMenuFeature.Where(u => u.OrganisationId == organisationId && u.Email.ToLower() == email.ToLower()).ToList();
            foreach (var or in organisationInvitingFeatures)
            {
                context.OrganisationUserMenuFeature.Add(new Dal.Models.OrganisationUserMenuFeature
                {
                    MenuFeatureId = or.MenuFeatureId,
                    OrganisationId = organisationId,
                    UserId = user.Id
                });
            }
            context.WorkingOrganisation.Add(new Dal.Models.WorkingOrganisation
            {
                OrganisationId=organisationId,
                UserId=user.Id
            });
            context.OrganisationInvitingUserMenuFeature.RemoveRange(organisationInvitingFeatures);
            context.OrganisationInvitingUser.Remove(organisationUser);
            context.SaveChanges();
        }

        public List<MenuFeature> GetMenuFeatures()
        {
            using var context = Context();
            var result = context.MenuFeature.Select(u => new MenuFeature
            {
                Id=u.Id,
                IsCheck=false,
                Name=u.Name
            }).ToList();
            return result;
        }

        public OrganisationUser GetOrganisationUser(string organisationId, string requestUserId, string email)
        {
            using var context = Context();
            if (!context.OrganisationUserMenuFeature.Any(u => u.OrganisationId == organisationId && u.UserId == requestUserId && u.MenuFeatureId == (int)MenuFeatureEnum.ManageOrganisactionSetting))
            {
                throw new Exception("You don't have permission to invite user");
            }
            var organisationUser = context.OrganisationUser.FirstOrDefault(u => u.OrganisationId == organisationId && u.User.Email.ToLower() == email);
            var menuFeauture= context.MenuFeature.Select(u => new MenuFeature
            {
                Id=u.Id,
                Name=u.Name
            }).ToList();
            if (organisationUser != null)
            {
                var claims = organisationUser.User.AspNetUserClaims.Select(u => new { u.ClaimValue, u.ClaimType }).ToList();
                var result= new OrganisationUser
                {
                     IsAdministrator=organisationUser.IsOwner || organisationUser.RoleName=="Administrator",
                     IsOwner=organisationUser.IsOwner,
                     IsInviting=false,
                     IsInvitingExpired=false,
                     OrganisationId=organisationUser.OrganisationId,
                     RoleName=organisationUser.RoleName,
                     User=new User
                     {
                         Email=organisationUser.User.Email,
                         Name = claims.FirstOrDefault(u => u.ClaimType == ClaimTypes.Name).ClaimValue,
                         Id=organisationUser.UserId,
                         
                     },
                     MenuFeatures=menuFeauture
                };
                var mfs = context.OrganisationUserMenuFeature.Where(u => u.OrganisationId == organisationId && u.UserId == organisationUser.UserId)
                    .Select(u => u.MenuFeatureId).ToList();
                foreach(var m in result.MenuFeatures)
                {
                    m.IsCheck = mfs.Any(u => u == m.Id);
                }
                return result;
            }
            var invitingUser = context.OrganisationInvitingUser.FirstOrDefault(u => u.OrganisationId == organisationId && u.Email == email);
            if (invitingUser != null)
            {
                var result= new OrganisationUser
                {
                    IsAdministrator= invitingUser.RoleName == "Administrator",
                    IsInviting=true,
                    IsInvitingExpired=invitingUser.ExpireToken<CurrentCambodiaTime,
                    IsOwner=false,
                    RoleName=invitingUser.RoleName,
                    OrganisationId=invitingUser.OrganisationId,
                    User=new User
                    {
                        Email=invitingUser.Email,
                        Name=invitingUser.DisplayName
                    },
                    MenuFeatures= menuFeauture
                };
                var mfs = context.OrganisationInvitingUserMenuFeature.Where(u => u.OrganisationId == organisationId && u.Email == result.User.Email)
                  .Select(u => u.MenuFeatureId).ToList();
                foreach (var m in result.MenuFeatures)
                {
                    m.IsCheck = mfs.Any(u => u == m.Id);
                }
                return result;
            }
            throw new Exception("User doesn't exist");

        }

        public OrganisationUser GetOrganisationUser(string token)
        {
            using var context = Context();
            var invitingUser = context.OrganisationInvitingUser.FirstOrDefault(u => u.Token == token);
            if (invitingUser == null)
            {
                return null;
            }
            return new OrganisationUser
            {
                RoleName = invitingUser.RoleName,
                IsInviting = true,
                IsInvitingExpired = invitingUser.ExpireToken < CurrentCambodiaTime,
                User = new User
                {
                    Email = invitingUser.Email,
                    Name = invitingUser.DisplayName,

                },
                OrganisationId=invitingUser.OrganisationId
            };
        }

        public List<OrganisationUser> GetOrganisationUsers(string organisationId, string userId)
        {
            using var context = Context();
            if(!context.OrganisationUser.Any(t=>t.OrganisationId==organisationId && t.UserId == userId))
            {
                throw new Exception("Unauthorize");
            }
            var users = context.OrganisationUser.Where(u => u.OrganisationId == organisationId).Select(u => new OrganisationUser
            {
                IsInviting = false,
                IsInvitingExpired = false,
                IsOwner = u.IsOwner,
                OrganisationId = u.OrganisationId,
                RoleName = u.RoleName,
                User = new User
                {
                    Email = u.User.Email,
                    Id = u.User.Id,

                },

            }).ToList();
            var invitingUsers = context.OrganisationInvitingUser.Where(u => u.OrganisationId == organisationId).ToList().Select(u => new OrganisationUser
            {
                IsInviting = true,
                IsOwner = false,
                OrganisationId = u.OrganisationId,
                RoleName = u.RoleName,
                IsInvitingExpired = u.ExpireToken < CurrentCambodiaTime,
                User = new User
                {
                    Email = u.Email,
                    Name = u.DisplayName
                }
            }).ToList();
            if (invitingUsers.Any())
            {
                foreach(var inviteUser in invitingUsers)
                {
                    users.Insert(0, inviteUser);
                }
            }
            return users;
        }

        public User GetUser(string userId)
        {
            using var context = Context();
            var user = context.AspNetUsers.FirstOrDefault(u => u.Id == userId);
            var claims = user.AspNetUserClaims.Select(u => new { u.ClaimType, u.ClaimValue }).ToList();
            var language = user.AspNetUserClaims.FirstOrDefault(u => u.ClaimType == "Language");
            return new User
            {
                Id=user.Id,
                Email=user.Email,
                Language=language==null?"en":language.ClaimValue,
                Name = claims.FirstOrDefault(u => u.ClaimType == ClaimTypes.Name) == null ? "" : claims.First(u => u.ClaimType == ClaimTypes.Name).ClaimValue,
                PhoneNumber = claims.FirstOrDefault(u => u.ClaimType == ClaimTypes.MobilePhone) == null ? "" : claims.First(u => u.ClaimType == ClaimTypes.MobilePhone).ClaimValue,
            };
        }

        public async Task InviteUserAsync(OrganisationUser organisationUser, string organisationId, string userId, string webUrl)
        {
            using var context = Context();
            if (string.IsNullOrEmpty(organisationUser.User.Email))
            {
                throw new Exception("Email is require");
            }
            if (string.IsNullOrEmpty(organisationUser.User.Name))
            {
                throw new Exception("Display name is require");
            }
            if(!context.OrganisationUserMenuFeature.Any(u=>u.OrganisationId==organisationId && u.UserId==userId && u.MenuFeatureId == (int)MenuFeatureEnum.ManageOrganisactionSetting))
            {
                throw new Exception("You don't have permission to invite user");
            }
            if(context.OrganisationUser.Any(u=>u.OrganisationId==organisationId && u.User.Email.ToLower() == organisationUser.User.Email.ToLower()))
            {
                throw new Exception("User already invited");
            }
            if(context.OrganisationInvitingUser.Any(u=>u.OrganisationId==organisationId && u.Email == organisationUser.User.Email.ToLower()))
            {
                throw new Exception("User already invited");
            }
            var totalInvitedUser = context.OrganisationUser.Where(u => u.OrganisationId == organisationId).Count();
            totalInvitedUser += context.OrganisationInvitingUser.Where(u => u.OrganisationId == organisationId).Count();
            var totalUserCanAdd = GetTotalUsersCanAssign(organisationId, context);
            if (totalInvitedUser >= totalUserCanAdd)
            {
                throw new Exception("Your license is reach out to add user.");
            }
            var newInviteUser =new Dal.Models.OrganisationInvitingUser{
                DisplayName=organisationUser.User.Name,
                Email=organisationUser.User.Email.ToLower(),
                OrganisationId=organisationId,
                RoleName=organisationUser.RoleName,
                ExpireToken=CurrentCambodiaTime.AddDays(2),
                Token=Guid.NewGuid().ToString(),
            };

            if (!organisationUser.IsAdministrator)
            {
                foreach(var m in organisationUser.MenuFeatures)
                {
                    if (m.IsCheck)
                    {
                        context.OrganisationInvitingUserMenuFeature.Add(new Dal.Models.OrganisationInvitingUserMenuFeature
                        {
                            Email=organisationUser.User.Email.ToLower(),
                            MenuFeatureId=m.Id,
                            OrganisationId=organisationId
                        });
                    }
                }
            }
            else
            {
                foreach (var m in organisationUser.MenuFeatures)
                {
                    context.OrganisationInvitingUserMenuFeature.Add(new Dal.Models.OrganisationInvitingUserMenuFeature
                    {
                        Email = organisationUser.User.Email.ToLower(),
                        MenuFeatureId = m.Id,
                        OrganisationId = organisationId
                    });
                }
            }
            var organisation = context.Organisation.FirstOrDefault(u => u.Id == organisationId);
            context.OrganisationInvitingUser.Add(newInviteUser);
            context.SaveChanges();
            await ResentInviatationAsync(organisationId, userId, organisationUser.User.Email, webUrl);
        }
        private int GetTotalUsersCanAssign(string organisationId, Dal.Models.InvoiceContext context)
        {
            var ownerUser = context.OrganisationUser.FirstOrDefault(u => u.OrganisationId == organisationId && u.IsOwner).User.UserPlan;
            if (ownerUser.PlanId == 1)
            {
                return 1;
            }
            if (ownerUser.PlanId == 2)
            {
                return 4;
            }
            return 100000000;

        }
        public void RemoveOrganisationUser(string organisationId, string removeByUserId, string email)
        {
            using var context = Context();
            if (!context.OrganisationUserMenuFeature.Any(u => u.OrganisationId == organisationId && u.UserId == removeByUserId && u.MenuFeatureId == (int)MenuFeatureEnum.ManageOrganisactionSetting))
            {
                throw new Exception("You don't have permission to remove user");
            }
            var organisationUser = context.OrganisationUser.FirstOrDefault(u => u.OrganisationId == organisationId && u.User.Email.ToLower() == email.ToLower());
            if (organisationUser != null)
            {
                if (organisationUser.IsOwner)
                {
                    throw new Exception("You cannot remove owner");
                }
                if (organisationUser.UserId == removeByUserId)
                {
                    throw new Exception("You cannot remove yourself");
                }
                context.OrganisationUserMenuFeature.RemoveRange(context.OrganisationUserMenuFeature.Where(u => u.OrganisationId == organisationId && u.UserId == organisationUser.UserId));
                context.OrganisationUser.Remove(organisationUser);
                context.SaveChanges();
                return;
            }
            var organisationInviteUser = context.OrganisationInvitingUser.FirstOrDefault(u => u.OrganisationId == organisationId && u.Email == email.ToLower());
            if (organisationInviteUser != null)
            {
                context.OrganisationInvitingUserMenuFeature.RemoveRange(context.OrganisationInvitingUserMenuFeature.Where(u => u.OrganisationId == organisationId && u.Email.ToLower() == email.ToLower()));
                context.OrganisationInvitingUser.Remove(organisationInviteUser);
                context.SaveChanges();
                return;
            }
            throw new Exception("User doesn't exist");
        }

        public async Task ResentInviatationAsync(string organisationId, string userId, string email,string webUrl)
        {
            using var context = Context();
            if (!context.OrganisationUserMenuFeature.Any(u => u.OrganisationId == organisationId && u.UserId == userId && u.MenuFeatureId == (int)MenuFeatureEnum.ManageOrganisactionSetting))
            {
                throw new Exception("You don't have permission to invite user");
            }
            var inviteUser = context.OrganisationInvitingUser.FirstOrDefault(u => u.OrganisationId == organisationId && u.Email == email.ToLower());
            if (inviteUser == null)
            {
                throw new Exception("Invite user does't exist");
            }
            inviteUser.ExpireToken = CurrentCambodiaTime.AddDays(2);
            inviteUser.Token = Guid.NewGuid().ToString();
            context.SaveChanges();
            var userName = context.AspNetUserClaims.FirstOrDefault(u => u.UserId == userId && u.ClaimType == ClaimTypes.Name).ClaimValue;
            var organisation = context.Organisation.FirstOrDefault(u => u.Id == organisationId).DisplayName;
            webUrl += "/Identity/Account/Invite?token=" + inviteUser.Token+"&name="+userName;
            string subject = $"{userName} អញ្ជើញអ្នកចូលរួមប្រើ Skai Account របស់ក្រុមហ៊ុន {organisation}({userName} invite to join  Skai Account for company {organisation})";
            string body = $"You have invite to join Skai Account for company {organisation}. Please click <a href='{webUrl}'>here</a>​ to join.";
            string bodyKh = $"អ្នកបានអញ្ជើញឱ្យចូលរួម Skai Account សម្រាប់ក្រុមហ៊ុន {organisation}។ សូមចុច <a href='{webUrl}'>ត្រង់នេះ</a>​ ដើម្បីចូលរួម។";
            await emailService.SendEmailAsync(new List<string> { inviteUser.Email }, inviteUser.DisplayName, subject, body, bodyKh);
        }

        public void UpdateUser(User user)
        {
            using var context = Context();
        
            var claimName = context.AspNetUserClaims.FirstOrDefault(u => u.UserId == user.Id && u.ClaimType == ClaimTypes.Name);
            if (claimName != null)
            {
                claimName.ClaimValue = user.Name;
            }
            else
            {
                context.AspNetUserClaims.Add(new Dal.Models.AspNetUserClaims
                {
                    ClaimType = ClaimTypes.Name,
                    ClaimValue = user.Name,
                    UserId = user.Id
                });
            }
            var claimPhone = context.AspNetUserClaims.FirstOrDefault(u => u.UserId == user.Id && u.ClaimType == ClaimTypes.MobilePhone);
            if (claimPhone != null)
            {
                claimPhone.ClaimValue = user.PhoneNumber;
            }
            else
            {
                context.AspNetUserClaims.Add(new Dal.Models.AspNetUserClaims
                {
                    ClaimType = ClaimTypes.MobilePhone,
                    ClaimValue = user.PhoneNumber,
                    UserId = user.Id
                });
            }
            context.SaveChanges();
        }

        public void UpdateUserLanguage(User user)
        {
            using var context = Context();
            var claimLanguage = context.AspNetUserClaims.FirstOrDefault(u => u.UserId == user.Id && u.ClaimType == "Language");
            if (claimLanguage != null)
            {
                claimLanguage.ClaimValue = user.Language;
                context.SaveChanges();
                return;
            }
            context.AspNetUserClaims.Add(new Dal.Models.AspNetUserClaims
            {
                ClaimValue=user.Language,
                ClaimType= "Language",
                UserId=user.Id
            });
            context.SaveChanges();
        }

        public async Task UpdateUserRoleAsync(string organisationId, string updateByUserId, OrganisationUser organisationUser)
        {
            using var context = Context();
            if (!context.OrganisationUserMenuFeature.Any(u => u.OrganisationId == organisationId && u.UserId == updateByUserId && u.MenuFeatureId == (int)MenuFeatureEnum.ManageOrganisactionSetting))
            {
                throw new Exception("You don't have permission to update user role");
            }
            var user = context.OrganisationUser.FirstOrDefault(u => u.OrganisationId == organisationId && u.User.Email.ToLower() == organisationUser.User.Email.ToLower());
            if (user != null)
            {
                if (user.IsOwner)
                {
                    throw new Exception("You cannot update role for owner user");
                }
                user.RoleName = organisationUser.RoleName;
                context.OrganisationUserMenuFeature.RemoveRange(context.OrganisationUserMenuFeature.Where(u => u.OrganisationId == organisationId && u.UserId == user.UserId));
                foreach(var t in organisationUser.MenuFeatures)
                {
                    if (t.IsCheck)
                    {
                        context.OrganisationUserMenuFeature.Add(new Dal.Models.OrganisationUserMenuFeature
                        {
                            MenuFeatureId=t.Id,
                            OrganisationId=organisationId,
                            UserId=user.UserId
                        });
                    }
                }
                context.SaveChanges();
                await emailService.SendEmailAsync(user.User.Email, "Organisation role change", "Organisation role change", true);
                return;
            }
            var invitingUser = context.OrganisationInvitingUser.FirstOrDefault(u => u.OrganisationId == organisationId && u.Email.ToLower() == organisationUser.User.Email.ToLower());
            if (invitingUser != null)
            {
                invitingUser.RoleName = organisationUser.RoleName;
                context.OrganisationInvitingUserMenuFeature.RemoveRange(context.OrganisationInvitingUserMenuFeature.Where(u => u.OrganisationId == organisationId && u.Email.ToLower() == organisationUser.User.Email.ToLower()));
                foreach(var t in organisationUser.MenuFeatures)
                {
                    if (t.IsCheck)
                    {
                        context.OrganisationInvitingUserMenuFeature.Add(new Dal.Models.OrganisationInvitingUserMenuFeature
                        {
                            MenuFeatureId=t.Id,
                            OrganisationId=organisationId,
                            Email=organisationUser.User.Email.ToLower()
                        });
                    }
                }
                context.SaveChanges();
                return;
            }
            throw new Exception("User doesn't exist");
        }
    }
}
