using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MyApp.Namespace
{
    [Route("api/[controller]")]
    [ApiController]
    public class MesaModel : ControllerBase
    {
        public int Id { get; set; }
    public string NumeroMesa { get; set; } = string.Empty; // Ej: "A1"
    public int Capacidad { get; set; }
    public int RestauranteId { get; set; }
    public RestaurantModel? Restaurant { get; set; }
    }
}
