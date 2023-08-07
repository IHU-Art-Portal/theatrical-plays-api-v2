using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace Theatrical.Services.Email;

public interface IEmailService
{
    Task SendConfirmationEmailAsync(string email, string confirmationToken);
}

public class EmailService : IEmailService
{
    private readonly string _userEmail = "theatricalportalv2@gmail.com";
    private readonly string _emailPassword = "lyhommmmlgekezed";
    
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
}