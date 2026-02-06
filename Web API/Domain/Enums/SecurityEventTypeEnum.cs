namespace Web_API.Domain.Enums;

public enum SecurityEventTypeEnum
{
    LoginSuccess = 1,
    LoginFailed = 2,
    Logout = 3,
    RefreshTokenCreated = 4,
    RefreshTokenRevoked = 5,
    PasswordChanged = 6,
    AccountLocked = 7,
    DeviceMismatch = 8
}
