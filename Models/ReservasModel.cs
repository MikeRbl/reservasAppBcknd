using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using reservasApp.Models;

namespace reservasApp.Models
{
    public class ReservasModel
    {
        public int Id { get; set; }
        public DateTime FechaHora { get; set; }
        public string Estado { get; set; } = "Pendiente"; 
        
        public int ClienteId { get; set; }
        public UserModel? Cliente { get; set; }
        
        public int RestauranteId { get; set; }
        public RestaurantModel? Restaurant { get; set; }
        
        public int? MesaId { get; set; } 
        public MesaModel? Mesa { get; set; }
    }
}
