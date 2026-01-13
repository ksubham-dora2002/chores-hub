using System.Text.RegularExpressions;
using ChoresHub.Application.Common;
using ChoresHub.Application.Helpers;
using ChoresHub.Domain.Entities;
using ChoresHub.Domain.Interfaces;

namespace ChoresHub.Application.Validators
{
    public class UserValidators(IUserRepository userRepository) : IUserValidators
    {
        private readonly IUserRepository _userRepository = userRepository;

        public Result<string> ValidateEmailFormat(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return Result<string>.Failure(UserMessages.EmailCannotBeEmpty);
            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return Result<string>.Failure(UserMessages.InvalidEmail);
            return Result<string>.Success(UserMessages.ValidEmail);

        }
     
        public Result<string> ValidatePasswordFormat(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8 || password.Length > 8)
                return Result<string>.Failure(UserMessages.PasswordPolicy);

            if (!Regex.IsMatch(password, @"[A-Z]") ||
                !Regex.IsMatch(password, @"[a-z]") ||
                !Regex.IsMatch(password, @"[0-9]") ||
                !Regex.IsMatch(password, @"[\W_]"))
            {
                return Result<string>.Failure(UserMessages.PasswordPolicy);
            }

            return Result<string>.Success(UserMessages.ValidPassword);
        }
        public async Task<Result<User>> ValidateUserExists(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user != null)
                return Result<User>.Success(user);
            return Result<User>.Failure(UserMessages.UserNotFoundById);
        }

        public async Task<Result<User>> ValidateUserExists(string email)
        {
            var user = await _userRepository.GetUserByEmailAsync(email.ToLower());
            if (user != null)
                return Result<User>.Success(user);
            return Result<User>.Failure(UserMessages.UserNotFoundByEmail);
        }

    }
}