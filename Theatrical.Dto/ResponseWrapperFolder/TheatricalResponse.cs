namespace Theatrical.Dto.ResponseWrapperFolder;

public enum ErrorCode
{
    NotFound,
    InvalidPayload,
    InvalidToken,
    AlreadyExists,
    Unauthorized,
    Forbidden
}

public class TheatricalResponse
{
    public bool Success { get; }
    public string Message { get; }
    public string?  ErrorCode { get; }

    public TheatricalResponse(ErrorCode code, string message)
    {
        ErrorCode = code.ToString();
        Success = false;
        Message = message;
    }

    public TheatricalResponse(string message = "Successful")
    {
        Success = true;
        Message = message;
    }
}

public class TheatricalResponse<T> : TheatricalResponse
{
    public T? Data { get; }

    public TheatricalResponse(T? data, string message = "Completed") : base(message)
    {
        Data = data;
    }

    public TheatricalResponse(T? data, ErrorCode code, string message) : base(code, message)
    {
        Data = data;
    }
}