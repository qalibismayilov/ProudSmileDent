namespace DentClinicApi.Models
{
    public class Service
    {
        public int Id { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? SubName { get; set; }
        public string Price { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}