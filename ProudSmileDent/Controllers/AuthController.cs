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
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly PasswordService _passwordService;

        public AuthController(AppDbContext context, PasswordService passwordService)
        {
            _context = context;
            _passwordService = passwordService;
        }

        // 🔹 REGISTER
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (dto.Password != dto.ConfirmPassword)
                return BadRequest(new { message = "Şifrələr uyğun deyil" });

            var exists = await _context.Users.AnyAsync(x =>
                x.Username == dto.Username || x.Email == dto.Email);

            if (exists)
                return BadRequest(new { message = "İstifadəçi artıq mövcuddur" });

            var role = "User";

            if (dto.Username.Trim().ToLower() == "admin")
            {
                role = "Admin";
            }

            var user = new User
            {
                FullName = dto.FullName,
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = _passwordService.HashPassword(dto.Password),
                Role = role,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Qeydiyyat uğurlu",
                user = new
                {
                    id = user.Id,
                    fullName = user.FullName,
                    username = user.Username,
                    email = user.Email
                }
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.UsernameOrEmail) || string.IsNullOrWhiteSpace(dto.Password))
            {
                return BadRequest(new { message = "İstifadəçi adı və şifrəni daxil edin." });
            }

            var user = await _context.Users.FirstOrDefaultAsync(x =>
                x.Username == dto.UsernameOrEmail || x.Email == dto.UsernameOrEmail);

            if (user == null)
            {
                return BadRequest(new { message = "İstifadəçi tapılmadı." });
            }

            var valid = _passwordService.VerifyPassword(dto.Password, user.PasswordHash);

            if (!valid)
            {
                return BadRequest(new { message = "Şifrə yanlışdır." });
            }

            return Ok(new
            {
                message = "Giriş uğurludur.",
                user = new
                {
                    id = user.Id,
                    fullName = user.FullName,
                    username = user.Username,
                    email = user.Email,
                    role = user.Role
                }
            });
        }
        [HttpPost("admin-login")]
        public IActionResult AdminLogin(LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.UsernameOrEmail) || string.IsNullOrWhiteSpace(dto.Password))
            {
                return BadRequest(new { message = "Admin istifadəçi adı və kodu daxil edin." });
            }

            const string adminUsername = "admin";
            const string adminCode = "12345admin";

            if (dto.UsernameOrEmail.Trim().ToLower() != adminUsername)
            {
                return BadRequest(new { message = "Admin istifadəçi adı yanlışdır." });
            }

            if (dto.Password != adminCode)
            {
                return BadRequest(new { message = "Admin kodu yanlışdır." });
            }

            return Ok(new
            {
                message = "Admin giriş uğurludur.",
                user = new
                {
                    id = 0,
                    fullName = "System Admin",
                    username = "admin",
                    email = "admin@proudsmiledent.local",
                    role = "Admin"
                }
            });
        }
    }
}