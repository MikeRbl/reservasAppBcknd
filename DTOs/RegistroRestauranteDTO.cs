using System.Text.Json.Serialization;

namespace reservasApp.DTOs
{
    public class RegistroRestauranteDTO
    {
        // Datos del Dueño (Usuario)
        [JsonPropertyName("nombreDueno")]
        public string NombreDueño { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string TelefonoDueño { get; set; } = string.Empty;

        // Datos del Restaurante
        public string NombreRestaurante { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string TelefonoRestaurante { get; set; } = string.Empty;
        public string ImagenUrl { get; set; } = string.Empty; // Opcional por ahora
    }
}