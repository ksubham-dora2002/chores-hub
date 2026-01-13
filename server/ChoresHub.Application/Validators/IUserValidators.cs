using ChoresHub.Application.Common;
using ChoresHub.Domain.Entities;

namespace ChoresHub.Application.Validators
{
    public interface IUserValidators
    {
        Result<string> ValidateEmailFormat(string email);
        Task<Result<User>> ValidateUserExists(Guid userId);
        Task<Result<User>> ValidateUserExists(string email);
        Result<string> ValidatePasswordFormat(string password);
    }
}