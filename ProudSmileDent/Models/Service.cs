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

        public decimal OriginalPrice { get; set; }

        public decimal CurrentPrice { get; set; }

        public bool IsDiscountActive { get; set; } = false;

        public DateTime? DiscountStart { get; set; }

        public DateTime? DiscountEnd { get; set; }
    }
}