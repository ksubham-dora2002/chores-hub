namespace ChoresHub.Application.Helpers
{
    public static class UserMessages
    {
        public const string PasswordPolicy = "Password must be exactly 8 characters long and contain at least one uppercase letter, one lowercase letter, one number, and one special character.";
        public const string ValidPassword = "Password is valid";
        public const string IncorrectPassword = "Incorrect password!";
        public const string UserNotFoundById = "User not found with given id!";
        public const string UserNotFoundByEmail = "User not found with given email!";
        public const string EmailAlreadyInUse = "Email already in use by another user";
        public const string EmailCannotBeEmpty = "Email cannot be empty";
        public const string ValidEmail = "Email is valid and exists";
        public const string InvalidEmail = "Email format is invalid";

    }
}