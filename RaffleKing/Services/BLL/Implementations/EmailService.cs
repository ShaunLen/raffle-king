using MimeKit;
using RaffleKing.Services.BLL.Interfaces;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace RaffleKing.Services.BLL.Implementations;

public class EmailService(string smtpServer, int smtpPort, string fromAddress, 
    string smtpUsername, string smtpPassword, string emailTemplatePath) : IEmailService
{
    public async Task SendEmail(string recipient, string subject, string htmlBody)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress("Raffle King", fromAddress));
        email.To.Add(MailboxAddress.Parse(recipient));
        email.Subject = subject;

        var builder = new BodyBuilder
        {
            HtmlBody = htmlBody
        };
        email.Body = builder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(smtpServer, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(smtpUsername, smtpPassword);
        await client.SendAsync(email);
        await client.DisconnectAsync(true);
    }
    
    public void SendGuestEntranceEmail(string recipient, string guestRef)
    {
        Task.Run(async () =>
        {
            var templatePath = Path.Combine(emailTemplatePath, "GuestEntranceEmail.html");
            var emailBody = await File.ReadAllTextAsync(templatePath);
            emailBody = emailBody.Replace("{guestRef}", guestRef);

            await SendEmail(
                recipient,
                "You've entered a draw",
                emailBody
            );
        });
    }

    public void SendUserEntranceEmail(string userId)
    {
        throw new NotImplementedException();
    }
    
    public void SendGuestWinnerEmail(string recipient)
    {
        Task.Run(async () =>
        {
            var templatePath = Path.Combine(emailTemplatePath, "GuestWinnerEmail.html");
            var emailBody = await File.ReadAllTextAsync(templatePath);

            await SendEmail(
                recipient,
                "You're a winner!",
                emailBody
            );
        });
    }

    public void SendUserWinnerEmail(string recipient)
    {
        Task.Run(async () =>
        {
            var templatePath = Path.Combine(emailTemplatePath, "UserWinnerEmail.html");
            var emailBody = await File.ReadAllTextAsync(templatePath);

            await SendEmail(
                recipient,
                "You're a winner!",
                emailBody
            );
        });
    }
}