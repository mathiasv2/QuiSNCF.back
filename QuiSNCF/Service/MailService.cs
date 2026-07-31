using MailKit.Net.Smtp;
using MimeKit;

namespace QuiSNCF.Service;

public class MailService
{
    public async Task SendMail()
    {
        var msg = new MimeMessage();
        msg.From.Add(new MailboxAddress("App", "sncfdle.contact@gmail.com"));
        msg.To.Add(new MailboxAddress("User", "sncfdle.contact@gmail.com"));
        msg.Subject = "Hello from .NET!";
        msg.Body = new TextPart("plain") { Text = "This is a test email." };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync("localhost", 1025, false);
        await smtp.SendAsync(msg);
        await smtp.DisconnectAsync(true);
    }
}