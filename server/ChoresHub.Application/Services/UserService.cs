using AutoMapper;
using System.Globalization;
using ChoresHub.Domain.Entities;
using Microsoft.AspNetCore.Http;
using ChoresHub.Application.DTOs;
using ChoresHub.Domain.Interfaces;
using ChoresHub.Application.Common;
using ChoresHub.Application.Helpers;
using ChoresHub.Application.Validators;
using ChoresHub.Application.Interfaces;
using SystemTask = System.Threading.Tasks.Task;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace ChoresHub.Application.Services
{

    public class UserService(IUserRepository userRepository, IMapper mapper, ITokenService tokenService, IUserValidators userValidators, IPasswordResetTokenRepository passwordResetTokenRepository, CloudinaryHelper cloudinaryHelper, IEmailSender emailSender, IConfiguration configuration) : IUserService
    {
        private readonly IMapper _mapper = mapper;
        private readonly ITokenService _tokenService = tokenService;
        private readonly IUserValidators _userValidators = userValidators;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly CloudinaryHelper _cloudinaryHelper = cloudinaryHelper;
        private readonly IPasswordResetTokenRepository _passwordResetTokenRepository = passwordResetTokenRepository;
        private readonly IEmailSender _emailSender = emailSender;
        private readonly IConfiguration _configuration = configuration;
        private readonly DateTime _passwordResetExpiry = DateTime.UtcNow.AddMinutes(15);

        private string  _baseUrl = configuration["ClientApp:BaseUrl"]!;
        private readonly string _subject = "Reset your ChoresHub password";

        public async Task<Result<IList<UserDTO>>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            var usersDto = _mapper.Map<IList<UserDTO>>(users);
            return Result<IList<UserDTO>>.Success(usersDto);
        }
        public async Task<Result<UserDTO>> GetUserByIdAsync(Guid id)
        {
            var isValidUser = await _userValidators.ValidateUserExists(id);
            if (!isValidUser.IsSuccess) return Result<UserDTO>.Failure(UserMessages.UserNotFoundById);
            var user = _mapper.Map<UserDTO>(isValidUser.Value);
            return Result<UserDTO>.Success(user);
        }
        public async Task<Result<UserDTO>> GetUserByEmailAsync(string email)
        {
            var isValidUser = await _userValidators.ValidateUserExists(email);
            if (!isValidUser.IsSuccess) return Result<UserDTO>.Failure(UserMessages.UserNotFoundByEmail);
            var user = _mapper.Map<UserDTO>(isValidUser.Value);
            return Result<UserDTO>.Success(user);
        }
        public async Task<Result<TokenPairDTO>> LoginAsync(LoginDTO loginDTO)
        {
            var isUserExist = await _userValidators.ValidateUserExists(loginDTO.Email);
            if (!isUserExist.IsSuccess) return Result<TokenPairDTO>.Failure(UserMessages.UserNotFoundByEmail);
            var dbUser = isUserExist.Value!;

            if (!BCrypt.Net.BCrypt.Verify(loginDTO.Password, dbUser.Password))
                return Result<TokenPairDTO>.Failure(UserMessages.IncorrectPassword);

            var revokingTokens = await _tokenService.RevokeAllRefreshTokensAsync(dbUser.Email);
            if (!revokingTokens.IsSuccess) return Result<TokenPairDTO>.Failure("An unexpected error occurred while logging in!");

            var accessTokenResult = await AuthenticateUserAsync(dbUser.Email);
            var accessToken = accessTokenResult.Value!;
            var userDto = _mapper.Map<UserDTO>(dbUser);
            var refreshToken = await _tokenService.GenerateRefreshTokenAsync(userDto);

            return Result<TokenPairDTO>.Success(new TokenPairDTO
            {
                AccessToken = accessToken,
                RefreshTokenString = refreshToken.Value!.Token
            });
        }
        public async Task<Result<string>> DeleteUserAsync(Guid id, string password)
        {
            var user = await _userValidators.ValidateUserExists(id);
            if (!user.IsSuccess) return Result<string>.Failure(UserMessages.UserNotFoundById);

            if (!BCrypt.Net.BCrypt.Verify(password, user.Value!.Password))
            {
                return Result<string>.Failure(UserMessages.IncorrectPassword);
            }
            await _userRepository.DeleteAsync(id);
            return Result<string>.Success("User deleted successfully!");
        }
        public async Task<Result<UserDTO>> RegisterAsync(RegisterUserDTO userDTO)
        {
            var isPasswordValid = _userValidators.ValidatePasswordFormat(userDTO.Password);
            if (!isPasswordValid.IsSuccess)
                return Result<UserDTO>.Failure(UserMessages.PasswordPolicy);

            var encryptedPswd = BCrypt.Net.BCrypt.HashPassword(userDTO.Password);

            var isEmailValid = _userValidators.ValidateEmailFormat(userDTO.Email);
            if (!isEmailValid.IsSuccess)
                return Result<UserDTO>.Failure(UserMessages.InvalidEmail);

            var isUserExist = await _userRepository.GetUserByEmailAsync(userDTO.Email.ToLower());
            if (isUserExist != null)
                return Result<UserDTO>.Failure(UserMessages.EmailAlreadyInUse);

            var newUser = new User()
            {
                Id = Guid.NewGuid(),
                Email = userDTO.Email.ToLower(),
                FullName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(userDTO.FullName.ToLower()),
                Password = encryptedPswd,
                ProfilePictureUrl = null,
            };
            try
            {
                await _userRepository.CreateAsync(newUser);
            }
            catch (Exception)
            {
                return Result<UserDTO>.Failure("An unexpected error occurred while creating the user.");
            }

            var userDto = _mapper.Map<UserDTO>(newUser);
            return Result<UserDTO>.Success(userDto);

        }

        public async Task<Result<string>> LogoutAsync(RefreshTokenDTO refreshTokenDTO)
        {
            var revokingToken = await _tokenService.RevokeRefreshTokenAsync(refreshTokenDTO.RefreshTokenString);
            if (!revokingToken.IsSuccess) return Result<string>.Failure("An unexpected error occurred while logging out the user!");

            return Result<string>.Success("User logged out successfully!");
        }
        public async Task<Result<string>> AuthenticateUserAsync(string email)
        {
            var isUserExist = await _userValidators.ValidateUserExists(email);
            if (!isUserExist.IsSuccess) return Result<string>.Failure(UserMessages.UserNotFoundByEmail);
            var user = isUserExist.Value!;

            var userDto = _mapper.Map<UserDTO>(user);
            var token = await _tokenService.GenerateAccessTokenAsync(userDto);
            return Result<string>.Success(token.Value!);
        }
        public async Task<Result<UserDTO>> UpdateUserAsync(UpdateUserDTO updateUserDTO, IFormFile? profilePicture)
        {
            var user = await _userValidators.ValidateUserExists(updateUserDTO.Id);
            if (!user.IsSuccess) return Result<UserDTO>.Failure(UserMessages.UserNotFoundById);
            var dbUser = user.Value!;

            var emailUpdateResult = await UpdateEmailIfNeeded(dbUser, updateUserDTO.Email);
            if (!emailUpdateResult.IsSuccess) return Result<UserDTO>.Failure(emailUpdateResult.Error!);

            UpdateFullNameIfNeeded(dbUser, updateUserDTO.FullName);

            await UpdateProfilePictureIfNeeded(dbUser, profilePicture);

            await _userRepository.UpdateAsync(dbUser);
            var userDto = _mapper.Map<UserDTO>(dbUser);
            return Result<UserDTO>.Success(userDto);

        }
        private void UpdateFullNameIfNeeded(User dbUser, string? newFullName)
        {
            var processedName = newFullName?.Trim();
            if (!string.IsNullOrWhiteSpace(processedName) && !string.Equals(processedName, dbUser.FullName, StringComparison.CurrentCultureIgnoreCase))
            {
                var textInfo = CultureInfo.CurrentCulture.TextInfo;
                dbUser.FullName = textInfo.ToTitleCase(processedName.ToLower());
            }
        }
        private async Task<Result<string>> UpdateEmailIfNeeded(User dbUser, string? newEmail)
        {
            if (!string.IsNullOrWhiteSpace(newEmail)
                && _userValidators.ValidateEmailFormat(newEmail).IsSuccess
                && !string.Equals(newEmail, dbUser.Email, StringComparison.OrdinalIgnoreCase))
            {
                var existingUser = await _userRepository.GetUserByEmailAsync(newEmail.ToLower());
                if (existingUser != null && existingUser.Id != dbUser.Id)
                    return Result<string>.Failure(UserMessages.EmailAlreadyInUse);
                dbUser.Email = newEmail.ToLower();
            }
            return Result<string>.Success("Email updated successfully.");
        }
        private async SystemTask UpdateProfilePictureIfNeeded(User dbUser, IFormFile? profilePicture)
        {
            if (profilePicture != null)
                dbUser.ProfilePictureUrl = await _cloudinaryHelper.UploadProfilePictureAsync(profilePicture);
        }

        private static string HashToken(string token)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(bytes);
        }

        private static string GenerateToken()
        {
            var bytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        public async Task<Result<string>> RequestPasswordResetAsync(string email)
        {
            var isUserExist = await _userValidators.ValidateUserExists(email);
            if (!isUserExist.IsSuccess) return Result<string>.Failure(UserMessages.UserNotFoundByEmail);

            var user = isUserExist.Value!;
            await _passwordResetTokenRepository.InvalidateTokensForUserAsync(user.Id);

            var token = GenerateToken();
            var tokenHash = HashToken(token);

            var resetToken = new PasswordResetToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                User = user,
                TokenHash = tokenHash,
                ExpiresAt = _passwordResetExpiry
            };

            await _passwordResetTokenRepository.CreateAsync(resetToken);

            var resetLink = $"{_baseUrl}/reset-password?token={Uri.EscapeDataString(token)}";

            var (htmlBody, textBody) = EmailTemplates.BuildResetPassword("ChoresHub", resetLink);
            await _emailSender.SendAsync(user.Email, _subject, htmlBody, textBody);

            return Result<string>.Success("Password reset token generated.");
        }

        public async Task<Result<string>> ConfirmPasswordResetAsync(string token, string newPassword)
        {
            var tokenHash = HashToken(token);
            var storedToken = await _passwordResetTokenRepository.FindActiveTokenByHashAsync(tokenHash);

            if (storedToken == null)
                return Result<string>.Failure("Invalid or expired reset token.");

            var user = await _userRepository.GetByIdAsync(storedToken.UserId);
            if (user == null) return Result<string>.Failure(UserMessages.UserNotFoundById);

            if (BCrypt.Net.BCrypt.Verify(newPassword, user.Password))
                return Result<string>.Failure("New password cannot be the same as the old password.");

            var isPasswordValid = _userValidators.ValidatePasswordFormat(newPassword);
            if (!isPasswordValid.IsSuccess)
                return Result<string>.Failure(UserMessages.PasswordPolicy);

            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _userRepository.UpdateAsync(user);

            storedToken.UsedAt = DateTime.UtcNow;
            await _passwordResetTokenRepository.UpdateAsync(storedToken);

            await _tokenService.RevokeAllRefreshTokensAsync(user.Email);

            return Result<string>.Success("Password reset successfully.");
        }

        public async Task<Result<string>> ChangePasswordAsync(ChangePasswordDTO changePasswordDTO, Guid userId)
        {
            var isUserExist = await _userValidators.ValidateUserExists(userId);
            if (!isUserExist.IsSuccess) return Result<string>.Failure(UserMessages.UserNotFoundById);
            var dbUser = isUserExist.Value!;

            if (!BCrypt.Net.BCrypt.Verify(changePasswordDTO.OldPassword, dbUser.Password))
                return Result<string>.Failure(UserMessages.IncorrectPassword);

            var isNewPasswordValid = _userValidators.ValidatePasswordFormat(changePasswordDTO.NewPassword);
            if (!isNewPasswordValid.IsSuccess)
                return Result<string>.Failure(UserMessages.PasswordPolicy);

            if (BCrypt.Net.BCrypt.Verify(changePasswordDTO.NewPassword, dbUser.Password))
                return Result<string>.Failure("New password cannot be the same as the old password.");

            var revokingTokens = await _tokenService.RevokeAllRefreshTokensAsync(dbUser.Email);
            if (!revokingTokens.IsSuccess)
                return Result<string>.Failure("An error occurred while revoking existing refresh tokens.");

            dbUser.Password = BCrypt.Net.BCrypt.HashPassword(changePasswordDTO.NewPassword);
            await _userRepository.UpdateAsync(dbUser);

            return Result<string>.Success("Password changed successfully!");
        }


    }

}