using AutoMapper;
using Domain.Entities;
using Domain.RepositoriesContracts;
using System;
using System.Collections.Generic;
using System.Text;
using Common.DTO.Response;using Common.DTO.Request;
using System.Threading.Tasks;
using Application.ServiceContracts;
using Infrastructure.Services.Communication;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.Services
{
    public class RefreshTokenService : BaseService<RefreshTokenModel, RefreshToken, int>, IRefreshTokenService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public RefreshTokenService(
            IMapper mapper,
            IRefreshTokenRepository refreshTokenRepository, 
            IUnitOfWork unitOfWork,
            SseService sseService
            ) : base(mapper, refreshTokenRepository, unitOfWork, sseService)
        {
            _refreshTokenRepository = refreshTokenRepository;
        }
        public async Task<bool> AddRefreshToken(RefreshTokenModel token)
        {
            return await _refreshTokenRepository.AddRefreshToken(mapper.Map<RefreshTokenModel, RefreshToken>(token));
        }

        public async Task<bool> RemoveRefreshTokenByID(string refreshTokenId)
        {
            return await _refreshTokenRepository.RemoveRefreshTokenByID(refreshTokenId);
        }

        public async Task<bool> RemoveRefreshToken(RefreshTokenModel refreshToken)
        {
            return await _refreshTokenRepository.RemoveRefreshToken(mapper.Map<RefreshTokenModel, RefreshToken>(refreshToken));
        }

        public async Task<bool> RemoveRefreshToken(List<RefreshTokenModel> refreshToken)
        {
            return await _refreshTokenRepository.RemoveRefreshToken(mapper.Map<List<RefreshTokenModel>, List<RefreshToken>>(refreshToken));
        }

        public async Task<RefreshTokenModel> FindRefreshToken(string refreshTokenId)
        {
            var result = await _refreshTokenRepository.FindRefreshToken(refreshTokenId);
            return mapper.Map<RefreshToken, RefreshTokenModel>(result);
        }

        public async Task<List<RefreshTokenModel>> GetAllRefreshTokens()
        {
            var result = await _refreshTokenRepository.GetAllRefreshTokensAsync();
            return mapper.Map<List<RefreshToken>, List<RefreshTokenModel>>(result);
        }

        public async Task<RefreshTokenModel> GetRefreshTokenByEmail(string userEmail)
        {
            var result = await _refreshTokenRepository.GetRefreshTokenByEmailAsync(userEmail);
            return mapper.Map<RefreshToken, RefreshTokenModel>(result);
        }

        public async Task<List<RefreshTokenModel>> GetRefreshTokensByEmail(string userEmail)
        {
            var result = await _refreshTokenRepository.GetRefreshTokensByEmail(userEmail);
            return mapper.Map<List<RefreshToken>, List<RefreshTokenModel>>(result);
        }
        public async Task<RefreshTokenModel> GetRefreshTokenByEmailAndRefreshTokenId(string userEmail, string refreshTokenId)
        {
            var result = await _refreshTokenRepository.GetRefreshTokenByEmailAndRefreshTokenIdAsync(userEmail, refreshTokenId);
            return mapper.Map<RefreshToken, RefreshTokenModel>(result);
        }

        // New methods for multi-device support
        public async Task<RefreshTokenModel> GetRefreshTokenByDevice(string username, string deviceId)
        {
            var result = await _refreshTokenRepository.GetRefreshTokenByDevice(username, deviceId);
            return mapper.Map<RefreshToken, RefreshTokenModel>(result);
        }

        public async Task<List<RefreshTokenModel>> GetActiveRefreshTokensByUsername(string username)
        {
            var result = await _refreshTokenRepository.GetActiveRefreshTokensByUsername(username);
            return mapper.Map<List<RefreshToken>, List<RefreshTokenModel>>(result);
        }

        public async Task<bool> RevokeRefreshTokenByDevice(string username, string deviceId)
        {
            return await _refreshTokenRepository.RevokeRefreshTokenByDevice(username, deviceId);
        }

        public async Task<bool> RevokeAllRefreshTokensForUser(string username)
        {
            return await _refreshTokenRepository.RevokeAllRefreshTokensForUser(username);
        }
    }
}

