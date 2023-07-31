using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.SignalR;
using ProductivityKeeperWeb.Domain;
using ProductivityKeeperWeb.Domain.Interfaces;
using System;
using System.Linq;

namespace ProductivityKeeperWeb.Services
{
    public class AuthService: IAuthService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuthService(IHttpContextAccessor httpContextAccessor) 
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int GetUnitId()
        {
            var context = _httpContextAccessor?.HttpContext;
            int result;
            if (int.TryParse(
                context.User.Claims
                    .FirstOrDefault(claim => claim.Type == Constants.UnitIdClaim)
                    ?.Value,
                out result))
            {
                return result;
            }

            throw new UnauthorizedAccessException("Cannot find claim of Unit Id");
        }

        public string GetUserEmail()
        {
            var context = _httpContextAccessor?.HttpContext;
            return context.User.Identity.Name;
        }
    }
}
