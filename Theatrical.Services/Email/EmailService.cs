using System.Net;
using System.Net.Mail;
using OtpNet;
using Theatrical.Data.Models;

namespace Theatrical.Services.Email;

public interface IEmailService
{
    Task SendConfirmationEmailAsync(string email, string confirmationToken);
    Task Send2FaVerificationCode(User user, string totpCode);
    Task SendConfirmationEmailTwoFactorActivated(string email);
    Task SendConfirmationEmailTwoFactorDeactivated(string email);
}

public class EmailService : IEmailService
{
    private readonly string _userEmail = "theatricalportalv2@gmail.com";
    private readonly string _emailPassword = "lyhommmmlgekezed";
    
    /// <summary>
    /// Sends a verification code to the registering user.
    /// </summary>
    /// <param name="email"></param>
    /// <param name="confirmationToken"></param>
    public async Task SendConfirmationEmailAsync(string email, string confirmationToken)
    {
        var message = new MailMessage();
        message.From = new MailAddress(_userEmail);
        message.To.Add(email);
        message.Subject = "Account Confirmation";

        
        var confirmationLink = $"https://localhost:7042/api/user/verify?token={confirmationToken}";
        message.Body = $"Please confirm your email address by clicking the following link: {confirmationLink}";

        using (var client = new SmtpClient("smtp.gmail.com", 587))
        {
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential(_userEmail, _emailPassword);

            await client.SendMailAsync(message);
        }
    }
    
    public async Task SendConfirmationEmailTwoFactorActivated(string email)
    {
        var message = new MailMessage();
        message.From = new MailAddress(_userEmail);
        message.To.Add(email);
        message.Subject = "Account 2FA Activated";
        
        message.Body = $"You have successfully activated two factor authentication." +
                       $"\nFrom now on you will be required to enter the 2fa code when attempting to log in.";

        using (var client = new SmtpClient("smtp.gmail.com", 587))
        {
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential(_userEmail, _emailPassword);

            await client.SendMailAsync(message);
        }
    }

    public async Task SendConfirmationEmailTwoFactorDeactivated(string email)
    {
        var message = new MailMessage();
        message.From = new MailAddress(_userEmail);
        message.To.Add(email);
        message.Subject = "Account 2FA Activated";
        
        message.Body = $"You have deactivated two factor authentication.";

        using (var client = new SmtpClient("smtp.gmail.com", 587))
        {
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential(_userEmail, _emailPassword);

            await client.SendMailAsync(message);
        }
    }

    /// <summary>
    /// Sends the code to the user's email.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="totpCode"></param>
    public async Task Send2FaVerificationCode(User user, string totpCode)
    {
        var message = new MailMessage();
        message.From = new MailAddress(_userEmail);
        message.To.Add(user.Email);
        message.Subject = "Log in with 2FA code.";
        
        message.Body = $"Your 2FA password is: {totpCode}" +
                       $"\nThe code expires in 30 seconds.";

        using var client = new SmtpClient("smtp.gmail.com", 587);
        client.UseDefaultCredentials = false;
        client.EnableSsl = true;
        client.Credentials = new NetworkCredential(_userEmail, _emailPassword);

        await client.SendMailAsync(message);
    }
}