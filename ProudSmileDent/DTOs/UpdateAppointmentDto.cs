namespace ProudSmileDent.DTOs
{
    public class UpdateAppointmentDto
    {
        public string Phone { get; set; } = string.Empty;

        public string Service { get; set; } = string.Empty;

        public DateTime Date { get; set; }

        public string? Message { get; set; }

        public int UserId { get; set; }
    }
}