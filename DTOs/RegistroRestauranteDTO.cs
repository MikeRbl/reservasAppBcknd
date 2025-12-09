using System.Text.Json.Serialization;

namespace reservasApp.DTOs
{
    public class RegistroRestauranteDTO
    {
        public string NombreRestaurante { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
    }
}