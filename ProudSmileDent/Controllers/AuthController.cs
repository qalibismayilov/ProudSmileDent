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
        private readonly EmailService _emailService;

        public AuthController(
            AppDbContext context,
            PasswordService passwordService,
            EmailService emailService)
        {
            _context = context;
            _passwordService = passwordService;
            _emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FullName) ||
                string.IsNullOrWhiteSpace(dto.Username) ||
                string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Password) ||
                string.IsNullOrWhiteSpace(dto.ConfirmPassword))
            {
                return BadRequest(new { message = "Bütün xanaları doldurun." });
            }

            if (dto.Password != dto.ConfirmPassword)
            {
                return BadRequest(new { message = "Şifrələr uyğun deyil." });
            }

            var exists = await _context.Users.AnyAsync(x =>
                x.Username == dto.Username || x.Email == dto.Email);

            if (exists)
            {
                return BadRequest(new { message = "İstifadəçi artıq mövcuddur." });
            }

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
                CreatedAt = DateTime.UtcNow,
                ResetCode = null,
                ResetCodeExpiresAt = null
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Qeydiyyat uğurludur.",
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

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
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
        public IActionResult AdminLogin([FromBody] LoginDto dto)
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

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Email))
                {
                    return BadRequest(new { message = "E-poçtu daxil edin." });
                }

                var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == dto.Email);

                if (user == null)
                {
                    return BadRequest(new { message = "Bu e-poçt ilə istifadəçi tapılmadı." });
                }

                var random = new Random();
                var code = random.Next(100000, 999999).ToString();

                user.ResetCode = code;
                user.ResetCodeExpiresAt = DateTime.UtcNow.AddMinutes(10);

                await _context.SaveChangesAsync();
                await _emailService.SendResetCodeEmail(user.Email, code);

                return Ok(new { message = "Reset kodu e-poçta göndərildi." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Forgot password zamanı server xətası baş verdi.",
                    detail = ex.Message
                });
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Email) ||
                    string.IsNullOrWhiteSpace(dto.Code) ||
                    string.IsNullOrWhiteSpace(dto.NewPassword) ||
                    string.IsNullOrWhiteSpace(dto.ConfirmPassword))
                {
                    return BadRequest(new { message = "Bütün xanaları doldurun." });
                }

                if (dto.NewPassword != dto.ConfirmPassword)
                {
                    return BadRequest(new { message = "Yeni şifrələr uyğun deyil." });
                }

                var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == dto.Email);

                if (user == null)
                {
                    return BadRequest(new { message = "İstifadəçi tapılmadı." });
                }

                if (string.IsNullOrWhiteSpace(user.ResetCode) || user.ResetCodeExpiresAt == null)
                {
                    return BadRequest(new { message = "Reset kodu tapılmadı." });
                }

                if (user.ResetCode != dto.Code)
                {
                    return BadRequest(new { message = "Kod yanlışdır." });
                }

                if (user.ResetCodeExpiresAt < DateTime.UtcNow)
                {
                    return BadRequest(new { message = "Kodun vaxtı bitib." });
                }

                user.PasswordHash = _passwordService.HashPassword(dto.NewPassword);
                user.ResetCode = null;
                user.ResetCodeExpiresAt = null;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Şifrə uğurla yeniləndi." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Reset password zamanı server xətası baş verdi.",
                    detail = ex.Message
                });
            }
        }
    }
}