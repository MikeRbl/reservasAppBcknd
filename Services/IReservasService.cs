using reservasApp.DTOs; // Asumiendo que crearás esta carpeta

namespace reservasApp.Services
{
    public interface IReservasService
    {
        Task<bool> CrearReservacion(ReservacionCreacionDTO dto, int userId);
        Task<bool> CancelarReservacion(int id, int userId);
    }
}