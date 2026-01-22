using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Newtonsoft.Json;
using Common.Extensions;
using System.Security.Claims;
using Application.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using IdentityModel;

namespace Infrastructure.Helpers;
public class AuthenticationHelper : IAuthenticationHelper
{
    /// <summary>
    /// Extracts list of roles from the access token. The code assumes that roles are returned as part of the Auth0 token.
    /// </summary>
    /// <param name="accessToken">Auth0 generated access token.</param>
    /// <returns>List of roles.</returns>
    public IEnumerable<string> GetRoles(JwtSecurityToken accessToken)
    {
        var rolesClaim = accessToken?.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Role);
        return rolesClaim != null ? rolesClaim.Value.Split(',') : Array.Empty<string>();
    }

    /// <summary>
    /// Decodes a JWT token.
    /// </summary>
    /// <param name="token">Token to decode.</param>
    /// <param name="key">Encryption key for the token.</param>
    /// <returns>Returns the content of the token.</returns>
    public string Decode(string token, string key)
    {
        var serializer = new JsonNetSerializer();
        var provider = new UtcDateTimeProvider();
        var validator = new JwtValidator(serializer, provider);
        var urlEncoder = new JwtBase64UrlEncoder();
        var decoder = new JwtDecoder(serializer, validator, urlEncoder, new HMACSHA256Algorithm());

        return decoder.Decode(token, key, verify: true);
    }

    /// <summary>
    /// Encodes a JWT token.
    /// </summary>
    /// <param name="payload">Payload to encode into the token.</param>
    /// <param name="key">Signing key.</param>
    /// <returns>Returns an encoded token.</returns>
    public string Encode(object payload, string key)
    {
        var encoder = new JwtEncoder(new HMACSHA256Algorithm(), new JsonNetSerializer(), new JwtBase64UrlEncoder());
        return encoder.Encode(payload, key);
    }

    /// <summary>
    /// Validates an Auth0 token. Throws exceptions if validation fails.
    /// </summary>
    /// <param name="token">Token from Auth0.</param>
    /// <param name="domain">Auth0 domain.</param>
    /// <param name="audiences">Audience claims.</param>
    /// <param name="apiIssuer">Auth0 API issuer.</param>
    /// <param name="clientSecret">Client secret used to generate the token.</param>
    public void ValidateApiToken(string token, string domain, IList<string> audiences, string apiIssuer, string clientSecret)
    {
        var jwtHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidIssuer = domain,
            ValidAudiences = audiences,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            ValidIssuers = new List<string> { apiIssuer }
        };

        try
        {
            jwtHandler.ValidateToken(token, validationParameters, out _);
        }
        catch (SecurityTokenException ex)
        {
            // Optionally log the exception or handle it as needed
            throw new InvalidOperationException("Token validation failed.", ex);
        }
    }

    /// <summary>
    /// Converts JSON into a collection of claims.
    /// </summary>
    /// <param name="json">JSON data.</param>
    /// <returns>A collection of claims.</returns>
    public IEnumerable<Claim> GetUserClaims(string json)
    {
        var items = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        return items.Select(item => new Claim(item.Key, item.Value)).ToList();
    }

    /// <summary>
    /// Reads the claim value from JwtSecurityToken.
    /// </summary>
    /// <param name="token">JwtSecurityToken.</param>
    /// <param name="claimName">Claim name.</param>
    /// <returns>Claim value.</returns>
    public string ReadTokenClaim(JwtSecurityToken token, string claimName)
    {
        return token?.Claims?.FirstOrDefault(c => c.Type == claimName)?.Value;
    }

    /// <summary>
    /// Gets the Guid from the JWT token.
    /// </summary>
    /// <param name="jsonToken">JSON token.</param>
    /// <param name="clientSecret">Auth0 Client Secret.</param>
    /// <returns>A Guid representing the user ID.</returns>
    public Guid GetGuid(string jsonToken, string clientSecret)
    {
        var json = Decode(jsonToken, clientSecret);
        var principal = new ClaimsPrincipal(new ClaimsIdentity(GetUserClaims(json)));
        principal.TryGetUserId(out Guid value);
        return value;
    }
}
