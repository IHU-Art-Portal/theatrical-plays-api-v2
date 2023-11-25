using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Theatrical.Api.Pages;

public class EmailVerification : PageModel
{
    public string VerificationStatus { get; set; }
    public string VerificationMessage { get; set; }
    
    public void OnGet(string status)
    {
        if (status == "success")
        {
            VerificationStatus = status;
            VerificationMessage = "Email verified successfully!";
        }
        else if (status == "already-verified")
        {
            VerificationStatus = status;
            VerificationMessage = "Email has already been verified.";
        }
        else if (status == "failed")
        {
            VerificationStatus = status;
            VerificationMessage = "Email verification failed. Please try again.";
        }
        else if (status == "internal-error")
        {
            VerificationStatus = status;
            VerificationMessage = "Internal error occured. Service might be down.";
        }
    }
}