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

            mail.From.Add(new MailboxAddress(fullName ?? "Müştəri", _settings.From));
            mail.To.Add(new MailboxAddress("Clinic", _settings.To));

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

            await SendAsync(mail);
        }

<<<<<<< HEAD
=======

>>>>>>> 2386656b82fc8776b7038d584c6b493cf03b5b3d
        public async Task SendReservationUpdatedEmail(string fullName, string phone, string email, string service, DateTime date, string? message)
        {
            var mail = new MimeMessage();

            mail.From.Add(new MailboxAddress("ProudSmileDent", _settings.From));
            mail.To.Add(new MailboxAddress("Clinic", _settings.To));

            if (!string.IsNullOrWhiteSpace(email))
            {
                mail.ReplyTo.Add(new MailboxAddress(fullName ?? "Müştəri", email));
            }

            mail.Subject = $"Rezervasiya düzəliş edildi – {fullName}";

            mail.Body = new TextPart("plain")
            {
                Text =
$@"İstifadəçi rezervasiyasında düzəliş etdi:

Ad: {fullName}
Telefon: {phone}
Email: {email}
Xidmət: {service}
Yeni tarix: {date}
Mesaj: {message}

"
            };

            await SendAsync(mail);
        }

<<<<<<< HEAD
=======

>>>>>>> 2386656b82fc8776b7038d584c6b493cf03b5b3d
        public async Task SendReminderEmail(string fullName, string email, string service, DateTime date)
        {
            if (string.IsNullOrWhiteSpace(email))
                return;

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

            await SendAsync(mail);
        }

        public async Task SendResetCodeEmail(string email, string code)
        {
            var mail = new MimeMessage();

            mail.From.Add(new MailboxAddress("ProudSmileDent", _settings.From));
            mail.To.Add(new MailboxAddress("User", email));

            mail.Subject = "Şifrə sıfırlama kodu";

            mail.Body = new TextPart("plain")
            {
                Text =
$@"Salam,

Sizin şifrə sıfırlama kodunuz:

KOD: {code}

Bu kod 10 dəqiqə ərzində keçərlidir.

Əgər bu sorğunu siz etməmisinizsə, bu mesajı nəzərə almayın.

Hörmətlə,
ProudSmileDent"
            };

            await SendAsync(mail);
        }

        public async Task SendApprovedEmail(string fullName, string email, string service, DateTime date)
        {
            if (string.IsNullOrWhiteSpace(email))
                return;

            var mail = new MimeMessage();

            mail.From.Add(new MailboxAddress("ProudSmileDent", _settings.From));
            mail.To.Add(new MailboxAddress(fullName ?? "İstifadəçi", email));

            mail.Subject = "Rezervasiyanız təsdiqləndi";

            mail.Body = new TextPart("plain")
            {
                Text =
$@"Salam {fullName},

Sizin rezervasiyanız təsdiqləndi.

Xidmət: {service}
Qəbul tarixi və saatı: {date}

Rezervasiyanız artıq aktiv rezervasiyalar siyahısındadır.

Hörmətlə,
ProudSmileDent"
            };

            await SendAsync(mail);
        }

        public async Task SendRejectedEmail(string fullName, string email, string service, DateTime date, string? adminMessage)
        {
            if (string.IsNullOrWhiteSpace(email))
                return;

            var finalMessage = string.IsNullOrWhiteSpace(adminMessage)
                ? "Təəssüf ki, seçdiyiniz vaxt üçün rezervasiya təsdiqlənmədi."
                : adminMessage;

            var mail = new MimeMessage();

            mail.From.Add(new MailboxAddress("ProudSmileDent", _settings.From));
            mail.To.Add(new MailboxAddress(fullName ?? "İstifadəçi", email));

            mail.Subject = "Rezervasiyanız rədd edildi";

            mail.Body = new TextPart("plain")
            {
                Text =
$@"Salam {fullName},

Sizin rezervasiyanız rədd edildi.

Xidmət: {service}
Qəbul tarixi və saatı: {date}

Səbəb / qeyd:
{finalMessage}

İstəsəniz, uyğun başqa vaxt üçün yenidən rezervasiya yarada bilərsiniz.

Hörmətlə,
ProudSmileDent"
            };

            await SendAsync(mail);
        }

        private async Task SendAsync(MimeMessage mail)
        {
            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(_settings.Host, _settings.Port, MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_settings.Username, _settings.Password);
            await smtp.SendAsync(mail);
            await smtp.DisconnectAsync(true);
        }
    }
}