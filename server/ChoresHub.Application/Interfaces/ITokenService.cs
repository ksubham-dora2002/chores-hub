using ChoresHub.Domain.Entities;
using ChoresHub.Application.DTOs;
using ChoresHub.Application.Common;

namespace ChoresHub.Application.Interfaces
{
    public interface ITokenService
    {
        Task<Result<string>> GenerateAccessTokenAsync(UserDTO userDto);
        Task<Result<string>> RevokeAllRefreshTokensAsync(string email);
        Task<Result<RefreshToken>> GenerateRefreshTokenAsync(UserDTO userDTO);
        Task<Result<TokenPairDTO>> RefreshAsync(string refToken);
        Task<Result<string>> RevokeRefreshTokenAsync(string refreshToken);
    }
}