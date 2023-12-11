using Microsoft.Extensions.Configuration;
using Theatrical.Dto.ResponseWrapperFolder;
using Theatrical.Services.Validation;
using Twilio;
using Twilio.Rest.Verify.V2.Service;

namespace Theatrical.Services.PhoneVerification.Twilio;

public interface ITwilioService
{
    Task<ValidationReport> SendVerificationCode(string phoneNumber);
    Task<ValidationReport> CheckVerificationCode(string phoneNumber, string verificationCode);
}

public class TwilioService : ITwilioService
{
    private readonly string? _twilioServiceSid;

    public TwilioService(IConfiguration configuration)
    {
        var twilioAccountSid = configuration["Twilio:AccountSid"];
        var twilioAuthToken = configuration["Twilio:AuthToken"];
        _twilioServiceSid = configuration["Twilio:ServiceSid"];
        
        TwilioClient.Init(twilioAccountSid, twilioAuthToken);
    }
    
    public async Task<ValidationReport> SendVerificationCode(string phoneNumber)
    {
        try
        {
            var verification = await VerificationResource.CreateAsync(
                to: phoneNumber,
                channel: "sms",
                pathServiceSid: _twilioServiceSid
            );

            if (verification.Status == "pending")
            {
                return new ValidationReport
                {
                    Success = true,
                    Message = "Pending"
                };
            }

            var report = new ValidationReport
            {
                Success = false,
                Message = $"There was an error sending the verification code: {verification.Status}",
                ErrorCode = ErrorCode.TwilioError
            };

            return report;
        }
        catch (Exception)
        {
            Console.WriteLine("There was an error sending the verification code, please check the phone number is correct and try again");
            return new ValidationReport
            {
                Message = "There was an error sending the verification code, please check the phone number is correct and try again.",
                Success = false,
                ErrorCode = ErrorCode.TwilioError
            };
        }
    }
    
    public async Task<ValidationReport> CheckVerificationCode(string phoneNumber, string verificationCode)
    {
        try
        {
            var verificationCheck = await VerificationCheckResource.CreateAsync(
                to: phoneNumber,
                code: verificationCode,
                pathServiceSid: _twilioServiceSid
            );

            if (verificationCheck.Status == "approved")
            {
                return new ValidationReport
                {
                    Success = true,
                    Message = "Approved!"
                };
            }

            var report = new ValidationReport
            {
                Success = false,
                Message = $"There was an error verifying the code: {verificationCheck.Status}",
                ErrorCode = ErrorCode.TwilioError
            };
            
            return report;
        }
        catch (Exception)
        {
            var report = new ValidationReport
            {
                Success = false,
                Message = "There was an error verifying the code, please check the code is correct and try again",
                ErrorCode = ErrorCode.TwilioError
            };
            return report;
        }
    }
}