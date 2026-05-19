using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProudSmileDent.Data;
using ProudSmileDent.DTOs;
using ProudSmileDent.Models;
using ProudSmileDent.Services;

namespace ProudSmileDent.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;

        public AppointmentsController(AppDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        private IActionResult? ValidateAppointmentDate(DateTime date)
        {
            if (date < DateTime.Now.AddHours(1))
<<<<<<< HEAD
            {
                return BadRequest(new { message = "Rezervasiya ən azı 1 saat əvvəl yaradılmalıdır." });
            }

            if (date.DayOfWeek == DayOfWeek.Sunday)
            {
                return BadRequest(new { message = "Bazar günü rezervasiya mümkün deyil." });
            }
=======
                return BadRequest(new { message = "Rezervasiya ən azı 1 saat əvvəl yaradılmalıdır." });

            if (date.DayOfWeek == DayOfWeek.Sunday)
                return BadRequest(new { message = "Bazar günü rezervasiya mümkün deyil." });
>>>>>>> 2386656b82fc8776b7038d584c6b493cf03b5b3d

            var time = date.TimeOfDay;
            var start = new TimeSpan(9, 0, 0);
            var end = new TimeSpan(20, 0, 0);

            if (time < start || time > end)
<<<<<<< HEAD
            {
                return BadRequest(new { message = "Rezervasiya saatı yalnız 09:00 - 20:00 aralığında ola bilər." });
            }
=======
                return BadRequest(new { message = "Rezervasiya saatı yalnız 09:00 - 20:00 aralığında ola bilər." });
>>>>>>> 2386656b82fc8776b7038d584c6b493cf03b5b3d

            return null;
        }

<<<<<<< HEAD
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AppointmentCreateDto dto)
        {
            if (dto.UserId == null || dto.UserId <= 0)
=======
        private object MapAppointment(Appointment appointment)
        {
            return new
>>>>>>> 2386656b82fc8776b7038d584c6b493cf03b5b3d
            {
                id = appointment.Id,
                fullName = appointment.FullName,
                phone = appointment.Phone,
                email = appointment.Email,
                service = appointment.Service,
                date = appointment.Date,
                message = appointment.Message,
                status = appointment.Status,
                userId = appointment.UserId,
                reminderSent = appointment.ReminderSent,
                isActive = appointment.IsActive,
                createdAt = appointment.CreatedAt,
                adminResponseMessage = appointment.AdminResponseMessage,
                hasUserEdited = appointment.HasUserEdited
            };
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AppointmentCreateDto dto)
        {
            if (dto.UserId == null || dto.UserId <= 0)
                return Unauthorized(new { message = "Rezervasiya üçün əvvəlcə daxil olun." });

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == dto.UserId.Value);

            if (user == null)
                return Unauthorized(new { message = "İstifadəçi tapılmadı." });

            var dateValidation = ValidateAppointmentDate(dto.Date);
            if (dateValidation != null)
                return dateValidation;

            var existingAppointment = await _context.Appointments
                .FirstOrDefaultAsync(x =>
                    x.UserId == dto.UserId.Value &&
                    (x.Status == "Pending" || x.Status == "Approved") &&
                    x.Date > DateTime.Now);

            if (existingAppointment != null)
            {
                return BadRequest(new
                {
                    message = "Sizin artıq aktiv və ya gözləmədə olan rezervasiyanız var."
                });
            }

            var conflictAppointment = await _context.Appointments
                .FirstOrDefaultAsync(x =>
                    x.Date == dto.Date &&
                    x.Status != "Rejected" &&
                    x.Date > DateTime.Now);

            if (conflictAppointment != null)
            {
                return BadRequest(new
                {
                    message = "Bu tarix və saat artıq seçilib. Zəhmət olmasa başqa vaxt seçin."
                });
            }

            var dateValidation = ValidateAppointmentDate(dto.Date);
            if (dateValidation != null)
            {
                return dateValidation;
            }

            var existingAppointment = await _context.Appointments
                .FirstOrDefaultAsync(x =>
                    x.UserId == dto.UserId.Value &&
                    x.Status == "Pending" &&
                    x.Date > DateTime.Now);

            if (existingAppointment != null)
            {
                return BadRequest(new
                {
                    message = "Sizin artıq aktiv və gözləmədə olan rezervasiyanız var."
                });
            }

            var conflictAppointment = await _context.Appointments
                .FirstOrDefaultAsync(x =>
                    x.Date == dto.Date &&
                    x.Status != "Rejected" &&
                    x.Date > DateTime.Now);

            if (conflictAppointment != null)
            {
                return BadRequest(new
                {
                    message = "Bu tarix və saat artıq seçilib. Zəhmət olmasa başqa vaxt seçin."
                });
            }

            var appointment = new Appointment
            {
                FullName = dto.FullName,
                Phone = dto.Phone,
                Email = dto.Email,
                Service = dto.Service,
                Date = dto.Date,
                Message = dto.Message,
                UserId = dto.UserId.Value,
                Status = "Pending",
                ReminderSent = false,
                IsActive = true,
                CreatedAt = DateTime.Now,
                AdminResponseMessage = null,
                HasUserEdited = false
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            try
            {
                await _emailService.SendReservationEmail(
                    appointment.FullName,
                    appointment.Phone,
                    appointment.Email,
                    appointment.Service,
                    appointment.Date,
                    appointment.Message
                );
            }
            catch
            {
            }

            return Ok(new
            {
                message = "Rezervasiya uğurla yaradıldı.",
<<<<<<< HEAD
                appointment = new
                {
                    id = appointment.Id,
                    fullName = appointment.FullName,
                    phone = appointment.Phone,
                    email = appointment.Email,
                    service = appointment.Service,
                    date = appointment.Date,
                    message = appointment.Message,
                    status = appointment.Status,
                    userId = appointment.UserId,
                    isActive = appointment.IsActive,
                    createdAt = appointment.CreatedAt,
                    adminResponseMessage = appointment.AdminResponseMessage,
                    hasUserEdited = appointment.HasUserEdited
                }
=======
                appointment = MapAppointment(appointment)
>>>>>>> 2386656b82fc8776b7038d584c6b493cf03b5b3d
            });
        }

        [HttpGet("user/{userId:int}")]
        public async Task<IActionResult> GetByUser(int userId)
        {
            var appointments = await _context.Appointments
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.Id)
<<<<<<< HEAD
                .Select(x => new
                {
                    id = x.Id,
                    fullName = x.FullName,
                    phone = x.Phone,
                    email = x.Email,
                    service = x.Service,
                    date = x.Date,
                    message = x.Message,
                    status = x.Status,
                    userId = x.UserId,
                    isActive = x.IsActive,
                    createdAt = x.CreatedAt,
                    adminResponseMessage = x.AdminResponseMessage,
                    hasUserEdited = x.HasUserEdited
                })
=======
>>>>>>> 2386656b82fc8776b7038d584c6b493cf03b5b3d
                .ToListAsync();

            return Ok(appointments.Select(MapAppointment));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAppointmentDto dto)
        {
            var appointment = await _context.Appointments.FirstOrDefaultAsync(x => x.Id == id);

            if (appointment == null)
<<<<<<< HEAD
            {
                return NotFound(new { message = "Rezervasiya tapılmadı." });
            }

            if (dto.UserId <= 0 || appointment.UserId != dto.UserId)
            {
                return BadRequest(new { message = "Bu rezervasiyanı dəyişmək icazəniz yoxdur." });
            }

            if (appointment.Status != "Pending")
            {
                return BadRequest(new { message = "Admin tərəfindən baxılmış rezervasiya dəyişdirilə bilməz." });
            }

            if (appointment.Date <= DateTime.Now)
            {
                return BadRequest(new { message = "Vaxtı bitmiş rezervasiya dəyişdirilə bilməz." });
            }

            if (appointment.HasUserEdited)
            {
                return BadRequest(new { message = "Bu rezervasiyada artıq bir dəfə düzəliş etmisiniz." });
            }

            var dateValidation = ValidateAppointmentDate(dto.Date);
            if (dateValidation != null)
            {
                return dateValidation;
            }
=======
                return NotFound(new { message = "Rezervasiya tapılmadı." });

            if (dto.UserId <= 0 || appointment.UserId != dto.UserId)
                return BadRequest(new { message = "Bu rezervasiyanı dəyişmək icazəniz yoxdur." });

            if (appointment.Status != "Pending")
                return BadRequest(new { message = "Admin tərəfindən baxılmış rezervasiya dəyişdirilə bilməz." });

            if (appointment.Date <= DateTime.Now)
                return BadRequest(new { message = "Vaxtı bitmiş rezervasiya dəyişdirilə bilməz." });

            if (appointment.HasUserEdited)
                return BadRequest(new { message = "Bu rezervasiyada artıq bir dəfə düzəliş etmisiniz." });

            var dateValidation = ValidateAppointmentDate(dto.Date);
            if (dateValidation != null)
                return dateValidation;
>>>>>>> 2386656b82fc8776b7038d584c6b493cf03b5b3d

            var conflictAppointment = await _context.Appointments
                .FirstOrDefaultAsync(x =>
                    x.Id != id &&
                    x.Date == dto.Date &&
                    x.Status != "Rejected" &&
                    x.Date > DateTime.Now);

            if (conflictAppointment != null)
            {
                return BadRequest(new
                {
                    message = "Bu tarix və saat artıq seçilib. Zəhmət olmasa başqa vaxt seçin."
                });
            }

            appointment.Phone = dto.Phone;
            appointment.Service = dto.Service;
            appointment.Date = dto.Date;
            appointment.Message = dto.Message;
<<<<<<< HEAD

=======
>>>>>>> 2386656b82fc8776b7038d584c6b493cf03b5b3d
            appointment.Status = "Pending";
            appointment.ReminderSent = false;
            appointment.IsActive = true;
            appointment.AdminResponseMessage = null;
            appointment.HasUserEdited = true;

            await _context.SaveChangesAsync();

            try
            {
                await _emailService.SendReservationUpdatedEmail(
                    appointment.FullName,
                    appointment.Phone,
                    appointment.Email,
                    appointment.Service,
                    appointment.Date,
                    appointment.Message
                );
            }
            catch
            {
            }

            return Ok(new
            {
                message = "Rezervasiya yeniləndi. Artıq ikinci dəfə düzəliş etmək mümkün deyil.",
<<<<<<< HEAD
                appointment = new
                {
                    id = appointment.Id,
                    fullName = appointment.FullName,
                    phone = appointment.Phone,
                    email = appointment.Email,
                    service = appointment.Service,
                    date = appointment.Date,
                    message = appointment.Message,
                    status = appointment.Status,
                    userId = appointment.UserId,
                    isActive = appointment.IsActive,
                    createdAt = appointment.CreatedAt,
                    adminResponseMessage = appointment.AdminResponseMessage,
                    hasUserEdited = appointment.HasUserEdited
                }
=======
                appointment = MapAppointment(appointment)
>>>>>>> 2386656b82fc8776b7038d584c6b493cf03b5b3d
            });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var appointment = await _context.Appointments.FirstOrDefaultAsync(x => x.Id == id);

            if (appointment == null)
                return NotFound(new { message = "Rezervasiya tapılmadı." });

            if (appointment.Date <= DateTime.Now)
                return BadRequest(new { message = "Vaxtı bitmiş rezervasiyaya müdaxilə etmək olmaz." });

            if (appointment.Status == "Approved")
                return BadRequest(new { message = "Təsdiqlənmiş rezervasiya silinə bilməz." });

            if (appointment.Status != "Pending" && appointment.Status != "Rejected")
                return BadRequest(new { message = "Bu rezervasiyanı silmək mümkün deyil." });

            if (appointment.Date <= DateTime.Now)
            {
                return BadRequest(new { message = "Vaxtı bitmiş rezervasiyaya müdaxilə etmək olmaz." });
            }

            if (appointment.Status == "Approved")
            {
                return BadRequest(new { message = "Təsdiqlənmiş rezervasiya silinə bilməz." });
            }

            if (appointment.Status != "Pending" && appointment.Status != "Rejected")
            {
                return BadRequest(new { message = "Bu rezervasiyanı silmək mümkün deyil." });
            }

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Rezervasiya silindi." });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var appointments = await _context.Appointments
                .OrderByDescending(x => x.Id)
<<<<<<< HEAD
                .Select(x => new
                {
                    id = x.Id,
                    fullName = x.FullName,
                    phone = x.Phone,
                    email = x.Email,
                    service = x.Service,
                    date = x.Date,
                    message = x.Message,
                    status = x.Status,
                    userId = x.UserId,
                    isActive = x.IsActive,
                    createdAt = x.CreatedAt,
                    adminResponseMessage = x.AdminResponseMessage,
                    hasUserEdited = x.HasUserEdited
                })
                .ToListAsync();

            return Ok(appointments);
        }

        [HttpPut("{id:int}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateAppointmentStatusDto dto)
        {
            var appointment = await _context.Appointments.FirstOrDefaultAsync(x => x.Id == id);

            if (appointment == null)
            {
                return NotFound(new { message = "Rezervasiya tapılmadı." });
            }

            if (string.IsNullOrWhiteSpace(dto.Status))
            {
                return BadRequest(new { message = "Status boş ola bilməz." });
            }

            var allowedStatuses = new[] { "Approved", "Rejected" };

            if (!allowedStatuses.Contains(dto.Status))
            {
                return BadRequest(new { message = "Admin yalnız Approved və ya Rejected seçə bilər." });
            }

            if (appointment.Status != "Pending")
            {
                return BadRequest(new { message = "Bu rezervasiyaya artıq admin tərəfindən baxılıb." });
            }

            if (appointment.Date <= DateTime.Now)
            {
                return BadRequest(new { message = "Vaxtı bitmiş rezervasiyaya status vermək mümkün deyil." });
            }

            appointment.Status = dto.Status;
            appointment.AdminResponseMessage = dto.AdminResponseMessage;

            if (dto.Status == "Rejected")
            {
                appointment.IsActive = false;
                appointment.ReminderSent = false;
            }
            else if (dto.Status == "Approved")
            {
                appointment.IsActive = true;
            }

            await _context.SaveChangesAsync();

            try
            {
                if (dto.Status == "Approved")
                {
                    await _emailService.SendApprovedEmail(
                        appointment.FullName,
                        appointment.Email,
                        appointment.Service,
                        appointment.Date
                    );
                }
                else if (dto.Status == "Rejected")
                {
                    await _emailService.SendRejectedEmail(
                        appointment.FullName,
                        appointment.Email,
                        appointment.Service,
                        appointment.Date,
                        appointment.AdminResponseMessage
                    );
                }
            }
            catch
            {
            }

            return Ok(new { message = "Rezervasiya statusu yeniləndi." });
=======
                .ToListAsync();

            return Ok(appointments.Select(MapAppointment));
>>>>>>> 2386656b82fc8776b7038d584c6b493cf03b5b3d
        }

        [HttpGet("admin/all")]
        public async Task<IActionResult> GetAllForAdmin()
        {
            var appointments = await _context.Appointments
                .OrderByDescending(x => x.Id)
<<<<<<< HEAD
                .Select(x => new
                {
                    id = x.Id,
                    fullName = x.FullName,
                    phone = x.Phone,
                    email = x.Email,
                    service = x.Service,
                    date = x.Date,
                    message = x.Message,
                    status = x.Status,
                    userId = x.UserId,
                    isActive = x.IsActive,
                    createdAt = x.CreatedAt,
                    adminResponseMessage = x.AdminResponseMessage,
                    hasUserEdited = x.HasUserEdited
                })
=======
>>>>>>> 2386656b82fc8776b7038d584c6b493cf03b5b3d
                .ToListAsync();

            return Ok(appointments.Select(MapAppointment));
        }

        [HttpPut("{id:int}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateAppointmentStatusDto dto)
        {
            var appointment = await _context.Appointments.FirstOrDefaultAsync(x => x.Id == id);

            if (appointment == null)
                return NotFound(new { message = "Rezervasiya tapılmadı." });

            if (string.IsNullOrWhiteSpace(dto.Status))
                return BadRequest(new { message = "Status boş ola bilməz." });

            var allowedStatuses = new[] { "Approved", "Rejected" };

            if (!allowedStatuses.Contains(dto.Status))
                return BadRequest(new { message = "Admin yalnız Approved və ya Rejected seçə bilər." });

            if (appointment.Status != "Pending")
                return BadRequest(new { message = "Bu rezervasiyaya artıq admin tərəfindən baxılıb." });

            if (appointment.Date <= DateTime.Now)
                return BadRequest(new { message = "Vaxtı bitmiş rezervasiyaya status vermək mümkün deyil." });

            appointment.Status = dto.Status;
            appointment.AdminResponseMessage = dto.AdminResponseMessage;

            if (dto.Status == "Rejected")
            {
                appointment.IsActive = false;
                appointment.ReminderSent = false;
            }

            if (dto.Status == "Approved")
            {
                appointment.IsActive = true;
            }

            await _context.SaveChangesAsync();

            try
            {
                if (dto.Status == "Approved")
                {
                    await _emailService.SendApprovedEmail(
                        appointment.FullName,
                        appointment.Email,
                        appointment.Service,
                        appointment.Date
                    );
                }

                if (dto.Status == "Rejected")
                {
                    await _emailService.SendRejectedEmail(
                        appointment.FullName,
                        appointment.Email,
                        appointment.Service,
                        appointment.Date,
                        appointment.AdminResponseMessage
                    );
                }
            }
            catch
            {
            }

            return Ok(new
            {
                message = "Rezervasiya statusu yeniləndi.",
                appointment = MapAppointment(appointment)
            });
        }
    }
}