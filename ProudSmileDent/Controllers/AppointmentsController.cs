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

        [HttpPost]
        public async Task<IActionResult> Create(AppointmentCreateDto dto)
        {
            if (dto.UserId == null)
            {
                return Unauthorized(new { message = "Rezervasiya üçün əvvəlcə daxil olun." });
            }

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == dto.UserId.Value);

            if (user == null)
            {
                return Unauthorized(new { message = "İstifadəçi tapılmadı." });
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
                Status = "Pending"
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
                appointmentId = appointment.Id
            });
        }

        [HttpGet("user/{userId:int}")]
        public async Task<IActionResult> GetByUser(int userId)
        {
            var appointments = await _context.Appointments
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.Id)
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
                    userId = x.UserId
                })
                .ToListAsync();

            return Ok(appointments);
        }
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var appointment = await _context.Appointments.FirstOrDefaultAsync(x => x.Id == id);

            if (appointment == null)
            {
                return NotFound(new { message = "Rezervasiya tapılmadı." });
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
                    userId = x.UserId
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

            var allowedStatuses = new[] { "Pending", "Approved", "Rejected" };

            if (!allowedStatuses.Contains(dto.Status))
            {
                return BadRequest(new { message = "Yanlış status göndərildi." });
            }

            appointment.Status = dto.Status;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Rezervasiya statusu yeniləndi." });
        }

        [HttpGet("admin/all")]
        public async Task<IActionResult> GetAllForAdmin()
        {
            var appointments = await _context.Appointments
                .OrderByDescending(x => x.Id)
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
                    userId = x.UserId
                })
                .ToListAsync();

            return Ok(appointments);
        }
    }
}