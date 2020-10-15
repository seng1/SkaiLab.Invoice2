using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SkaiLab.Invoice.Dal;
using SkaiLab.Invoice.Models;

namespace SkaiLab.Invoice.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private UserManager<ApplicationUser> _userManager;
        private readonly Option _option;
        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IOptions<Option> option)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _option = option.Value;
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> Login([FromBody] Login login)
        {
            var user = await _userManager.FindByEmailAsync(login.UserName);
            await _userManager.AddClaimsAsync(user, new List<Claim>
            {
                new Claim(ClaimTypes.Name,"Seng"),
                new Claim(ClaimTypes.Surname,"Orng"),
                new Claim(ClaimTypes.GivenName,"Seng"),
                new Claim(ClaimTypes.Email,"sorksengorng@gmail.com"),
                new Claim(ClaimTypes.Sid,user.Id),

            });
            if (user != null && await _userManager.CheckPasswordAsync(user, login.Password))
            {
                var claims = await _userManager.GetClaimsAsync(user);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    IssuedAt = DateTime.UtcNow,
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_option.JwtSecret)), SecurityAlgorithms.HmacSha256Signature)
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                var token = tokenHandler.WriteToken(securityToken);
                return Ok(new Token { AccessToken = token, Name = claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value, ValidHour = 1 });
            }
            else
            {
                return BadRequest("Username or password is incorrect.");
            }
        }
        [Authorize]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetUser()
        {
            var user = await _userManager.FindByIdAsync(Request.HttpContext.User.FindFirst(ClaimTypes.Sid).Value);
            var claims = await _userManager.GetClaimsAsync(user);
            var result = new User
            {
                Id = user.Id,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Name = claims.First(u => ClaimTypes.Name == u.Type).Value,
                FirstName = claims.First(u => ClaimTypes.GivenName == u.Type).Value,
                LastName = claims.First(u => ClaimTypes.Surname == u.Type).Value,
            };
            return Ok(result);
        }

    }
}
