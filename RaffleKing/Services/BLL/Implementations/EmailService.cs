using MimeKit;
using MimeKit.Text;
using RaffleKing.Services.BLL.Interfaces;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace RaffleKing.Services.BLL.Implementations;

public class EmailService(string smtpServer, int smtpPort, string fromAddress, string smtpUsername, string smtpPassword)
    : IEmailService
{
    public async Task SendEmail(string to, string subject, string body)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress("Raffle King", fromAddress));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;
        email.Body = new TextPart(TextFormat.Plain) { Text = body };

        using var client = new SmtpClient();
        await client.ConnectAsync(smtpServer, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(smtpUsername, smtpPassword);
        await client.SendAsync(email);
        await client.DisconnectAsync(true);
    }
}