using System.Security.Claims;
using Common.DTO.Response;
using Domain.Entities;

namespace Infrastructure.Middleware
{
    public interface IJwtHandler
    {
        JsonWebToken Create(ApplicationUser user,IList<Claim> claims);
    }
}
