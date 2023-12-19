namespace Theatrical.Data.enums;

public enum ErrorCode
{
    NotFound,
    InvalidPayload,
    InvalidToken,
    AlreadyExists,
    Unauthorized,
    Forbidden,
    BadRequest,
    ServerError,
    CurationFailure,
    InvalidEmail,
    AlreadyVerified,
    _2FaEnabled,
    _2FaFailure,
    _2FaDisabled,
    AlreadyLoggedIn,
    NoAvailablePhotos,
    InvalidCharacters,
    RoleNotFound,
    UserAlreadyClaimedVenue,
    UserAlreadyClaimedEvent,
    TwilioError,
    InsufficientBalance,
    WrongDateFormat
}