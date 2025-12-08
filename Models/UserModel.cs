using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace reservasApp.Models
{
    public class UserModel
    {
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string ApellidoPaterno { get; set; } = string.Empty;
    public string? ApellidoMaterno { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty; // En producción, usa hash
    public string Rol { get; set; } = string.Empty; // "Admin", "Restaurante", "Cliente"
    public string Telefono { get; set; } = string.Empty;
    }
}
