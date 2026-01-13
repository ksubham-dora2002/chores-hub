using AutoMapper;
using System.Text;
using System.Security.Claims;
using ChoresHub.Domain.Entities;
using ChoresHub.Application.DTOs;
using ChoresHub.Domain.Interfaces;
using System.Security.Cryptography;
using ChoresHub.Application.Common;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using ChoresHub.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using ChoresHub.Application.Helpers;


namespace ChoresHub.Application.Services
{
    public class TokenService : ITokenService
    {
        private readonly string _issuer;
        private readonly IMapper _mapper;
        private readonly string _audience;
        private readonly string _secretKey;
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private const int _ExpiryDurationMinutes = 3;
        private const int _RefreshTokenExpiryDays = 1;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private static readonly TimeSpan AccessTokenLifetime = TimeSpan.FromMinutes(_ExpiryDurationMinutes);
        private static readonly TimeSpan RefreshTokenLifetime = TimeSpan.FromDays(_RefreshTokenExpiryDays);

        public TokenService(IConfiguration configuration, IRefreshTokenRepository refreshTokenRepository, IUserRepository userRepository, IMapper mapper)
        {
            _mapper = mapper;
            _configuration = configuration;
            _userRepository = userRepository;
            _issuer = _configuration["JwtSettings:Issuer"]!;
            _refreshTokenRepository = refreshTokenRepository;
            _audience = _configuration["JwtSettings:Audience"]!;
            _secretKey = _configuration["JwtSettings:SecretKey"]!;
        }
        private string CreateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: BuildClaims(user),
                expires: DateTime.UtcNow.Add(AccessTokenLifetime),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static Claim[] BuildClaims(User user)
        {
            return [
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            ];
        }

        public async Task<Result<string>> GenerateAccessTokenAsync(UserDTO userDto)
        {
            var user = await _userRepository.GetUserByEmailAsync(userDto.Email);
            if (user == null) return Result<string>.Failure(UserMessages.UserNotFoundByEmail);

            return Result<string>.Success(CreateJwtToken(user));
        }

        public async Task<Result<string>> RevokeAllRefreshTokensAsync(string email)
        {
            var tokens = await _refreshTokenRepository.GetTokensByEmailAsync(email);
            if (tokens == null || !tokens.Any()) return Result<string>.Failure("No refresh tokens found to revoke.");

            foreach (var token in tokens)
            {
                token.IsRevoked = true;
                await _refreshTokenRepository.UpdateAsyncRefreshToken(token);
            }

            return Result<string>.Success("All refresh tokens revoked successfully.");
        }
        public async Task<Result<RefreshToken>> GenerateRefreshTokenAsync(UserDTO userDTO)
        {
            var user = await _userRepository.GetUserByEmailAsync(userDTO.Email);
            if (user == null) return Result<RefreshToken>.Failure(UserMessages.UserNotFoundByEmail);

            var randomBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);

            var refreshToken = new RefreshToken()
            {
                Id = Guid.NewGuid(),
                Token = Convert.ToBase64String(randomBytes),
                UserId = user.Id,
                User = user,
                UserEmail = user.Email,
                ExpiresAt = DateTime.UtcNow.AddDays(_RefreshTokenExpiryDays),
                IsRevoked = false
            };
            await _refreshTokenRepository.CreateAsync(refreshToken);
            return Result<RefreshToken>.Success(refreshToken);
        }
        public async Task<Result<TokenPairDTO>> RefreshAsync(string refToken)
        {
            var stored = await _refreshTokenRepository.FindRefreshTokenByTokenAsync(refToken);
            if (stored == null || stored.ExpiresAt < DateTime.UtcNow || stored.IsRevoked)
                return Result<TokenPairDTO>.Failure("Invalid or expired refresh token.");

            var user = await _userRepository.GetByIdAsync(stored.UserId);
            if (user == null) return Result<TokenPairDTO>.Failure(UserMessages.UserNotFoundById);

            var userDto = _mapper.Map<UserDTO>(user);

            // WITHOUT ROTATING
            var newAccessToken = await GenerateAccessTokenAsync(userDto);
            if (!newAccessToken.IsSuccess) return Result<TokenPairDTO>.Failure(newAccessToken.Error!);

            return Result<TokenPairDTO>.Success(new TokenPairDTO
            {
                AccessToken = newAccessToken.Value!,
                RefreshTokenString = stored.Token,
                UserId = user.Id
            });
        }
        public async Task<Result<string>> RevokeRefreshTokenAsync(string refreshToken)
        {
            var stored = await _refreshTokenRepository.FindRefreshTokenByTokenAsync(refreshToken);
            if (stored == null || stored.IsRevoked) return Result<string>.Failure("Refresh token not found or already revoked.");
            stored.IsRevoked = true;
            await _refreshTokenRepository.UpdateAsyncRefreshToken(stored);
            return Result<string>.Success("Refresh token revoked successfully.");
        }
    }
}