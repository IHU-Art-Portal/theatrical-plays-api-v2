using Microsoft.AspNetCore.Mvc;
using Theatrical.Dto.ResponseWrapperFolder;

namespace Theatrical.Dto.LoginDtos;

public class UserErrorMessage
{
    private readonly string validationMessage;

    public UserErrorMessage(string validationMessage)
    {
        this.validationMessage = validationMessage;
    }
    
    public ActionResult<TheatricalResponse> ConstructActionResult()
    {
        if (validationMessage.Equals("Invalid token"))
        {
            var errorResponse1 = new TheatricalResponse(ErrorCode.InvalidToken, validationMessage);
            return new ObjectResult(errorResponse1){StatusCode = 498};
        }
        else if (validationMessage.Contains("forbidden"))
        {
            var errorResponse1 = new TheatricalResponse(ErrorCode.Forbidden, validationMessage);
            return new ObjectResult(errorResponse1){StatusCode = 403};
        }
        else
        {
            var errorResponse = new TheatricalResponse(ErrorCode.Unauthorized, validationMessage);
            return new ObjectResult(errorResponse){StatusCode = 401};
        }
    }
}