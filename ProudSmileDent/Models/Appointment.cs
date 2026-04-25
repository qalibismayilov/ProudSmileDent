namespace ProudSmileDent.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Service { get; set; } = string.Empty;

        public DateTime Date { get; set; }

        public string? Message { get; set; }

        public string Status { get; set; } = "Pending";

        public int? UserId { get; set; }

        public bool ReminderSent { get; set; } = false;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

<<<<<<< HEAD
      
        public string? AdminResponseMessage { get; set; }
=======
        public string? AdminResponseMessage { get; set; }
        public bool HasUserEdited { get; set; } = false;
>>>>>>> 90e0734 (update)
    }
}