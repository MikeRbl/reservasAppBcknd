using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace reservasApp.Namespace
{
    public class UserModel
    {
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty; // En producción, usa hash
    public string Rol { get; set; } = string.Empty; // "Admin", "Restaurante", "Cliente"
    }
}
