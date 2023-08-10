using System.Text.Json;

namespace Theatrical.Dto.ResponseWrapperFolder;

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
    AlreadyVerified
}

public class ApiResponse
{
    public bool Success { get; }
    public string Message { get; }
    public string? ErrorCode { get; }

    public ApiResponse(ErrorCode code, string message)
    {
        ErrorCode = code.ToString();
        Success = false;
        Message = message;
    }

    public ApiResponse(string message = "Successful")
    {
        Success = true;
        Message = message;
    }
    
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}

public class ApiResponse<T> : ApiResponse
{
    public T? Data { get; }

    public ApiResponse(T? data, string message = "Completed") : base(message)
    {
        Data = data;
    }

    public ApiResponse(T? data, ErrorCode code, string message) : base(code, message)
    {
        Data = data;
    }
}