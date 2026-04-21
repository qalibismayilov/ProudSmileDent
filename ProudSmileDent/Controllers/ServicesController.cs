using Microsoft.AspNetCore.Mvc;
using DentClinicApi.Models;

namespace DentClinicApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServicesController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAll()
        {
            var services = new List<Service>
            {
                new Service { Id = 1, Category = "Stomatoloji qiymətlər", Name = "Diş çəkimi", Price = "50–150 AZN" },
                new Service { Id = 2, Category = "Stomatoloji qiymətlər", Name = "Kanal müalicəsi", Price = "10–20 AZN" },
                new Service { Id = 3, Category = "Stomatoloji qiymətlər", Name = "Ştift", Price = "15–25 AZN" },
                new Service { Id = 4, Category = "Stomatoloji qiymətlər", Name = "Plomb", Price = "40–150 AZN" },
                new Service { Id = 5, Category = "Stomatoloji qiymətlər", Name = "Güdül", Price = "40 AZN" },
                new Service { Id = 6, Category = "Stomatoloji qiymətlər", Name = "Diş ətinin müalicəsi", Price = "150 AZN" },
                new Service { Id = 7, Category = "Stomatoloji qiymətlər", Name = "Keramika", Price = "120–150 AZN" },
                new Service { Id = 8, Category = "Stomatoloji qiymətlər", Name = "Zirkon", Price = "200–350 AZN" },
                new Service { Id = 9, Category = "Stomatoloji qiymətlər", Name = "Protez", Price = "450–800 AZN" },
                new Service { Id = 10, Category = "Stomatoloji qiymətlər", Name = "Titan", Price = "70 AZN" },
                new Service { Id = 11, Category = "Stomatoloji qiymətlər", Name = "Qızıl", Price = "70 AZN" },
                new Service { Id = 12, Category = "Stomatoloji qiymətlər", Name = "Breket", Price = "1800–3500 AZN" },
                new Service { Id = 13, Category = "Stomatoloji qiymətlər", Name = "İmplant", Price = "450–2500 AZN" },
                new Service { Id = 14, Category = "Stomatoloji qiymətlər", Name = "İmplantüstü", Price = "350–500 AZN" },
                new Service { Id = 15, Category = "Stomatoloji qiymətlər", Name = "Vinir", Price = "450–600 AZN" },

                new Service { Id = 16, Category = "Dişin ağardılması", Name = "Kimyəvi ağartma", SubName = "Həssas olmayan dişlər üçün", Price = "450 AZN" },
                new Service { Id = 17, Category = "Dişin ağardılması", Name = "Təbii ağardılma", SubName = "AIRFLOW", Price = "50 AZN" },
                new Service { Id = 18, Category = "Dişin ağardılması", Name = "Təbii ağardılma", SubName = "Polirovka (pasta)", Price = "40 AZN" },
                new Service { Id = 19, Category = "Dişin ağardılması", Name = "Diş daşlarının təmizlənməsi", Price = "50 AZN" },
                new Service { Id = 20, Category = "Dişin ağardılması", Name = "Diş lakı", Price = "50 AZN" },
                new Service { Id = 21, Category = "Dişin ağardılması", Name = "Zoom-la ağartma", Price = "1200 AZN" }
            };

            return Ok(services);
        }
    }
}