using System.Text;
using Common.DTO.Response;
using Common.Options;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Domain.Entities;

namespace Infrastructure.Middleware
{
    public class JwtHandler : IJwtHandler
    {
        private readonly JwtOptions _options;
        private readonly AppOptions _AppOptions;
        private readonly SecurityKey _securityKey;
        private readonly SigningCredentials _signingCredentials;
        private string apiUrl = string.Empty;

        public JwtHandler(
            IOptions<JwtOptions> options,
            IOptions<AppOptions> AppOptions
            )
        {
            _AppOptions = AppOptions.Value;
            _options = options.Value;
            _securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
            _signingCredentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256);
            apiUrl = _AppOptions.ApiUrl;
        }

        public JsonWebToken Create(ApplicationUser user,IList<Claim> claims)
        {
            var nowUtc = DateTime.Now;
            var expires = nowUtc.AddMinutes(_options.ExpiryMinutes);
            var centuryBegin = new DateTime(1970, 1, 1).ToUniversalTime();
            var exp = (long)(new TimeSpan(expires.Ticks - centuryBegin.Ticks).TotalSeconds);
            var iat = (long)(new TimeSpan(nowUtc.Ticks - centuryBegin.Ticks).TotalSeconds);
            var token = new JwtSecurityToken(
                            issuer: apiUrl,
                            audience: apiUrl,
                            claims: claims,
                            expires: expires,
                            signingCredentials: _signingCredentials);
            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            return new JsonWebToken
            {
                AccessToken = accessToken,
                Expires = expires,
                Issue = nowUtc
            };
        }
    }
}
