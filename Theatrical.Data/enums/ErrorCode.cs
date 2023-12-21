namespace Theatrical.Data.enums;

public enum ErrorCode
{
    NotFound,
    InvalidToken,
    AlreadyExists,
    Unauthorized,
    Forbidden,
    BadRequest,
    ServerError,
    InvalidEmail,
    AlreadyVerified,
    _2FaEnabled,
    _2FaFailure,
    _2FaDisabled,
    AlreadyLoggedIn,
    InvalidCharacters,
    UserAlreadyClaimedVenue,
    UserAlreadyClaimedEvent,
    TwilioError,
    InsufficientBalance,
    WrongDateFormat
}