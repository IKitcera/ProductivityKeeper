using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProductivityKeeperWeb.Data;
using ProductivityKeeperWeb.Models;
using ProductivityKeeperWeb.Models.TaskRelated;
using ProductivityKeeperWeb.Services;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProductivityKeeperWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly ITaskPageHelper _helper;
        public AccountController(ApplicationContext context, ITaskPageHelper helper )
        {
            _context = context;
            _helper = helper;
        }

        [HttpPost("/token")]
        public async Task<ActionResult<string>> GetToken(string username, string password)
        {
            var identity = await GetIdentity(username, password);
            if (identity == null)
            {
                return BadRequest(new { message = "Invalid username or password." });
            }


            _helper.User = identity;

            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                    issuer: Models.AuthOptions.ISSUER,
                    audience: Models.AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(Models.AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return Ok(new { accessToken = encodedJwt, lifeTime = Models.AuthOptions.LIFETIME });
        }

        [HttpPost("/registration")]
        public async Task<IActionResult> Registrate(AbstractUser user)
        {
            if (await UserExists(user.Email))
                return BadRequest(new { message = "User with this email already exists" });

            var unit = new Unit { UserId = user.Email };
            await _context.Units.AddAsync(unit);
            await _context.SaveChangesAsync();

            var theUnit = await _context.Units.FirstOrDefaultAsync(u => u.UserId == user.Email);

            var concerteUser = new User
            {
                Email = user.Email,
                HashPassword = user.HashPassword,
                Role = Models.User.Roles.User,
                RegistrationDate = DateTime.UtcNow,
                UnitId = theUnit.Id
            };

            await _context.Users.AddAsync(concerteUser);
            await _context.SaveChangesAsync();
           
            return Ok();
        }

        [Authorize]
        [HttpPost("/refresh-token")]
        public async Task<ActionResult<string>> RefreshToken()
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

            // JWT-token
            var jwt = new JwtSecurityToken(
                    issuer: Models.AuthOptions.ISSUER,
                    audience: Models.AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(Models.AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
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
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.ToString())
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

