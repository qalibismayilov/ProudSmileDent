using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProudSmileDent.Data;
using ProudSmileDent.Models;

namespace ProudSmileDent.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ChatController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] ChatMessage dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Message))
                return BadRequest(new { message = "Mesaj boş ola bilməz." });

            if (dto.UserId == null || dto.UserId <= 0)
                return BadRequest(new { message = "Chat üçün əvvəlcə istifadəçi hesabı ilə daxil olun." });

            var message = new ChatMessage
            {
                UserId = dto.UserId,
                FullName = dto.FullName ?? string.Empty,
                Email = dto.Email ?? string.Empty,
                Message = dto.Message.Trim(),
                Sender = "User",
                IsReadByAdmin = false,
                IsReadByUser = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.ChatMessages.Add(message);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Mesaj göndərildi.",
                chatMessage = message
            });
        }

        [HttpPost("admin/reply")]
        public async Task<IActionResult> AdminReply([FromBody] ChatMessage dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Message))
                return BadRequest(new { message = "Cavab mesajı boş ola bilməz." });

            if (dto.UserId == null || dto.UserId <= 0)
                return BadRequest(new { message = "İstifadəçi seçilməlidir." });

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == dto.UserId);

            var reply = new ChatMessage
            {
                UserId = dto.UserId,
                FullName = user?.Username ?? dto.FullName ?? "İstifadəçi",
                Email = user?.Email ?? dto.Email ?? string.Empty,
                Message = dto.Message.Trim(),
                Sender = "Admin",
                IsReadByAdmin = true,
                IsReadByUser = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.ChatMessages.Add(reply);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Admin cavabı göndərildi.",
                chatMessage = reply
            });
        }

        [HttpGet("user/{userId:int}")]
        public async Task<IActionResult> GetByUser(int userId)
        {
            var messages = await _context.ChatMessages
                .Where(x => x.UserId == userId)
                .OrderBy(x => x.CreatedAt)
                .Select(x => new
                {
                    id = x.Id,
                    userId = x.UserId,
                    fullName = x.FullName,
                    email = x.Email,
                    message = x.Message,
                    sender = x.Sender,
                    isReadByAdmin = x.IsReadByAdmin,
                    isReadByUser = x.IsReadByUser,
                    createdAt = x.CreatedAt
                })
                .ToListAsync();

            return Ok(messages);
        }

        [HttpGet("admin/all")]
        public async Task<IActionResult> GetAllForAdmin()
        {
            var messages = await _context.ChatMessages
                .OrderBy(x => x.CreatedAt)
                .Select(x => new
                {
                    id = x.Id,
                    userId = x.UserId,
                    fullName = x.FullName,
                    email = x.Email,
                    message = x.Message,
                    sender = x.Sender,
                    isReadByAdmin = x.IsReadByAdmin,
                    isReadByUser = x.IsReadByUser,
                    createdAt = x.CreatedAt
                })
                .ToListAsync();

            return Ok(messages);
        }

        [HttpGet("admin/conversations")]
        public async Task<IActionResult> GetConversationsForAdmin()
        {
            var conversations = await _context.ChatMessages
                .Where(x => x.UserId != null)
                .GroupBy(x => x.UserId)
                .Select(g => new
                {
                    userId = g.Key,
                    fullName = g.OrderByDescending(x => x.CreatedAt).First().FullName,
                    email = g.OrderByDescending(x => x.CreatedAt).First().Email,
                    lastMessage = g.OrderByDescending(x => x.CreatedAt).First().Message,
                    lastSender = g.OrderByDescending(x => x.CreatedAt).First().Sender,
                    lastDate = g.Max(x => x.CreatedAt),
                    unreadForAdmin = g.Count(x => x.Sender == "User" && !x.IsReadByAdmin),
                    totalMessages = g.Count()
                })
                .OrderByDescending(x => x.lastDate)
                .ToListAsync();

            return Ok(conversations);
        }

        [HttpGet("admin/user/{userId:int}")]
        public async Task<IActionResult> GetConversationByUserForAdmin(int userId)
        {
            var messages = await _context.ChatMessages
                .Where(x => x.UserId == userId)
                .OrderBy(x => x.CreatedAt)
                .Select(x => new
                {
                    id = x.Id,
                    userId = x.UserId,
                    fullName = x.FullName,
                    email = x.Email,
                    message = x.Message,
                    sender = x.Sender,
                    isReadByAdmin = x.IsReadByAdmin,
                    isReadByUser = x.IsReadByUser,
                    createdAt = x.CreatedAt
                })
                .ToListAsync();

            var unreadMessages = await _context.ChatMessages
                .Where(x => x.UserId == userId && x.Sender == "User" && !x.IsReadByAdmin)
                .ToListAsync();

            foreach (var msg in unreadMessages)
                msg.IsReadByAdmin = true;

            if (unreadMessages.Any())
                await _context.SaveChangesAsync();

            return Ok(messages);
        }

        [HttpGet("user/{userId:int}/unread-count")]
        public async Task<IActionResult> GetUnreadCountForUser(int userId)
        {
            var count = await _context.ChatMessages
                .CountAsync(x => x.UserId == userId && x.Sender == "Admin" && !x.IsReadByUser);

            return Ok(new { unreadCount = count });
        }

        [HttpPut("user/{userId:int}/mark-read")]
        public async Task<IActionResult> MarkUserMessagesAsRead(int userId)
        {
            var unreadMessages = await _context.ChatMessages
                .Where(x => x.UserId == userId && x.Sender == "Admin" && !x.IsReadByUser)
                .ToListAsync();

            foreach (var msg in unreadMessages)
                msg.IsReadByUser = true;

            if (unreadMessages.Any())
                await _context.SaveChangesAsync();

            return Ok(new { message = "Mesajlar oxundu olaraq işarələndi." });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            var message = await _context.ChatMessages.FirstOrDefaultAsync(x => x.Id == id);

            if (message == null)
                return NotFound(new { message = "Mesaj tapılmadı." });

            _context.ChatMessages.Remove(message);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Mesaj silindi." });
        }

        [HttpPost("delete-selected")]
        public async Task<IActionResult> DeleteSelectedMessages([FromBody] DeleteSelectedChatMessagesRequest request)
        {
            if (request == null || request.Ids == null || !request.Ids.Any())
                return BadRequest(new { message = "Silinmək üçün mesaj seçilməyib." });

            var messages = await _context.ChatMessages
                .Where(x => request.Ids.Contains(x.Id))
                .ToListAsync();

            if (!messages.Any())
                return NotFound(new { message = "Seçilmiş mesajlar tapılmadı." });

            _context.ChatMessages.RemoveRange(messages);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Seçilmiş mesajlar silindi.",
                deletedCount = messages.Count
            });
        }

        [HttpDelete("admin/user/{userId:int}/clear")]
        public async Task<IActionResult> ClearUserConversation(int userId)
        {
            var messages = await _context.ChatMessages
                .Where(x => x.UserId == userId)
                .ToListAsync();

            if (messages.Any())
            {
                _context.ChatMessages.RemoveRange(messages);
                await _context.SaveChangesAsync();
            }

            return Ok(new { message = "İstifadəçinin söhbəti silindi." });
        }

        [HttpDelete("admin/clear")]
        public async Task<IActionResult> ClearAllMessages()
        {
            var messages = await _context.ChatMessages.ToListAsync();

            if (messages.Any())
            {
                _context.ChatMessages.RemoveRange(messages);
                await _context.SaveChangesAsync();
            }

            return Ok(new { message = "Bütün chat mesajları silindi." });
        }
    }

    public class DeleteSelectedChatMessagesRequest
    {
        public List<int> Ids { get; set; } = new List<int>();
    }
}