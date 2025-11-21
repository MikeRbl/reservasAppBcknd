using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiProyecto8;

namespace MyApp.Namespace
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservasService : ControllerBase
    {
        private readonly reservasApp.AppDbContext _context;

    public ReservasService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> CrearReservacion(ReservacionCreacionDTO dto, int userId)
    {
        var reserva = new Reservacion 
        {
            FechaHora = dto.FechaHora,
            RestauranteId = dto.RestauranteId,
            ClienteId = userId,
            Estado = "Pendiente"
        };
        _context.Reservaciones.Add(reserva);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> CancelarReservacion(int id, int userId)
    {
        var reserva = await _context.Reservaciones.FindAsync(id);
        // Validar que sea el dueño de la reserva o el restaurante
        if (reserva == null) return false;
        
        reserva.Estado = "Cancelada";
        return await _context.SaveChangesAsync() > 0;
    }
    }
}
