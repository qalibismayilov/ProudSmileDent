namespace ProudSmileDent.Services;

public interface IEmailService
{
    Task SendAppointmentEmailAsync(string fullName, string phone, string email, string service, DateTime date, string message);
}