namespace ProudSmileDent.Models
{
    public class ChatMessage
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public string Sender { get; set; } = "User";

        public bool IsReadByAdmin { get; set; } = false;

        public bool IsReadByUser { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}