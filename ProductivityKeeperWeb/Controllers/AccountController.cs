using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using ProductivityKeeperWeb.Data;
using ProductivityKeeperWeb.Domain;
using ProductivityKeeperWeb.Domain.Interfaces;
using ProductivityKeeperWeb.Domain.Models;
using ProductivityKeeperWeb.Domain.Models.TaskRelated;
using ProductivityKeeperWeb.Domain.Utils;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProductivityKeeperWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationContext _context;
        // TODO: Remove because move to authservice
        private readonly ITasksWriteService _tasksWriteService;
        public AccountController(ApplicationContext context, ITasksWriteService tasksWriteService)
        {
            _context = context;
            _tasksWriteService = tasksWriteService;
        }

        [HttpPost("/token")]
        public async Task<ActionResult<object>> GetToken(string username, string password)
        {
            var identity = await GetIdentity(username, password);
            if (identity == null)
            {
                return BadRequest(new { message = "Invalid username or password." });
            }

            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return Ok(new { accessToken = encodedJwt, lifeTime = AuthOptions.LIFETIME });
        }

        [HttpPost("/registration")]
        public async Task<ActionResult<object>> Registrate(AbstractUser user)
        {
            if (await UserExists(user.Email))
                return BadRequest(new { message = "User with this email already exists" });

            var unit = await _tasksWriteService.AddUnitForNewCommer(user.Email);

            var concerteUser = new User
            {
                Email = user.Email,
                HashPassword = user.HashPassword,
                Role = Domain.Models.User.Roles.User,
                RegistrationDate = DateTime.UtcNow,
                UnitId = unit.Id
            };

            await _context.Users.AddAsync(concerteUser);
            await _context.SaveChangesAsync();

            return await GetToken(user.Email, user.HashPassword);
        }

        [Authorize]
        [HttpPost("checkPasswordIfMatch")]
        public async Task<ActionResult<bool>> CheckOnPasswordMatch(string password)
        {
            var user = await _context.Users.FindAsync(User.Identity.Name);
            return user.HashPassword == password;
        }

        [Authorize]
        [HttpPost("changePassword")]
        public async Task<IActionResult> ChangePasssword(string newPassword)
        {
            var user = await _context.Users.FindAsync(User.Identity.Name);

            _context.Entry(user).State = EntityState.Modified;
            user.HashPassword = newPassword;
            await _context.SaveChangesAsync();
            return Ok();
        }


        [Authorize]
        [HttpPost("/refresh-token")]
        public async Task<ActionResult<object>> RefreshToken()
        {
            var now = DateTime.UtcNow;
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == User.Identity.Name);
            var claims = new System.Collections.Generic.List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.ToString())
                };
            ClaimsIdentity claimsIdentity =
            new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);


            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return Ok(new { accessToken = encodedJwt, lifeTime = AuthOptions.LIFETIME });
        }

        private async Task<ClaimsIdentity> GetIdentity(string email, string password)
        {
            User user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email && x.HashPassword == password);
            if (user != null)
            {
                var claims = new System.Collections.Generic.List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.ToString()),
                    new Claim(Constants.UnitIdClaim, user.UnitId.ToString())
                };
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }

            return null;
        }

        private async Task<bool> UserExists(string email)
        {
            var res = await _context.Users.FindAsync(email);
            return res is not null;
        }
    }
}

