using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MyApp.Namespace
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservasModel : ControllerBase
    {
        public int Id { get; set; }
    public DateTime FechaHora { get; set; }
    public string Estado { get; set; } = "Pendiente"; // "Aceptada", "Cancelada"
    
    public int ClienteId { get; set; }
    public UserModel? Cliente { get; set; }
    
    public int RestauranteId { get; set; }
    public RestaurantModel? Restaurant { get; set; }
    
    public int? MesaId { get; set; } // Nulo hasta que el restaurante la asigne
    public MesaModel? Mesa { get; set; }
    }
}
