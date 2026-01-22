using System.Security.Claims;
using IdentityModel;

namespace Common.Extensions
{
    public static class UserExtensions
    {
        /// <summary>
        /// Retrieve a claim by name and indicates if it succeeded.
        /// </summary>
        /// <param name="user">The current User.</param>
        /// <param name="claimName">Queried claim name.</param>
        /// <param name="claimValue">The value of the claim.</param>
        /// <returns>True if the claim exists with a value.</returns>
        public static bool TryGetUserClaims(this ClaimsPrincipal user, string claimName, out string claimValue)
        {
            claimValue = user?.Claims?.FirstOrDefault(c => c.Type == claimName)?.Value;
            return !string.IsNullOrWhiteSpace(claimValue);
        }

        /// <summary>
        /// Get user ID from the claims principal.
        /// </summary>
        /// <param name="user">Claims principal representing the user.</param>
        /// <param name="userId">User ID obtained from the claims principal.</param>
        /// <returns>True if it could find the user ID.</returns>
        public static bool TryGetUserId(this ClaimsPrincipal user, out Guid userId)
        {
            if (user.TryGetUserClaims(JwtClaimTypes.Id, out var userIdString) &&
                Guid.TryParse(userIdString, out userId))
            {
                return true;
            }

            userId = Guid.Empty;
            return false;
        }

        /// <summary>
        /// Get access token from claims.
        /// </summary>
        public static bool TryGetAccessToken(this ClaimsPrincipal user, out string accessToken) =>
            user.TryGetUserClaims("access_token", out accessToken);

        /// <summary>
        /// Get ID token from claims.
        /// </summary>
        public static bool TryGetIdToken(this ClaimsPrincipal user, out string idToken) =>
            user.TryGetUserClaims("id_token", out idToken);

        /// <summary>
        /// Get external ID (name identifier) from claims.
        /// </summary>
        public static bool TryGetExternalId(this ClaimsPrincipal user, out string externalId) =>
            user.TryGetUserClaims(ClaimTypes.NameIdentifier, out externalId);

        /// <summary>
        /// Get the primary SID from the claims.
        /// </summary>
        public static string GetAccessToken(this ClaimsPrincipal principal) =>
            principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.PrimarySid)?.Value;

        /// <summary>
        /// Get the user ID from the claims.
        /// </summary>
        public static string GetUserId(this ClaimsPrincipal principal) =>
            principal.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Id)?.Value;

        /// <summary>
        /// Get the name identifier from claims.
        /// </summary>
        public static string GetName(this ClaimsPrincipal principal) =>
            principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        /// <summary>
        /// Get the user name from claims.
        /// </summary>
        public static string GetUserName(this ClaimsPrincipal principal) =>
            principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

        /// <summary>
        /// Get the user email from claims.
        /// </summary>
        public static string GetUserEmail(this ClaimsPrincipal principal) =>
            principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        /// <summary>
        /// Get the full name of the user (combines first and last name from claims).
        /// </summary>
        public static string GetUserFullName(this ClaimsPrincipal principal)
        {
            var firstName = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value;
            var lastName = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value;

            return $"{firstName} {lastName}";
        }

        /// <summary>
        /// Get the user's first name from claims.
        /// </summary>
        public static string GetUserFirstName(this ClaimsPrincipal principal) =>
            principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value;

        /// <summary>
        /// Get the user's last name from claims.
        /// </summary>
        public static string GetUserLastName(this ClaimsPrincipal principal) =>
            principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value;

        /// <summary>
        /// Get the user's role from claims.
        /// </summary>
        public static string GetUserRole(this ClaimsPrincipal principal) =>
            principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

        /// <summary>
        /// Get the user's roles from claims.
        /// </summary>
        public static List<string> GetUserRoles(this ClaimsPrincipal principal) =>
            principal.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).Distinct().ToList();

        /// <summary>
        /// Check if the user is in a specific role.
        /// </summary>
        /// <param name="principal">The claims principal.</param>
        /// <param name="role">The role to check.</param>
        /// <returns>True if the user is in the specified role.</returns>
        public static bool UserIsInRole(this ClaimsPrincipal principal, string role)
        {
            var actualRole = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            return actualRole == role;
        }

        /// <summary>
        /// Retrieve all claims for the user.
        /// </summary>
        public static IList<Claim> GetUserClaims(this ClaimsPrincipal user, out IList<Claim> claimsValue) =>
            claimsValue = user?.Claims?.ToList() ?? new List<Claim>();
    }
}

