using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using ProudSmileDent.Models;

namespace ProudSmileDent.Services
{
    public class EmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendReservationEmail(string fullName, string phone, string email, string service, DateTime date, string? message)
        {
            var mail = new MimeMessage();

            // From (göndərən)
            mail.From.Add(new MailboxAddress(fullName ?? "Müştəri", _settings.From));

            // To (klinika maili)
            mail.To.Add(new MailboxAddress("Clinic", _settings.To));

            // Reply-To (pasiyentə cavab vermək üçün)
            if (!string.IsNullOrWhiteSpace(email))
            {
                mail.ReplyTo.Add(new MailboxAddress(fullName, email));
            }

            mail.Subject = $"Yeni rezervasiya – {fullName}";

            mail.Body = new TextPart("plain")
            {
                Text =
$@"Yeni rezervasiya gəldi:

Ad: {fullName}
Telefon: {phone}
Email: {email}
Xidmət: {service}
Tarix: {date}
Mesaj: {message}"
            };

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(_settings.Host, _settings.Port, MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_settings.Username, _settings.Password);
            await smtp.SendAsync(mail);
            await smtp.DisconnectAsync(true);
        }
        public async Task SendReminderEmail(string fullName, string email, string service, DateTime date)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return;
            }

            var mail = new MimeMessage();

            mail.From.Add(new MailboxAddress("ProudSmileDent", _settings.From));
            mail.To.Add(new MailboxAddress(fullName ?? "İstifadəçi", email));

            mail.Subject = "Qəbul vaxtınıza 1 saat qalıb";

            mail.Body = new TextPart("plain")
            {
                Text =
        $@"Salam {fullName},

Sizin stomatoloji qəbul vaxtınıza 1 saat qalıb.

Xidmət: {service}
Tarix və saat: {date}

Zəhmət olmasa qəbul vaxtına yaxın klinikaya yaxınlaşın.

Hörmətlə,
ProudSmileDent"
            };

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(_settings.Host, _settings.Port, MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_settings.Username, _settings.Password);
            await smtp.SendAsync(mail);
            await smtp.DisconnectAsync(true);
        }
    }
}