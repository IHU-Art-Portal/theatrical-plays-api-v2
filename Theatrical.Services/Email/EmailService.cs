using System.Net;
using System.Net.Mail;
using MailKit.Security;
using MimeKit;
using Theatrical.Data.Models;

namespace Theatrical.Services.Email;

public interface IEmailService
{
    Task SendConfirmationEmailAsync(string email, string url);
    Task Send2FaVerificationCode(User user, string totpCode);
    Task SendConfirmationEmailTwoFactorActivated(string email);
    Task SendConfirmationEmailTwoFactorDeactivated(string email);
    Task SendTemporaryPassword(string email, string temporaryPassword);
    Task SendApprovalEmail(string email, string personFullname);
    Task SendDisApprovalEmail(string email, string personFullname);
}

public class EmailService : IEmailService
{
    private readonly string _userEmail = "theatricalportalv2@gmail.com";
    private readonly string _emailPassword = "lyhommmmlgekezed";
    
    /// <summary>
    /// Sends a verification code to the registering user.
    /// </summary>
    /// <param name="email"></param>
    /// <param name="url"></param>
    public async Task SendConfirmationEmailAsync(string email, string url)
    {
        var message = new MailMessage();
        message.From = new MailAddress(_userEmail);
        message.To.Add(email);
        message.Subject = "Account Confirmation";

        message.Body = $"Please confirm your email address by clicking the following link: {url}";

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
        message.Subject = "Account 2FA Deactivated";
        
        message.Body = $"You have deactivated two factor authentication.";

        using (var client = new SmtpClient("smtp.gmail.com", 587))
        {
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential(_userEmail, _emailPassword);

            await client.SendMailAsync(message);
        }
    }
    
    public async Task SendTemporaryPassword(string email, string temporaryPassword)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("", _userEmail));
        message.To.Add(new MailboxAddress("", email));
        message.Subject = "Your Temporary Password";
        message.Body = new TextPart("html")
        {
            Text =
                $"<p><font color=\"black\">Use this temporary password to log in and change your password:</font></p>" +
                $"<p><font color=\"red\">{temporaryPassword}</font></p>"
        };
        
        using var client = new MailKit.Net.Smtp.SmtpClient();
        await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_userEmail, _emailPassword);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }

    public async Task SendApprovalEmail(string email, string personFullname)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("", _userEmail));
        message.To.Add(new MailboxAddress("", email));
        message.Subject = "Your Account Request has been approved!";
        message.Body = new TextPart("html")
        {
            Text =
                $"<p><font color=\"black\">Your Account request for {personFullname} has been <font color=\"green\">approved!</font></font></p>"
        };
        
        using var client = new MailKit.Net.Smtp.SmtpClient();
        await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_userEmail, _emailPassword);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }

    public async Task SendDisApprovalEmail(string email, string personFullname)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("", _userEmail));
        message.To.Add(new MailboxAddress("", email));
        message.Subject = "Your Account Request has been rejected.";
        message.Body = new TextPart("html")
        {
            Text =
                $"<p><font color=\"black\">Your Account request for {personFullname} has been <font color=\"red\">rejected!</font></font></p>"
        };
        
        using var client = new MailKit.Net.Smtp.SmtpClient();
        await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_userEmail, _emailPassword);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
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