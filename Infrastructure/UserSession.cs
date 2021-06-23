using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class UserSession : IUserSession
    {
        private readonly IHttpContextAccessor _accessor;

        public UserSession(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }
        public string Name => _accessor.HttpContext.User.Identity.Name;
        public string Email => _accessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Email")?.Value;
        public string Login => _accessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Login")?.Value;
        public long Id => Convert.ToInt64(_accessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value);
        public string Role => _accessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        public bool IsAuthenticated()
        {
            return _accessor.HttpContext.User.Identity.IsAuthenticated;
        }

        public IEnumerable<Claim> GetClaimsIdentity()
        {
            return _accessor.HttpContext.User.Claims;
        } 

    }
}
