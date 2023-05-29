using Microsoft.AspNetCore.Http;
using ProductivityKeeperWeb.Domain;
using ProductivityKeeperWeb.Domain.Interfaces;
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
            int result;
            if (int.TryParse(
                _httpContextAccessor.HttpContext.User.Claims
                    .FirstOrDefault(claim => claim.Type == Constants.UnitIdClaim)
                    ?.Value,
                out result))
            {
                return result;
            }
            throw new System.Exception("Cannot find claim of Unit Id");
        }
    }
}
