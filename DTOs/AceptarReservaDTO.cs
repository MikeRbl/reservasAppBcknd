namespace reservasApp.DTOs
{
    public class AceptarReservaDTO
    {
        public int ReservaId { get; set; }
        public bool Aprobada { get; set; } // true = Confirmar, false = Rechazar
        public int? MesaId { get; set; }   // Opcional, si queremos asignar mesa al aceptar
    }
}