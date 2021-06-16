using System.Collections.Generic;
using System.Security.Claims;


namespace Core.Interfaces
{
    public interface IUserSession
    {
        long Id { get; }
        string Name { get; }
        string Email { get; }
        string Login { get; }
        bool IsAuthenticated();
        IEnumerable<Claim> GetClaimsIdentity();
    }
}
