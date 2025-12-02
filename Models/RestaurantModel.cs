using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using reservasApp.Models;
namespace reservasApp.Models
{
    public class RestaurantModel
    {
        public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public bool EstaAprobado { get; set; } = false;
    public int UsuarioId { get; set; } // Foreign key to the UserModel
    public UserModel? Dueño { get; set; } // Navigation property for the owner
    public List<MesaModel> Mesas { get; set; } = new(); // Navigation property for tables
}
}
