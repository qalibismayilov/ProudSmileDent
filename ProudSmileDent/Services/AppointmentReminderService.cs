using Microsoft.EntityFrameworkCore;
using ProudSmileDent.Data;

namespace ProudSmileDent.Services
{
    public class AppointmentReminderService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public AppointmentReminderService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();

                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();

                var now = DateTime.Now;
                var oneHourLater = now.AddHours(1);

                
                var reminderAppointments = await context.Appointments
                    .Where(x =>
                        x.Date >= now &&
                        x.Date <= oneHourLater &&
                        x.Status == "Approved" &&
                        !x.ReminderSent)
                    .ToListAsync(stoppingToken);

                foreach (var appointment in reminderAppointments)
                {
                    try
                    {
                        await emailService.SendReminderEmail(
                            appointment.FullName,
                            appointment.Email,
                            appointment.Service,
                            appointment.Date
                        );

                        appointment.ReminderSent = true;
                    }
                    catch
                    {
                    }
                }

              
                var expiredAppointments = await context.Appointments
                    .Where(x => x.Date < now && x.IsActive)
                    .ToListAsync(stoppingToken);

                foreach (var appointment in expiredAppointments)
                {
                    appointment.IsActive = false;

                    if (appointment.Status == "Pending" || appointment.Status == "Approved")
                    {
                        appointment.Status = "Deactivated";
                    }
                }

                if (reminderAppointments.Any() || expiredAppointments.Any())
                {
                    await context.SaveChangesAsync(stoppingToken);
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}