namespace reservasApp.DTOs
{
    public class ReservacionCreacionDTO
    {
        public int RestauranteId { get; set; }
        public DateTime FechaHora { get; set; }
        public int NumeroPersonas { get; set; } // Opcional si agregas este campo a tu modelo
    }
}