using Microsoft.AspNetCore.Http;
using ChoresHub.Application.DTOs;
using ChoresHub.Application.Common;

namespace ChoresHub.Application.Interfaces
{
    public interface IUserService
    {
        Task<Result<UserDTO>> GetUserByIdAsync(Guid id);
        Task<Result<IList<UserDTO>>> GetAllUsersAsync();
        Task<Result<UserDTO>> GetUserByEmailAsync(string email);
        Task<Result<UserDTO>> RegisterAsync(RegisterUserDTO userDTO);
        Task<Result<string>> DeleteUserAsync(Guid id, string password);
        Task<Result<UserDTO>> UpdateUserAsync(UpdateUserDTO updateUserDTO, IFormFile? profilePicture);


        Task<Result<TokenPairDTO>> LoginAsync(LoginDTO loginDTO);
        Task<Result<string>> RequestPasswordResetAsync(string email);
        Task<Result<string>> LogoutAsync(RefreshTokenDTO refreshTokenDTO);
        Task<Result<string>> AuthenticateUserAsync(string email);
        Task<Result<string>> ConfirmPasswordResetAsync(string token, string newPassword);
        Task<Result<string>> ChangePasswordAsync(ChangePasswordDTO changePasswordDTO, Guid userId);

    }
}