using Domain.Entities;
using Infrastructure.Database;
using Domain.RepositoriesContracts;
using Common.DTO.Response;
using Microsoft.AspNetCore.Identity;
using static Common.Utilities.Enums;

namespace Infrastructure.Repository
{
    public class RefreshTokenRepository : BaseRepository<RefreshToken, int>, IRefreshTokenRepository
    {
        protected readonly UserManager<ApplicationUser> _userManager;
        public RefreshTokenRepository(ISqlServerDbContext context, UserManager<ApplicationUser> userManager) : base(context)
        {
            _userManager = userManager;
        }
        public async Task<bool> AddRefreshToken(RefreshToken token)
        {
            var user = await _userManager.FindByIdAsync(token.Id);
            if (user != null)
            {
                // Check if a token already exists for this specific device
                var existingToken = await FirstOrDefaultAsync(r => r.Username == token.Username && r.DeviceId == token.DeviceId);
                if (existingToken != null)
                {
                    // Update existing token for this device instead of removing it
                    existingToken.Token = token.Token;
                    existingToken.Expired = token.Expired;
                    existingToken.Issued = DateTime.Now;
                    existingToken.Revoked = false;
                    await UpdateAsync(existingToken);
                }
                else
                {
                    // Add new token for this device
                    await AddAsync(token);
                }
            }
            else
            {
                // Fallback: add token even if user not found (for backward compatibility)
                await AddAsync(token);
            }
            return await DbContext.SaveChangesAsync() > 0;
        }
        public async Task<bool> RemoveRefreshToken(RefreshToken refreshToken)
        {
            await DeleteAsync(refreshToken);
            return await DbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveRefreshToken(List<RefreshToken> refreshToken)
        {
            await DeleteAsync(x=>x.Revoked);
            return await DbContext.SaveChangesAsync() > 0;
        }


        //Remove the Refesh Token by id
        public async Task<bool> RemoveRefreshTokenByID(string refreshTokenId)
        {
            var refreshToken = await FirstOrDefaultAsync(x=> x.RefreshTokenId == int.Parse(refreshTokenId));
            if (refreshToken != null)
            {
                await DeleteAsync(refreshToken);
                return await DbContext.SaveChangesAsync() > 0;
            }
            return false;
        }
        public async Task<RefreshToken> FindRefreshToken(string refreshTokenId)
        {
            var refreshToken = await FirstOrDefaultAsync(x => x.RefreshTokenId == int.Parse(refreshTokenId));
            return refreshToken;
        }
        public async Task<List<RefreshToken>> GetAllRefreshTokensAsync()
        {
            return await GetAsync();
        }

        public async Task<RefreshToken> GetRefreshTokenByEmailAsync(string userEmail)
        {
            return await FirstOrDefaultAsync(x => x.Username == userEmail);
        }

        public async Task<List<RefreshToken>> GetRefreshTokensByEmail(string userEmail)
        {
            return await GetAsync(x => x.Username == userEmail);
        }
        public async Task<RefreshToken> GetRefreshTokenByEmailAndRefreshTokenIdAsync(string userEmail, string refreshTokenId)
        {
            var refreshToken = await FirstOrDefaultAsync(x => x.Username == userEmail && x.RefreshTokenId == int.Parse(refreshTokenId));
            return refreshToken;
        }
        public async Task<RefreshToken> GetRefreshToken(string token)
        {
            var refreshToken = await FirstOrDefaultAsync(x => x.Token == token);
            return refreshToken;
        }
        public string GetRefreshTokenId(String userId)
        {
            var data = Get().Join(
               DbContext.User,
               rt => rt.Username,
               u => u.UserName,
               (rt, u) => new
               {
                   RefreshTokenId = rt.RefreshTokenId, // Updated to use new primary key
                   userId = u.Id
               }).FirstOrDefault(x => x.userId == userId);
            string refreshTokenId = data?.RefreshTokenId.ToString();
            return refreshTokenId;
        }

        // New method to get refresh token by device
        public async Task<RefreshToken> GetRefreshTokenByDevice(string username, string deviceId)
        {
            return await FirstOrDefaultAsync(x => x.Username == username && x.DeviceId == deviceId);
        }

        // New method to get all active refresh tokens for a user
        public async Task<List<RefreshToken>> GetActiveRefreshTokensByUsername(string username)
        {
            return await GetAsync(x => x.Username == username && !x.Revoked && x.Expired > DateTime.Now);
        }

        // New method to revoke refresh token by device
        public async Task<bool> RevokeRefreshTokenByDevice(string username, string deviceId)
        {
            var refreshToken = await FirstOrDefaultAsync(x => x.Username == username && x.DeviceId == deviceId);
            if (refreshToken != null)
            {
                refreshToken.Revoked = true;
                await UpdateAsync(refreshToken);
                return await DbContext.SaveChangesAsync() > 0;
            }
            return false;
        }

        // New method to revoke all refresh tokens for a user (useful for logout from all devices)
        public async Task<bool> RevokeAllRefreshTokensForUser(string username)
        {
            var refreshTokens = await GetAsync(x => x.Username == username && !x.Revoked);
            foreach (var token in refreshTokens)
            {
                token.Revoked = true;
                await UpdateAsync(token);
            }
            return await DbContext.SaveChangesAsync() > 0;
        }
    }
}
