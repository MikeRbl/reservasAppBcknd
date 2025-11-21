using reservasApp.Models;
using reservasApp.DTOs;
using Microsoft.EntityFrameworkCore;

namespace reservasApp.Services
{
    public class ReservasService : IReservasService
    {
        private readonly AppDbContext _context;

        public ReservasService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CrearReservacion(ReservacionCreacionDTO dto, int userId)
        {
            var reserva = new ReservasModel 
            {
                FechaHora = dto.FechaHora,
                RestauranteId = dto.RestauranteId,
                ClienteId = userId,
                Estado = "Pendiente"
            };
            _context.Reservaciones.Add(reserva); // Asegúrate que en AppDbContext se llame "Reservaciones"
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> CancelarReservacion(int id, int userId)
        {
            var reserva = await _context.Reservaciones.FindAsync(id);
            if (reserva == null) return false;
            
            // Aquí podrías validar si el userId corresponde al dueño de la reserva
            reserva.Estado = "Cancelada";
            return await _context.SaveChangesAsync() > 0;
        }
    }
}