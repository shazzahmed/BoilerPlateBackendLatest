using Domain.Entities;
using Common.DTO.Response;

namespace Application.ServiceContracts
{
    public interface IRefreshTokenService : IBaseService<RefreshTokenModel, RefreshToken, int>
    {
        Task<bool> AddRefreshToken(RefreshTokenModel token);
        Task<bool> RemoveRefreshTokenByID(string refreshTokenId);
        Task<bool> RemoveRefreshToken(RefreshTokenModel refreshToken);
        Task<bool> RemoveRefreshToken(List<RefreshTokenModel> refreshToken);
        Task<RefreshTokenModel> FindRefreshToken(string refreshTokenId);
        Task<List<RefreshTokenModel>> GetAllRefreshTokens();
        Task<RefreshTokenModel> GetRefreshTokenByEmail(string userEmail);
        Task<List<RefreshTokenModel>> GetRefreshTokensByEmail(string userEmail);
        Task<RefreshTokenModel> GetRefreshTokenByEmailAndRefreshTokenId(string userEmail, string refreshTokenId);
        
        // New methods for multi-device support
        Task<RefreshTokenModel> GetRefreshTokenByDevice(string username, string deviceId);
        Task<List<RefreshTokenModel>> GetActiveRefreshTokensByUsername(string username);
        Task<bool> RevokeRefreshTokenByDevice(string username, string deviceId);
        Task<bool> RevokeAllRefreshTokensForUser(string username);
    }
}
