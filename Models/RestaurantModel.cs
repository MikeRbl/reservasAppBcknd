using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema; // <--- IMPORTANTE: Agrega esto

using reservasApp.Models;
namespace reservasApp.Models
{
    public class RestaurantModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public bool EstaAprobado { get; set; } = false;
        
        public int UsuarioId { get; set; } // Esta es tu columna real en BD

        // Agregamos el atributo ForeignKey apuntando a la variable de arriba
        [ForeignKey("UsuarioId")] 
        public UserModel? Dueno { get; set; } 
        
        public List<MesaModel> Mesas { get; set; } = new(); 
    }
}