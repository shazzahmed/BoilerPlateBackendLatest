using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace Application.Interfaces
{
    public interface IAuthenticationHelper
    {
        IEnumerable<string> GetRoles(JwtSecurityToken accessToken);
        string Decode(string token, string key);
        string Encode(object payload, string key);
        void ValidateApiToken(string token, string domain, IList<string> audiences, string apiIssuer, string clientSecret);
        IEnumerable<Claim> GetUserClaims(string json);
        string ReadTokenClaim(JwtSecurityToken token, string claimName);
        Guid GetGuid(string jsonToken, string clientSecret);
    }
}
