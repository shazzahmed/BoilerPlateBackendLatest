using Domain.Entities;

namespace Domain.RepositoriesContracts
{
    public interface IRefreshTokenRepository : IBaseRepository<RefreshToken, int>
    {
        Task<bool> AddRefreshToken(RefreshToken token);
        Task<bool> RemoveRefreshToken(RefreshToken refreshToken);
        Task<bool> RemoveRefreshToken(List<RefreshToken> refreshToken);
        Task<bool> RemoveRefreshTokenByID(string refreshTokenId);
        Task<RefreshToken> FindRefreshToken(string refreshTokenId);
        Task<List<RefreshToken>> GetAllRefreshTokensAsync();
        Task<RefreshToken> GetRefreshTokenByEmailAsync(string userEmail);
        Task<List<RefreshToken>> GetRefreshTokensByEmail(string userEmail);
        Task<RefreshToken> GetRefreshTokenByEmailAndRefreshTokenIdAsync(string userEmail, string refreshTokenId);
        Task<RefreshToken> GetRefreshToken(string token);
        string GetRefreshTokenId(String userId);
        
        // New methods for multi-device support
        Task<RefreshToken> GetRefreshTokenByDevice(string username, string deviceId);
        Task<List<RefreshToken>> GetActiveRefreshTokensByUsername(string username);
        Task<bool> RevokeRefreshTokenByDevice(string username, string deviceId);
        Task<bool> RevokeAllRefreshTokensForUser(string username);
    }
}
