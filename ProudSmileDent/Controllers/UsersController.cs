using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProudSmileDent.Data;

namespace ProudSmileDent.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _context.Users
                .OrderByDescending(x => x.Id)
                .Select(x => new
                {
                    id = x.Id,
                    fullName = x.FullName,
                    username = x.Username,
                    email = x.Email,
                    role = x.Role,
                    createdAt = x.CreatedAt
                })
                .ToListAsync();

            return Ok(users);
        }
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                return NotFound(new { message = "İstifadəçi tapılmadı." });
            }

            if (user.Role == "Admin")
            {
                return BadRequest(new { message = "Admin istifadəçini silmək olmaz." });
            }

            var userAppointments = await _context.Appointments
                .Where(x => x.UserId == id)
                .ToListAsync();

            if (userAppointments.Any())
            {
                _context.Appointments.RemoveRange(userAppointments);
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "İstifadəçi və ona aid rezervasiyalar silindi." });
        }

        [HttpDelete("all")]
        public async Task<IActionResult> DeleteAll()
        {
            var appointments = await _context.Appointments.ToListAsync();
            if (appointments.Any())
            {
                _context.Appointments.RemoveRange(appointments);
            }

            var users = await _context.Users.ToListAsync();
            if (users.Any())
            {
                _context.Users.RemoveRange(users);
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Bütün istifadəçilər və onlara aid rezervasiyalar silindi." });
        }

    }
}