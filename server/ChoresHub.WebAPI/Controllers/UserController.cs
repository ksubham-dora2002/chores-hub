using System.Security.Claims;
using ChoresHub.WebAPI.Helpers;
using Microsoft.AspNetCore.Mvc;
using ChoresHub.Application.DTOs;
using ChoresHub.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;


namespace ChoresHub.WebAPI.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class UserController(IUserService userService, ITokenService tokenService) : ControllerBase
    {
        private readonly IUserService _userService = userService;
        private readonly ITokenService _tokenService = tokenService;
        private readonly TimeSpan _expirationTimeLong = TimeSpan.FromDays(7);

        [HttpGet()]
        [Authorize]
        public async Task<ActionResult<IList<UserDTO>>> GetAllUsers()
        {
            var result = await _userService.GetAllUsersAsync();
            if (!result.IsSuccess) return NotFound(result.Error);
            return Ok(result.Value);
        }
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<UserDTO>> GetUser([FromRoute] Guid id)
        {
            if (id == Guid.Empty) return BadRequest("Invalid request");

            var result = await _userService.GetUserByIdAsync(id);
            if (!result.IsSuccess || result.Value == null)
                return NotFound(result.Error ?? "User not found");

            return Ok(result.Value);
        }
        [HttpDelete()]
        [Authorize]
        public async Task<ActionResult<string>> DeleteUser([FromBody] DeleteUserDTO dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized("Invalid user id");

            if (string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Invalid delete request");

            var result = await _userService.DeleteUserAsync(userId, dto.Password);

            CookieHelper.DeleteCookie(Response, "atk");
            CookieHelper.DeleteCookie(Response, "rtk");

            if (!result.IsSuccess)
                return StatusCode(500, result.Error ?? "Delete failed");
            return Ok(result.Value);
        }
        [HttpPut("update")]
        [Authorize]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateUser([FromForm] UpdateUserDTO updateUserDTO, [FromForm] IFormFile? profilePicture)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized("Invalid user id");

            updateUserDTO.Id = userId; 

            if (updateUserDTO == null) return BadRequest("Invalid update request");

            var result = await _userService.UpdateUserAsync(updateUserDTO, profilePicture);

            if (!result.IsSuccess || result.Value == null)
                return StatusCode(500, result.Error ?? "Update failed");

            return Ok(result.Value);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            if (loginDTO == null || string.IsNullOrWhiteSpace(loginDTO.Email) || string.IsNullOrWhiteSpace(loginDTO.Password))
                return BadRequest("Invalid login request");

            var tokenPair = await _userService.LoginAsync(loginDTO);

            if (tokenPair.IsSuccess == false || tokenPair.Value == null)
                return Unauthorized(tokenPair.Error ?? "Invalid credentials");

            CookieHelper.SetSessionCookie(Response, "atk", tokenPair.Value.AccessToken, isHttpOnly: true);
            CookieHelper.SetCookie(Response, "rtk", tokenPair.Value.RefreshTokenString, _expirationTimeLong, isHttpOnly: true);
            var user = await _userService.GetUserByEmailAsync(loginDTO.Email);

            if (user == null || !user.IsSuccess)
                return Unauthorized(user?.Error ?? "User not found");
            return Ok(user.Value!.Id);

        }
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var refreshToken = Request.Cookies["rtk"];

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(refreshToken))
            {
                CookieHelper.DeleteCookie(Response, "atk");
                CookieHelper.DeleteCookie(Response, "rtk");
                return Ok();
            }

            var result = await _userService.LogoutAsync(new RefreshTokenDTO
            {
                Email = email,
                RefreshTokenString = refreshToken
            });

            CookieHelper.DeleteCookie(Response, "atk");
            CookieHelper.DeleteCookie(Response, "rtk");

            if (!result.IsSuccess)
                return StatusCode(500, result.Error ?? "Logout failed");

            return Ok(result.Value);
        }

        [HttpPost("request-reset-password")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] RequestPasswordResetDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest("Invalid request");

            var result = await _userService.RequestPasswordResetAsync(dto.Email);
            if (!result.IsSuccess)
                return StatusCode(500, result.Error ?? "Reset request failed");

            return Ok(result.Value);
        }

        [HttpPost("confirm-reset-password")]
        public async Task<IActionResult> ConfirmPasswordReset([FromBody] ConfirmPasswordResetDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Token) || string.IsNullOrWhiteSpace(dto.NewPassword))
                return BadRequest("Invalid request");

            var result = await _userService.ConfirmPasswordResetAsync(dto.Token, dto.NewPassword);
            if (!result.IsSuccess)
                return StatusCode(500, result.Error ?? "Password reset failed");

            return Ok(result.Value);
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.OldPassword) || string.IsNullOrWhiteSpace(dto.NewPassword))
                return BadRequest("Invalid change password request");

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized("Invalid user id");

            var result = await _userService.ChangePasswordAsync(dto, userId);

            if (!result.IsSuccess)
                return StatusCode(500, result.Error ?? "Change password failed");

            return Ok(result.Value);
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register([FromBody] RegisterUserDTO userDTO)
        {
            if (userDTO == null) return BadRequest("There is no user!");

            var result = await _userService.RegisterAsync(userDTO);
            if (!result.IsSuccess || result.Value == null)
            {
                return StatusCode(500, result.Error);
            }

            var newUser = await _userService.GetUserByEmailAsync(userDTO.Email);

            if (newUser == null)
                return StatusCode(500, result.Error);

            var accessToken = await _tokenService.GenerateAccessTokenAsync(newUser.Value!);
            var refreshToken = await _tokenService.GenerateRefreshTokenAsync(newUser.Value!);

            CookieHelper.SetSessionCookie(Response, "atk", accessToken.Value!, isHttpOnly: true);
            CookieHelper.SetCookie(Response, "rtk", refreshToken.Value!.Token, _expirationTimeLong);
            
            var user = await _userService.GetUserByEmailAsync(userDTO.Email);
            return Ok(user.Value);
        }
        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken()
        {

            var refreshToken = Request.Cookies["rtk"];

            if (string.IsNullOrWhiteSpace(refreshToken))
                return BadRequest("Refresh token is missing or invalid");

            var result = await _tokenService.RefreshAsync(refreshToken);
            if (!result.IsSuccess)
                return Unauthorized(result.Error ?? "Refresh failed");

            CookieHelper.SetSessionCookie(Response, "atk", result.Value!.AccessToken, isHttpOnly: true);
            CookieHelper.SetCookie(Response, "rtk", result.Value.RefreshTokenString, _expirationTimeLong);

            return Ok(result.Value.UserId);
        }
    }
}